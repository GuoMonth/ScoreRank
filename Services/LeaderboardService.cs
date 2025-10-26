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
        public LeaderboardService()
        {
            _leaderboard = new Leaderboard();
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
        /// Adds or updates a customer's score and updates ranks accordingly.
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="scoreDelta"></param>
        public void AddOrUpdateCustomerScore(long customerId, decimal scoreDelta)
        {
            _leaderboard.AddOrUpdateCustomer(customerId, scoreDelta);
        }
    }
}