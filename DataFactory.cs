using ScoreRank.Models;

namespace ScoreRank.Data
{
    /// <summary>
    /// Factory class for creating data-related instances.
    /// </summary>
    public class DataFactory
    {
        /// <summary>
        /// Creates a new instance of the Leaderboard.
        /// </summary>
        /// <returns></returns>
        public Leaderboard GetLeaderboard()
        {
            return new Leaderboard();
        }
    }
}