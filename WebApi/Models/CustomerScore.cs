using System.Text.Json.Serialization;

namespace ScoreRank.Models
{
    /// <summary>
    /// Represents a customer's score information.
    /// </summary>
    public class CustomerScore
    {
        /// <summary>
        /// The unique identifier of the customer.
        /// </summary>
        [JsonPropertyName("customer_id")]
        public long CustomerId { get; set; }

        /// <summary>
        /// The score of the customer.
        /// </summary>
        [JsonPropertyName("score")]
        public decimal Score { get; set; }
    }
}