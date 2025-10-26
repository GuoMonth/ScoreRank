using ScoreRank.Models;

namespace ScoreRank.Services
{
    /// <summary>
    /// Service for managing leaderboard operations.
    /// </summary>
    public class LeaderboardService
    {
        /// <summary>
        /// The leaderboard instance.
        /// </summary>
        private readonly Leaderboard _leaderboard;


        /// <summary>
        /// Initializes a new instance of the <see cref="LeaderboardService"/> class.
        /// </summary>
        public LeaderboardService(Leaderboard leaderboard)
        {
            _leaderboard = leaderboard;
        }


        /// <summary>
        /// Gets the rank of a customer by their ID.
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public int GetCustomerRank(long customerId)
        {
            return _leaderboard.GetRankByCustomerId(customerId);
        }

        /// <summary>
        /// Gets customers by their rank range.
        /// </summary>
        /// <param name="start">Begin at 1. The starting rank position (inclusive, if exists).</param>
        /// <param name="end">Begin at 1. The ending rank position (inclusive, if exists).</param>
        /// <returns></returns>
        public IEnumerable<CustomerRank> GetCustomersByRankRange(int start, int end)
        {
            return _leaderboard.GetByRankRange(start, end);
        }

        /// <summary>
        /// Gets neighboring customers around a specific customer.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <param name="high">The number of higher-ranked neighbors to include. begins at 1.</param>
        /// <param name="low">The number of lower-ranked neighbors to include. begins at 1.</param>
        public IEnumerable<CustomerRank> GetWithNeighbors(long customerId, int high, int low)
        {
            return _leaderboard.GetWithNeighbors(customerId, high, low);
        }
    }
}