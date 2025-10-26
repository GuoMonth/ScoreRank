using Microsoft.AspNetCore.Mvc.Testing;
using ScoreRank;
using System.Net.Http.Json;
using ScoreRank.Models;
using System.Diagnostics;

namespace TestProject
{
    public class PerformanceIntegrationTests
    {
        private static readonly int LargeDataSize = 100000; // 10万条随机数据
        private static readonly int SpecialDataSize = 10; // 10条特殊数据
        private static readonly Random Random = new Random(42); // 固定种子确保可重现

        private static List<(long CustomerId, decimal Score)> GenerateRandomData()
        {
            var data = new List<(long, decimal)>();
            for (long i = 1; i <= LargeDataSize; i++)
            {
                var score = (decimal)(Random.Next(-1000, 1000)); // -1000 to 999
                data.Add((i, score));
            }
            return data;
        }

        private static List<(long CustomerId, decimal Score)> GenerateSpecialData()
        {
            var data = new List<(long, decimal)>();
            for (long i = LargeDataSize + 1; i <= LargeDataSize + SpecialDataSize; i++)
            {
                var score = 999m; // 高于随机数据最大1000，确保排名在前
                data.Add((i, score));
            }
            return data;
        }

        private async Task InitializeLargeData(HttpClient client)
        {
            var data = GenerateRandomData();
            foreach (var (customerId, score) in data)
            {
                var response = await client.PostAsync($"/api/customer/{customerId}/score/{score}", null);
                response.EnsureSuccessStatusCode();
            }
        }

        [Fact]
        public async Task TestLargeScaleDataPerformance()
        {
            // Arrange
            using var factory = new WebApplicationFactory<Program>();
            using var client = factory.CreateClient();
            var stopwatch = new Stopwatch();

            // Step 1: Write 900,000 random data
            await InitializeLargeData(client);

            // Step 2: Write 10 special data and record average time
            var specialData = GenerateSpecialData();
            var writeTimes = new List<long>();
            foreach (var (customerId, score) in specialData)
            {
                stopwatch.Restart();
                var response = await client.PostAsync($"/api/customer/{customerId}/score/{score}", null);
                stopwatch.Stop();
                response.EnsureSuccessStatusCode();
                writeTimes.Add(stopwatch.ElapsedMilliseconds);
            }
            var averageWriteTime = writeTimes.Average();

            // Log average write time
            Console.WriteLine($"Average write time for 10 special data: {averageWriteTime} ms");

            // Step 3: Query special data and record time
            stopwatch.Restart();
            var queryResponse = await client.GetAsync("/api/leaderboard/100001?high=5&low=5");
            stopwatch.Stop();
            queryResponse.EnsureSuccessStatusCode();
            var apiResponse = await queryResponse.Content.ReadFromJsonAsync<ApiResponse<List<CustomerRank>>>();

            var queryTime = stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"Query time for customer 100001 with neighbors: {queryTime} ms");

            // Assert
            Assert.NotNull(apiResponse);
            Assert.True(apiResponse.Success);
            Assert.NotNull(apiResponse.Data);
            Assert.Equal(11, apiResponse.Data.Count); // self + 5 high + 5 low

            // Verify special customer is in the result
            var specialCustomer = apiResponse.Data.FirstOrDefault(r => r.customerScore.CustomerId == 100001);
            Assert.NotNull(specialCustomer);

            // Verify ranks are correct (descending score, ascending ID for ties)
            for (int i = 1; i < apiResponse.Data.Count; i++)
            {
                var prev = apiResponse.Data[i - 1];
                var curr = apiResponse.Data[i];
                Assert.True(prev.customerScore.Score >= curr.customerScore.Score);
                if (prev.customerScore.Score == curr.customerScore.Score)
                {
                    Assert.True(prev.customerScore.CustomerId < curr.customerScore.CustomerId);
                }
            }

            // Performance thresholds (adjustable)
            Assert.True(averageWriteTime < 100); // Average write < 100ms
            Assert.True(queryTime < 500); // Query < 500ms
        }
    }
}
