using MediatR;

namespace IAMService.Application.UseCases.Auth.Token
{
    /// <summary>
    /// Represents a request to refresh an existing access token
    /// using a valid refresh token.
    /// </summary>
    /// <param name="RefreshToken">The refresh token provided by the client.</param>
    /// <param name="AccessToken">The current access token (optional - used to identify and revoke specific token).</param>
    public record RefreshAccessTokenCommand(RefreshTokenRequest token) : IRequest<LoginResultDto>;
}
