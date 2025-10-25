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
        /// The rank position of the customer in the leaderboard.
        /// </summary>
        [JsonPropertyName("rank")]
        public int Rank { get; set; }
    }
}