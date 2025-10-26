using ScoreRank.Data;
using ScoreRank.Models;

namespace ScoreRank.Services
{
    /// <summary>
    /// Service for managing customer-related operations.
    /// </summary>
    public class CustomerService
    {
        /// <summary>
        /// The leaderboard instance.
        /// </summary>
        private readonly Leaderboard _leaderboard;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerService"/> class.
        /// </summary>
        public CustomerService(DataFactory dataFactory)
        {
            _leaderboard = dataFactory.GetLeaderboard();
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