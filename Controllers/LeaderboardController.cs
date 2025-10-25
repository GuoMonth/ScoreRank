using Microsoft.AspNetCore.Mvc;
using ScoreRank.Models;

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

        /// <summary>
        /// Gets customers by their rank range.
        /// </summary>
        /// <param name="start">The starting rank position (inclusive, if exists).</param>
        /// <param name="end">The ending rank position (inclusive, if exists).</param>
        /// <returns>An ApiResponse containing the list of customers with their rank and score information.</returns>
        [HttpGet]
        public ActionResult<ApiResponse<List<CustomerRank>>> GetCustomersByRank(int? start, int? end)
        {
            var errors = new List<string>();

            if (start == null || end == null)
            {
                errors.Add("Both start and end rank parameters are required.");
            }

            // Validate start parameter
            if (start.HasValue && start.Value < 1)
            {
                errors.Add("Start rank must be a positive integer.");
            }

            // Validate end parameter
            if (end.HasValue && end.Value < 1)
            {
                errors.Add("End rank must be a positive integer.");
            }

            // Validate order when both parameters are provided
            if (start.HasValue && end.HasValue && start.Value > end.Value)
            {
                errors.Add("Start rank cannot be greater than end rank.");
            }

            // Return validation errors if any
            if (errors.Any())
            {
                return BadRequest(new ApiResponse<List<CustomerRank>>
                {
                    Success = false,
                    Message = "Invalid parameters.",
                    Errors = errors
                });
            }

            // Implementation not required for this task
            return Ok(new ApiResponse<List<CustomerRank>>
            {
                Success = true,
                Data = new List<CustomerRank>(),
                Message = "Customers retrieved successfully"
            });
        }

        /// <summary>
        /// Gets a customer and their neighboring customers by rank.
        /// </summary>
        /// <param name="customerId">The unique identifier of the customer to look up.</param>
        /// <param name="high">The number of higher-ranked neighbors to include (default: 0).</param>
        /// <param name="low">The number of lower-ranked neighbors to include (default: 0).</param>
        /// <returns>An ApiResponse containing the customer and their neighbors with rank and score information.</returns>
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
                return BadRequest(new ApiResponse<List<CustomerRank>>
                {
                    Success = false,
                    Message = "Invalid parameters.",
                    Errors = errors
                });
            }


            // Implementation not required for this task
            return Ok(new ApiResponse<List<CustomerRank>>
            {
                Success = true,
                Data = new List<CustomerRank>(),
                Message = "Customer and neighbors retrieved successfully"
            });
        }
    }
}