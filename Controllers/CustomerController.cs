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

    }
}

