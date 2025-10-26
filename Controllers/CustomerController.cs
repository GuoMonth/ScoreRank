using Microsoft.AspNetCore.Mvc;
using ScoreRank.Models;
using ScoreRank.Services;

namespace ScoreRank.Controllers
{
    /// <summary>
    /// Controller for managing customer-related operations.
    /// </summary>
    [ApiController]
    [Route("api/customer")]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerController"/> class.
        /// </summary>
        public CustomerController(CustomerService customerService, ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }


        /// <summary>
        /// Updates the score for a customer.
        /// Note, If the customer does not exist, they are created with initial score of 0 before applying the update.
        /// If add new customer, or update existing customer score, the rank of all customers in leaderboard should be updated accordingly.
        /// </summary>
        /// <param name="customerId">The customer ID (positive int64).</param>
        /// <param name="score">The score adjustment (decimal in range [-1000, +1000]). Positive increases, negative decreases.</param>
        /// <returns>An ApiResponse containing the current score after update.</returns>
        [HttpPost("{customerId}/score/{score}")]
        public ActionResult<ApiResponse<object>> UpdateScore(long customerId, decimal score)
        {
            var errors = new List<string>();

            // Validate customerId parameter
            if (customerId <= 0)
            {
                errors.Add("Customer ID must be a positive integer.");
            }

            // Validate score parameter
            if (score < -1000m || score > 1000m)
            {
                errors.Add("Score must be between -1000 and 1000.");
            }

            // Return validation errors if any
            if (errors.Any())
            {
                _logger.LogWarning("customerId: {customerId}, Validation errors: {Errors}", customerId, string.Join(", ", errors));
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid parameters.",
                    Errors = errors
                });
            }

            _customerService.AddOrUpdateCustomerScore(customerId, score);

            // Implementation not required for this task
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = $"Customer {customerId} score updated successfully"
            });
        }
    }
}

