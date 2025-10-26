using System.Collections.Concurrent;

namespace ScoreRank.Models
{
    /// <summary>
    /// Custom comparer to sort by Score descending, then by CustomerId ascending.
    /// </summary>
    public class Comparer : IComparer<CustomerScore>
    {
        /// <summary>
        /// Compares two CustomerScore objects.
        /// </summary>
        /// <param name="x">The first CustomerScore object to compare.</param>
        /// <param name="y">The second CustomerScore object to compare.</param>
        /// <returns>A negative integer, zero, or a positive integer as the first argument is less than, equal to, or greater than the second.</returns>
        public int Compare(CustomerScore? x, CustomerScore? y)
        {
            // Compare by Score descending
            int result = y?.Score.CompareTo(x?.Score) ?? 0;
            if (result != 0)
            {
                return result;
            }
            else
            {
                // If Scores are equal, compare by CustomerId ascending
                return x?.CustomerId.CompareTo(y?.CustomerId) ?? 0;
            }
        }
    }

    /// <summary>
    /// Represents the leaderboard containing customer rankings.
    /// </summary>
    public class Leaderboard
    {
        /// <summary>
        /// Maximum allowed data size in memory.
        /// Default is 1 million entries.
        /// </summary>
        private readonly int MaxDataSize = 1000000;

        private SortedSet<CustomerScore> _sortedScore;

        private ConcurrentDictionary<long, CustomerScore> _customerDict;

        /// <summary>
        /// Initializes a new instance of the <see cref="Leaderboard"/> class.
        /// </summary>
        public Leaderboard()
        {
            _sortedScore = new SortedSet<CustomerScore>(new Comparer());
            _customerDict = new ConcurrentDictionary<long, CustomerScore>();
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
                // slow but simple way to get rank. need to optimize if performance issue found
                return _sortedScore.ToList().IndexOf(customerRank) + 1; // Rank starts at 1
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
            if (_customerDict.TryGetValue(customerId, out var customerScore))
            {
                // Update existing customer
                _sortedScore.Remove(customerScore);
                customerScore.Score += scoreDelta;
            }
            else
            {
                // For safety, check max data size
                if (_customerDict.Count >= MaxDataSize)
                {
                    throw new InvalidOperationException("Leaderboard has reached its maximum data size. Cannot add new customer.");
                }

                // Add new customer
                customerScore = new CustomerScore
                {
                    CustomerId = customerId,
                    Score = scoreDelta
                };
                _customerDict[customerId] = customerScore;
            }

            var addResult = _sortedScore.Add(customerScore);

            if (!addResult)
            {
                throw new InvalidOperationException($"Failed to add or update customer rank in the leaderboard. customerId: {customerId}");
            }
        }

        /// <summary>
        /// Gets customers within the specified rank range.
        /// </summary>
        /// <param name="startRank">The starting rank (inclusive). begins at 1.</param>
        /// <param name="endRank">The ending rank (inclusive). begins at 1.</param>
        public IEnumerable<CustomerRank> GetByRankRange(int startRank, int endRank)
        {
            var res = _sortedScore.Skip(startRank - 1).Take(endRank - startRank + 1);

            // calculate rank for each customer
            int rank = startRank;
            // initialize result list with capacity
            var result = new List<CustomerRank>(res.Count());
            foreach (var customer in res)
            {
                result.Add(new CustomerRank
                {
                    customerScore = customer,
                    Rank = rank++
                });
            }

            return result;
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