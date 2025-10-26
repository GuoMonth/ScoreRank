namespace ScoreRank.Models
{
    /// <summary>
    /// Represents the leaderboard containing customer rankings.
    /// </summary>
    public class Leaderboard
    {
        private SortedSet<CustomerRank> _sortedRanks;

        private Dictionary<long, CustomerRank> _customerDict;

        /// <summary>
        /// Initializes a new instance of the <see cref="Leaderboard"/> class.
        /// </summary>
        public Leaderboard()
        {
            _sortedRanks = new SortedSet<CustomerRank>(new Comparer());
            _customerDict = new Dictionary<long, CustomerRank>();
        }

        /// <summary>
        /// Gets the sorted set of customer ranks.
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public int GetRankByCustomerId(long customerId)
        {
            if (_customerDict.TryGetValue(customerId, out var customerRank))
            {
                return _sortedRanks.ToList().IndexOf(customerRank) + 1; // Rank starts at 1
            }
            return -1; // Customer not found
        }

        /// <summary>
        /// Adds or updates a customer's score and updates ranks accordingly.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <param name="scoreDelta">The score adjustment.</param>
        public void AddOrUpdateCustomer(long customerId, decimal scoreDelta)
        {
            if (_customerDict.TryGetValue(customerId, out var customerRank))
            {
                // Update existing customer
                _sortedRanks.Remove(customerRank);
                customerRank.Score += scoreDelta;
            }
            else
            {
                // Add new customer
                customerRank = new CustomerRank
                {
                    CustomerId = customerId,
                    Score = scoreDelta
                };
                _customerDict[customerId] = customerRank;
            }

            _sortedRanks.Add(customerRank);
        }

        /// <summary>
        /// Gets customers within the specified rank range.
        /// </summary>
        /// <param name="startRank">The starting rank (inclusive). begins at 1.</param>
        /// <param name="endRank">The ending rank (inclusive). begins at 1.</param>
        public IEnumerable<CustomerRank> GetByRankRange(int startRank, int endRank)
        {
            return _sortedRanks.Skip(startRank - 1).Take(endRank - startRank + 1);
        }

        /// <summary>
        /// Gets neighboring customers around a specific customer.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <param name="high">The number of higher-ranked neighbors to include. begins at 1.</param>
        /// <param name="low">The number of lower-ranked neighbors to include. begins at 1.</param>
        public IEnumerable<CustomerRank> GetWithNeighbors(long customerId, int high, int low)
        {
            var currentRank = GetRankByCustomerId(customerId);
            if (currentRank == -1)
            {
                return [];
            }
            else
            {
                var startRank = Math.Max(1, currentRank - high);
                var endRank = currentRank + low;
                return GetByRankRange(startRank, endRank);
            }
        }
    }
}