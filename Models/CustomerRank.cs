using System.Text.Json.Serialization;

namespace ScoreRank.Models
{
    /// <summary>
    /// Represents a customer's ranking information.
    /// </summary>
    public class CustomerRank
    {
        /// <summary>
        /// The unique identifier of the customer.
        /// </summary>
        [JsonPropertyName("customer_id")]
        public long CustomerId { get; set; }

        /// <summary>
        /// The current score of the customer.
        /// </summary>
        [JsonPropertyName("score")]
        public decimal Score { get; set; }

        /// <summary>
        /// The rank position of the customer in the leaderboard, depending on their score.
        /// If two customers have the same score, lower customer ID gets the better rank.
        /// Note: Rank starts at 1 for the highest score. Rank 2 is the second highest, and so on.
        /// Note: Rank is always a positive integer. and never same rank for different customers.
        /// </summary>
        [JsonPropertyName("rank")]
        public long? Rank { get; set; }
    }

    /// <summary>
    /// Custom comparer to sort by Score descending, then by CustomerId ascending.
    /// </summary>
    public class Comparer : IComparer<CustomerRank>
    {
        /// <summary>
        /// Compares two CustomerRank objects.
        /// </summary>
        /// <param name="x">The first CustomerRank object to compare.</param>
        /// <param name="y">The second CustomerRank object to compare.</param>
        /// <returns>A negative integer, zero, or a positive integer as the first argument is less than, equal to, or greater than the second.</returns>
        public int Compare(CustomerRank? x, CustomerRank? y)
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
}