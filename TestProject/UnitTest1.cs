using Microsoft.AspNetCore.Mvc.Testing;
using ScoreRank;
using System.Net.Http.Json;
using ScoreRank.Models;

namespace TestProject
{
    public class LeaderboardIntegrationTests
    {
        private static readonly List<(long CustomerId, decimal Score)> TestData = new()
        {
            (1, 100),
            (2, 200),
            (3, 150),
            (4, 50),
            (5, 50),
            (6, -100),
            (7, 75),
            (8, 300),
            (9, 25),
            (10, 0)
        };

        private async Task InitializeTestData(HttpClient client)
        {
            foreach (var (customerId, score) in TestData)
            {
                var response = await client.PostAsync($"/api/customer/{customerId}/score/{score}", null);
                response.EnsureSuccessStatusCode();
            }
        }

        [Fact]
        public async Task TestRankRangeQueryAccuracy()
        {
            // Arrange
            using var factory = new WebApplicationFactory<Program>();
            using var client = factory.CreateClient();
            await InitializeTestData(client);

            // Act
            var response = await client.GetAsync("/api/leaderboard?start=1&end=5");
            response.EnsureSuccessStatusCode();
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<CustomerRank>>>();

            // Assert
            Assert.NotNull(apiResponse);
            Assert.True(apiResponse.Success);
            Assert.NotNull(apiResponse.Data);
            Assert.Equal(5, apiResponse.Data.Count);

            // Expected ranks: 8(300), 2(200), 3(150), 1(100), 7(75)
            var expected = new List<(long CustomerId, decimal Score, long Rank)>
            {
                (8, 300, 1),
                (2, 200, 2),
                (3, 150, 3),
                (1, 100, 4),
                (7, 75, 5)
            };

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i].CustomerId, apiResponse.Data[i].customerScore.CustomerId);
                Assert.Equal(expected[i].Score, apiResponse.Data[i].customerScore.Score);
                Assert.Equal(expected[i].Rank, apiResponse.Data[i].Rank);
            }
        }

        [Fact]
        public async Task TestMidRangeAndBoundaryQuery()
        {
            // Arrange
            using var factory = new WebApplicationFactory<Program>();
            using var client = factory.CreateClient();
            await InitializeTestData(client);

            // Act: Mid range
            var response = await client.GetAsync("/api/leaderboard?start=3&end=7");
            response.EnsureSuccessStatusCode();
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<CustomerRank>>>();

            // Assert
            Assert.NotNull(apiResponse);
            Assert.True(apiResponse.Success);
            Assert.NotNull(apiResponse.Data);
            Assert.Equal(5, apiResponse.Data.Count);

            // Expected: 3(150), 1(100), 7(75), 4(50), 5(50)
            var expectedMid = new List<(long CustomerId, decimal Score, long Rank)>
            {
                (3, 150, 3),
                (1, 100, 4),
                (7, 75, 5),
                (4, 50, 6),
                (5, 50, 7)
            };

            for (int i = 0; i < expectedMid.Count; i++)
            {
                Assert.Equal(expectedMid[i].CustomerId, apiResponse.Data[i].customerScore.CustomerId);
                Assert.Equal(expectedMid[i].Score, apiResponse.Data[i].customerScore.Score);
                Assert.Equal(expectedMid[i].Rank, apiResponse.Data[i].Rank);
            }

            // Act: Full range
            var fullResponse = await client.GetAsync("/api/leaderboard?start=1&end=10");
            fullResponse.EnsureSuccessStatusCode();
            var fullApiResponse = await fullResponse.Content.ReadFromJsonAsync<ApiResponse<List<CustomerRank>>>();

            Assert.NotNull(fullApiResponse);
            Assert.True(fullApiResponse.Success);
            Assert.NotNull(fullApiResponse.Data);
            Assert.Equal(10, fullApiResponse.Data.Count);

            // Act: Out of range
            var outResponse = await client.GetAsync("/api/leaderboard?start=11&end=15");
            outResponse.EnsureSuccessStatusCode();
            var outApiResponse = await outResponse.Content.ReadFromJsonAsync<ApiResponse<List<CustomerRank>>>();

            Assert.NotNull(outApiResponse);
            Assert.True(outApiResponse.Success);
            Assert.NotNull(outApiResponse.Data);
            Assert.Empty(outApiResponse.Data);
        }

        [Fact]
        public async Task TestCustomerNeighborsQueryAccuracy()
        {
            // Arrange
            using var factory = new WebApplicationFactory<Program>();
            using var client = factory.CreateClient();
            await InitializeTestData(client);

            // Act: Customer ID 5 (Rank 7, Score 50), high=2, low=1
            var response = await client.GetAsync("/api/leaderboard/5?high=2&low=1");
            response.EnsureSuccessStatusCode();
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<CustomerRank>>>();

            // Assert
            Assert.NotNull(apiResponse);
            Assert.True(apiResponse.Success);
            Assert.NotNull(apiResponse.Data);
            Assert.Equal(4, apiResponse.Data.Count); // high=2, low=1, plus self

            // Expected: Rank 5(7,75), Rank 6(4,50), Rank 7(5,50), Rank 8(9,25)
            var expected = new List<(long CustomerId, decimal Score, long Rank)>
            {
                (7, 75, 5),
                (4, 50, 6),
                (5, 50, 7),
                (9, 25, 8)
            };

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i].CustomerId, apiResponse.Data[i].customerScore.CustomerId);
                Assert.Equal(expected[i].Score, apiResponse.Data[i].customerScore.Score);
                Assert.Equal(expected[i].Rank, apiResponse.Data[i].Rank);
            }
        }

        [Fact]
        public async Task TestTieScoreIdSorting()
        {
            // Arrange
            using var factory = new WebApplicationFactory<Program>();
            using var client = factory.CreateClient();
            await InitializeTestData(client);

            // Act
            var response = await client.GetAsync("/api/leaderboard?start=1&end=10");
            response.EnsureSuccessStatusCode();
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<CustomerRank>>>();

            // Assert
            Assert.NotNull(apiResponse);
            Assert.True(apiResponse.Success);
            Assert.NotNull(apiResponse.Data);
            Assert.Equal(10, apiResponse.Data.Count);

            // Check tie scores: Rank 6 and 7 both Score 50, ID 4 before 5
            var rank6 = apiResponse.Data[5]; // Index 5 is Rank 6
            var rank7 = apiResponse.Data[6]; // Index 6 is Rank 7

            Assert.Equal(50, rank6.customerScore.Score);
            Assert.Equal(50, rank7.customerScore.Score);
            Assert.Equal(4, rank6.customerScore.CustomerId);
            Assert.Equal(5, rank7.customerScore.CustomerId);
            Assert.Equal(6, rank6.Rank);
            Assert.Equal(7, rank7.Rank);
        }
    }
}