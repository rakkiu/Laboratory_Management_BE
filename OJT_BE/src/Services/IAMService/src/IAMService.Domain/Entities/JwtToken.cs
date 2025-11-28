
namespace IAMService.Domain.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class JwtToken
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; set; }
        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        public string Token { get; set; } = null!;
        /// <summary>
        /// Gets or sets the expires at.
        /// </summary>
        /// <value>
        /// The expires at.
        /// </value>
        public string TokenType { get; set; }
        /// <summary>
        /// Gets or sets the expires at.
        /// </summary>
        /// <value>
        /// The expires at.
        /// </value>
        public DateTime ExpiresAt { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is revoked.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is revoked; otherwise, <c>false</c>.
        /// </value>
        public bool IsRevoked { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public Guid UserId { get; set; }
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public User User { get; set; } = null!;
    }
}
