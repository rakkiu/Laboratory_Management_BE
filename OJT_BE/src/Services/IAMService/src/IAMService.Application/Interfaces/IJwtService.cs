namespace IAMService.Application.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Generates the access token.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        string GenerateAccessToken(User user);
        
        /// <summary>
        /// Generates the refresh token.
        /// </summary>
        /// <returns></returns>
        string GenerateRefreshToken();
        
        /// <summary>
        /// Validates the token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="validateLifetime">if set to <c>true</c> [validate lifetime].</param>
        /// <returns></returns>
        ClaimsPrincipal? ValidateToken(string token, bool validateLifetime = true);
        
        /// <summary>
        /// Gets the access token expiration in minutes from settings.
        /// </summary>
        /// <returns>Access token expiration in minutes</returns>
        int GetAccessTokenExpirationMinutes();
        
        /// <summary>
        /// Gets the refresh token expiration in days from settings.
        /// </summary>
        /// <returns>Refresh token expiration in days</returns>
        int GetRefreshTokenExpirationDays();
    }
}
