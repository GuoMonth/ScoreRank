namespace ScoreRank.Models
{
    /// <summary>
    /// Common API response model for all endpoints.
    /// </summary>
    /// <typeparam name="T">The type of the data payload.</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indicates whether the operation was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Optional message providing additional information about the response.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// The data payload of the response. Null when Success is false.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Collection of error messages. Only populated when Success is false.
        /// </summary>
        public List<string>? Errors { get; set; }
    }
}