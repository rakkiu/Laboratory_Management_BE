namespace IAMService.Application.UseCases.Auth.Logout
{
    public class LogoutCommand : IRequest<bool>
    {
        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        /// <value>
        /// The refresh token.
        /// </value>
        public string refreshToken { get; set; } = string.Empty;
    }
}
