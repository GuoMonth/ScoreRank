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
        public long CustomerId { get; set; }

        /// <summary>
        /// The current score of the customer.
        /// </summary>
        public decimal Score { get; set; }

        /// <summary>
        /// The rank position of the customer in the leaderboard.
        /// </summary>
        public int Rank { get; set; }
    }
}