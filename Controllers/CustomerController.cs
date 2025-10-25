using Microsoft.AspNetCore.Mvc;

namespace ScoreRank.Controllers
{
    /// <summary>
    /// Controller for managing customer-related operations.
    /// </summary>
    [ApiController]
    [Route("api/customer")]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public CustomerController(ILogger<CustomerController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Updates the score for a customer.
        /// Note, If the customer does not exist, they are created with initial score of 0 before applying the update.
        /// </summary>
        /// <param name="customerId">The customer ID (positive int64).</param>
        /// <param name="score">The score adjustment (decimal in range [-1000, +1000]). Positive increases, negative decreases.</param>
        /// <returns>The current score after update.</returns>
        [HttpPost("{customerId}/score/{score}")]
        public ActionResult<decimal> UpdateScore(long customerId, decimal score)
        {
            // Implementation not required for this task
            return Ok(0m);
        }
    }
}

