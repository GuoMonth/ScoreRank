using Microsoft.AspNetCore.Mvc;

namespace ScoreRank.Controllers
{
    /// <summary>
    /// Controller for managing leaderboard-related operations.
    /// </summary>
    [ApiController]
    [Route("api/leaderboard")]
    public class LeaderboardController : ControllerBase
    {
        private readonly ILogger<LeaderboardController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LeaderboardController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public LeaderboardController(ILogger<LeaderboardController> logger)
        {
            _logger = logger;
        }

    }
}