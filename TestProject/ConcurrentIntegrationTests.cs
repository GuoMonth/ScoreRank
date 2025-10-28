using Microsoft.AspNetCore.Mvc.Testing;
using ScoreRank;
using System.Net.Http.Json;
using ScoreRank.Models;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace TestProject
{
    public class ConcurrentIntegrationTests
    {
        // Constants
        private const int WriteThreads = 5;
        private const int QueryThreads = 2;
        private const int DataPerThread = 200000; // 20万 per thread
        private const int MaxQueryTime = 500; // ms
        private const int QueryDuration = 30000; // 30 seconds
        private static readonly Random Random = new Random(42);

        // Data generation
        private static List<(long CustomerId, decimal Score)> GenerateDataForThread(int threadIndex)
        {
            var data = new List<(long, decimal)>();
            long startId = (long)threadIndex * DataPerThread + 1;
            long endId = startId + DataPerThread - 1;

            for (long id = startId; id <= endId; id++)
            {
                var score = (decimal)Random.Next(-1000, 1000);
                data.Add((id, score));
            }
            return data;
        }

        // Concurrent write task
        private async Task WriteDataBatch(HttpClient client, int threadIndex)
        {
            var data = GenerateDataForThread(threadIndex);
            foreach (var (customerId, score) in data)
            {
                var response = await client.PostAsync($"/api/customer/{customerId}/score/{score}", null);
                response.EnsureSuccessStatusCode();
            }
        }

        // Random query task
        private async Task PerformRandomQueries(HttpClient client, ConcurrentBag<long> queryTimes, ConcurrentBag<bool> querySuccesses, CancellationToken cancellationToken)
        {
            var stopwatch = new Stopwatch();

            while (!cancellationToken.IsCancellationRequested)
            {
                // Random query type
                bool isRangeQuery = Random.Next(2) == 0;

                if (isRangeQuery)
                {
                    // Range query: /api/leaderboard?start=X&end=Y
                    int start = Random.Next(1, 100001);
                    int end = Random.Next(start, Math.Min(start + 100, 1000001));
                    var url = $"/api/leaderboard?start={start}&end={end}";

                    stopwatch.Restart();
                    var response = await client.GetAsync(url);
                    stopwatch.Stop();

                    queryTimes.Add(stopwatch.ElapsedMilliseconds);
                    querySuccesses.Add(response.IsSuccessStatusCode);
                }
                else
                {
                    // Neighbor query: /api/leaderboard/{id}?high=X&low=Y
                    long customerId = Random.Next(1, 1000001);
                    int high = Random.Next(1, 6);
                    int low = Random.Next(1, 6);
                    var url = $"/api/leaderboard/{customerId}?high={high}&low={low}";

                    stopwatch.Restart();
                    var response = await client.GetAsync(url);
                    stopwatch.Stop();

                    queryTimes.Add(stopwatch.ElapsedMilliseconds);
                    querySuccesses.Add(response.IsSuccessStatusCode);
                }

                // Small delay to avoid overwhelming
                await Task.Delay(10);
            }
        }

        [Fact]
        public async Task TestConcurrentWriteAndQueryPerformance()
        {
            // Arrange
            using var factory = new WebApplicationFactory<Program>();
            using var client = factory.CreateClient();

            var queryTimes = new ConcurrentBag<long>();
            var querySuccesses = new ConcurrentBag<bool>();
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(QueryDuration);

            // Act
            // Start query tasks FIRST (they will query existing data or get 404s)
            var queryTasks = new Task[QueryThreads];
            for (int i = 0; i < QueryThreads; i++)
            {
                queryTasks[i] = Task.Run(() =>
                    PerformRandomQueries(client, queryTimes, querySuccesses, cancellationTokenSource.Token));
            }

            // Start write tasks
            var writeTasks = new Task[WriteThreads];
            for (int i = 0; i < WriteThreads; i++)
            {
                int threadIndex = i;
                writeTasks[i] = Task.Run(() => WriteDataBatch(client, threadIndex));
            }

            // Wait for all write tasks to complete
            await Task.WhenAll(writeTasks);

            // Cancel query tasks after writes complete
            cancellationTokenSource.Cancel();

            // Wait for query tasks to finish
            await Task.WhenAll(queryTasks);

            // Assert
            var timesList = queryTimes.ToList();
            var successesList = querySuccesses.ToList();

            // In concurrent scenario, some queries may fail (404 for non-existent customers)
            // We only check that successful queries meet the time requirement
            var successfulQueries = timesList.Where((time, index) => successesList[index]).ToList();
            var failedQueries = successesList.Count(s => !s);

            // All successful queries should be within time limit
            Assert.All(successfulQueries, time => Assert.True(time <= MaxQueryTime));

            // Log performance results
            if (timesList.Any())
            {
                var avgTime = successfulQueries.Any() ? successfulQueries.Average() : 0;
                var maxTime = successfulQueries.Any() ? successfulQueries.Max() : 0;
                var totalQueries = timesList.Count;
                var successRate = (double)successfulQueries.Count / totalQueries * 100;

                Console.WriteLine($"Concurrent test completed:");
                Console.WriteLine($"- Total queries: {totalQueries}");
                Console.WriteLine($"- Successful queries: {successfulQueries.Count}");
                Console.WriteLine($"- Failed queries: {failedQueries}");
                Console.WriteLine($"- Success rate: {successRate:F1}%");
                Console.WriteLine($"- Average response time (successful): {avgTime:F2}ms");
                Console.WriteLine($"- Max response time (successful): {maxTime}ms");
                Console.WriteLine($"- All successful queries within {MaxQueryTime}ms: {successfulQueries.All(t => t <= MaxQueryTime)}");
            }
        }
    }
}