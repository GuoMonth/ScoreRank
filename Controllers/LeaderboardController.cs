using Microsoft.AspNetCore.Mvc;
using ScoreRank.Models;
using ScoreRank.Services;

namespace ScoreRank.Controllers
{
    /// <summary>
    /// Controller for managing leaderboard-related operations.
    /// </summary>
    [ApiController]
    [Route("api/leaderboard")]
    public class LeaderboardController : ControllerBase
    {
        private readonly LeaderboardService _leaderboardService;
        private readonly ILogger<LeaderboardController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LeaderboardController"/> class.
        /// </summary>
        public LeaderboardController(LeaderboardService leaderboardService, ILogger<LeaderboardController> logger)
        {
            _leaderboardService = leaderboardService;
            _logger = logger;
        }

        /// <summary>
        /// Gets customers by their rank range.
        /// </summary>
        /// <param name="start">The starting rank position (inclusive, if exists).</param>
        /// <param name="end">The ending rank position (inclusive, if exists).</param>
        /// <returns>An ApiResponse containing the list of customers with their rank and score information. The list will not be sorted.</returns>
        [HttpGet]
        public ActionResult<ApiResponse<List<CustomerRank>>> GetCustomersByRank(int start, int end)
        {
            var errors = new List<string>();

            // Validate start parameter
            if (start < 1)
            {
                errors.Add("Start rank must be a positive integer.");
            }

            // Validate end parameter
            if (end < 1)
            {
                errors.Add("End rank must be a positive integer.");
            }

            // Validate order when both parameters are provided
            if (start > end)
            {
                errors.Add("Start rank cannot be greater than end rank.");
            }

            // Return validation errors if any
            if (errors.Any())
            {
                _logger.LogWarning("GetCustomersByRank validation errors: {Errors}", string.Join(", ", errors));
                return BadRequest(new ApiResponse<List<CustomerRank>>
                {
                    Success = false,
                    Message = "Invalid parameters.",
                    Errors = errors
                });
            }

            var customers = _leaderboardService.GetCustomersByRankRange(start, end);
            return Ok(new ApiResponse<List<CustomerRank>>
            {
                Success = true,
                Data = customers.ToList(),
                Message = "Customers retrieved successfully"
            });
        }

        /// <summary>
        /// Gets a customer and their neighboring customers by rank.
        /// </summary>
        /// <param name="customerId">The unique identifier of the customer to look up.</param>
        /// <param name="high">The number of higher-ranked neighbors to include (default: 0).</param>
        /// <param name="low">The number of lower-ranked neighbors to include (default: 0).</param>
        /// <returns>An ApiResponse containing the customer and their neighbors with rank and score information. The list will not be sorted.</returns>
        [HttpGet("{customerId}")]
        public ActionResult<ApiResponse<List<CustomerRank>>> GetCustomerWithNeighbors(long customerId, int? high = 0, int? low = 0)
        {
            var errors = new List<string>();

            // Validate customerId parameter
            if (customerId <= 0)
            {
                errors.Add("Customer ID must be a positive integer.");
            }

            // Validate high parameter
            if (high.HasValue && high.Value < 0)
            {
                errors.Add("High parameter must be non-negative.");
            }

            // Validate low parameter
            if (low.HasValue && low.Value < 0)
            {
                errors.Add("Low parameter must be non-negative.");
            }

            // Return validation errors if any
            if (errors.Any())
            {
                _logger.LogWarning("customerId: {customerId}, Validation errors: {Errors}", customerId, string.Join(", ", errors));
                return BadRequest(new ApiResponse<List<CustomerRank>>
                {
                    Success = false,
                    Message = "Invalid parameters.",
                    Errors = errors
                });
            }

            var customers = _leaderboardService.GetWithNeighbors(customerId, high ?? 0, low ?? 0);

            // Implementation not required for this task
            return Ok(new ApiResponse<List<CustomerRank>>
            {
                Success = true,
                Data = customers.ToList(),
                Message = "Customer and neighbors retrieved successfully"
            });
        }
    }
}