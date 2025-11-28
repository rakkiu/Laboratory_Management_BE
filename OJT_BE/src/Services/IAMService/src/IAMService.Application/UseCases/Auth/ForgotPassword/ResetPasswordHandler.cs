using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using IAMService.Domain.Interfaces;
using BCrypt.Net;

namespace IAMService.Application.UseCases.Auth.ForgotPassword
{
    public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenRepository _tokenRepository;

        public ResetPasswordHandler(
            IUserRepository userRepository,
            IJwtTokenRepository tokenRepository)
        {
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
        }

        public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Token))
            {
                throw new ArgumentException("Reset token is required.");
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                throw new ArgumentException("New password is required.");
            }

            // Find and validate reset token
            var resetToken = await _tokenRepository.GetByTokenAsync(request.Token);
            if (resetToken == null || resetToken.TokenType != "PasswordReset")
            {
                throw new UnauthorizedAccessException("Invalid reset token.");
            }

            // Check if token is revoked
            if (resetToken.IsRevoked)
            {
                throw new UnauthorizedAccessException("Reset token has been revoked.");
            }

            // Check if token is expired - Normalize both to UTC for comparison
            var tokenExpiresAt = DateTime.SpecifyKind(resetToken.ExpiresAt, DateTimeKind.Utc);
            if (tokenExpiresAt < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Reset token has expired.");
            }

            var user = await _userRepository.GetByIdWithoutDecryptAsync(resetToken.UserId, cancellationToken);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            
            _userRepository.UpdatePasswordOnly(user);

            // Revoke the reset token
            resetToken.IsRevoked = true;
            _tokenRepository.Update(resetToken);

            // Save all changes in one transaction
            await _userRepository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
