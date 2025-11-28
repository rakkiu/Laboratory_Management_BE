namespace IAMService.Application.UseCases.Auth.ForgotPassword
{
    public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenRepository _tokenRepository;
        private readonly IEmailService _emailSender;

        public ForgotPasswordHandler(
            IUserRepository userRepository,
            IJwtTokenRepository tokenRepository,
            IEmailService emailSender)
        {
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
            _emailSender = emailSender;
        }

        public async Task<Unit> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            // Validate email
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                throw new ArgumentException("Email is missing.");
            }

            var plainEmail = request.Email;

            var user = await _userRepository.GetByEmailWithoutDecryptAsync(plainEmail, cancellationToken);
            if (user == null)
            {
                throw new KeyNotFoundException("Email not found in the system.");
            }

            // Generate reset token
            var resetToken = GenerateResetToken();
            var tokenExpiry = DateTime.UtcNow.AddHours(1);

            // Save token to database
            var passwordResetToken = new JwtToken
            {
                Id = Guid.NewGuid(),
                Token = resetToken,
                TokenType = "PasswordReset",
                ExpiresAt = tokenExpiry,
                IsRevoked = false,
                UserId = user.UserId
            };

            await _tokenRepository.SaveResetTokenAsync(passwordResetToken, cancellationToken);

            // Generate reset link
            var resetLink = GenerateResetLink(resetToken);

            // ✅ Không cần FullName - chỉ dùng "Dear User"
            var emailBody = $@"
                <h2>Password Reset Request</h2>
                <p>Dear User,</p>
                <p>You have requested to reset your password. Please click the link below to reset your password:</p>
                <p><a href='{resetLink}'>Reset Password</a></p>
                <p>This link will expire in 1 hour.</p>
                <p>If you did not request this, please ignore this email.</p>
            ";

            // Gửi email với plainEmail (từ request)
            await _emailSender.SendAsync(plainEmail, "Password Reset Request", emailBody);

            return Unit.Value;
        }

        private string GenerateResetToken()
        {
            // Generate a secure random token
            return Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
        }

        private string GenerateResetLink(string token)
        {
            // Frontend sẽ chỉ dùng token để reset password
            return $"http://localhost:5173/reset-password?token={Uri.EscapeDataString(token)}";
        }
    }
}