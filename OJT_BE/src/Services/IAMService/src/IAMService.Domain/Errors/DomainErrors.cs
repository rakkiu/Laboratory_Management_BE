namespace IAMService.Domain.Errors
{
    /// <summary>
    /// Represents a specific domain error.
    /// Renamed to DomainError to avoid conflicts with other 'Error' classes.
    /// </summary>
    public class DomainError
    {
        /// <summary>
        /// Gets the error code.
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Gets the error description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainError"/> class.
        /// </summary>
        /// <param name="code">The error code.</param>
        /// <param name="description">The error description.</param>
        public DomainError(string code, string description)
        {
            Code = code;
            Description = description;
        }

        /// <summary>
        /// Represents a non-error state.
        /// </summary>
        public static readonly DomainError None = new(string.Empty, string.Empty);
    }

    /// <summary>
    /// Contains domain-specific errors.
    /// </summary>
    public static class DomainErrors
    {
        /// <summary>
        /// Contains errors related to the User domain.
        /// </summary>
        public static class UserError
        {
            /// <summary>
            /// Gets the error for when a user is not found.
            /// </summary>
            public static readonly DomainError NotFound = new(
                "User.NotFound",
                "The user with the specified ID was not found.");


            public static readonly DomainError AlreadyLocked = new(
                "User.AlreadyLocked",
                "This user account is already locked (deactivated).");


            public static readonly DomainError AlreadyUnlocked = new(
                "User.AlreadyUnlocked",
                "This user account is already active (unlocked).");

 
        }
    }
}