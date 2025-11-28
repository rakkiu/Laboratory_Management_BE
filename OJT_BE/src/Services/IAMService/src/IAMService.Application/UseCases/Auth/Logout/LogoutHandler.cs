using IAMService.Domain.Interfaces;

namespace IAMService.Application.UseCases.Auth.Logout
{
    /// <summary>
    /// Handles user logout by revoking refresh tokens.
    /// </summary>
    public class LogoutHandler : IRequestHandler<LogoutCommand, bool>
    {
        private readonly IJwtTokenRepository _jwtTokenRepository;

        public LogoutHandler(IJwtTokenRepository jwtTokenRepository)
        {
            _jwtTokenRepository = jwtTokenRepository;
        }

        public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.refreshToken))
                throw new ArgumentException("Refresh token is missing.");

            // Find the refresh token in DB
            var refreshToken = await _jwtTokenRepository.GetByTokenAsync(request.refreshToken);
            if (refreshToken == null || refreshToken.IsRevoked || refreshToken.ExpiresAt <= DateTime.UtcNow)
                throw new InvalidOperationException("Invalid or expired token.");

            // refresh token as revoked
            refreshToken.IsRevoked = true;

            await _jwtTokenRepository.UpdateAsync(refreshToken, cancellationToken);

            //revoke all active access tokens of this user (optional)
            var accessToken = await _jwtTokenRepository
            .GetAccessTokenByUserIdAsync(refreshToken.UserId);

            if (accessToken != null)
            {
                accessToken.IsRevoked = true;
                await _jwtTokenRepository.UpdateAsync(accessToken, cancellationToken);
            }
             await _jwtTokenRepository.SaveChangeAsync();
            return true;
        }
    }
}