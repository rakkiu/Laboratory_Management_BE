namespace IAMService.Application.Common
{
    /// <summary>
    /// Represents the outcome of an operation.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Gets a value indicating whether the operation failed.
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// Gets the error if the operation failed.
        /// </summary>
        public DomainError Error { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class.
        /// </summary>
        /// <param name="isSuccess">If set to <c>true</c> [is success].</param>
        /// <param name="error">The error.</param>
        protected Result(bool isSuccess, DomainError error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        /// <summary>
        /// Creates a success result.
        /// </summary>
        /// <returns>A successful <see cref="Result"/>.</returns>
        public static Result Success() => new(true, DomainError.None);

        /// <summary>
        /// Creates a failure result with the specified error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns>A failure <see cref="Result"/>.</returns>
        public static Result Failure(DomainError error) => new(false, error);
    }
}