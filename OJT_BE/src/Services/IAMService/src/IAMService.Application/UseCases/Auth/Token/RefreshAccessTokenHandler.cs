using IAMService.Application.Interfaces;
using IAMService.Domain.Entities;
using IAMService.Domain.Interfaces;

namespace IAMService.Application.UseCases.Auth.Token
{
    /// <summary>
    /// Handles the <see cref="RefreshAccessTokenCommand" /> request.
    /// Responsible for validating the provided refresh token and generating
    /// a new access token if the refresh token is valid and active.
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;IAMService.Application.UseCases.Auth.Token.RefreshAccessTokenCommand, IAMService.Application.Models.Login.LoginResultDto&gt;" />
    public class RefreshAccessTokenHandler : IRequestHandler<RefreshAccessTokenCommand, LoginResultDto>
    {
        /// <summary>
        /// The JWT service
        /// </summary>
        private readonly IJwtService _jwtService;
        /// <summary>
        /// The JWT token repository
        /// </summary>
        private readonly IJwtTokenRepository _jwtTokenRepository;
        /// <summary>
        /// The user repository
        /// </summary>
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshAccessTokenHandler"/> class.
        /// </summary>
        /// <param name="jwtService">The JWT service.</param>
        /// <param name="jwtTokenRepository">The JWT token repository.</param>
        /// <param name="userRepository">The user repository.</param>
        public RefreshAccessTokenHandler(IJwtService jwtService, IJwtTokenRepository jwtTokenRepository, IUserRepository userRepository)
        {
            _jwtService = jwtService;
            _jwtTokenRepository = jwtTokenRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Processes the refresh token request and returns new tokens if valid.
        /// Logic:
        /// - If RefreshToken is valid and not expired → Generate new tokens, delete old tokens
        /// - If RefreshToken is expired → Delete ALL tokens of user (force re-login)
        /// - If RefreshToken is revoked or not found → Return empty result
        /// </summary>
        /// <param name="request">The refresh token command containing the client's tokens.</param>
        /// <param name="ct">Cancellation token for async operation.</param>
        /// <returns>
        /// Returns new AccessToken and RefreshToken if successful; otherwise, empty LoginResultDto.
        /// </returns>
        public async Task<LoginResultDto> Handle(RefreshAccessTokenCommand request, CancellationToken ct)
        {
            // 1️⃣ Validate input
            if (string.IsNullOrWhiteSpace(request.token.RefreshToken))
            {
                return new LoginResultDto(); // Return empty if no refresh token provided
            }

            // 2️⃣ Tìm RefreshToken trong DB
            var storedRefreshToken = await _jwtTokenRepository.GetRefreshTokenAsync(request.token.RefreshToken, ct);

            // 3️⃣ Kiểm tra RefreshToken có tồn tại và hợp lệ không
            if (storedRefreshToken == null || storedRefreshToken.IsRevoked)
            {
                // Token không tồn tại hoặc đã bị thu hồi
                return new LoginResultDto();
            }

            var userId = storedRefreshToken.UserId;

            // 4️⃣ Kiểm tra RefreshToken đã hết hạn chưa
            if (storedRefreshToken.ExpiresAt < DateTime.UtcNow)
            {
                // ❌ RefreshToken HẾT HẠN → XÓA TẤT CẢ TOKENS CỦA USER (Force re-login)
                
                // Xóa RefreshToken hết hạn
                await _jwtTokenRepository.RemoveTokenAsync(storedRefreshToken, ct);

                // Xóa tất cả AccessToken của user
                var allAccessTokens = await _jwtTokenRepository.GetAccessTokenByUserIdAsync(userId, ct);
                if (allAccessTokens != null)
                {
                    await _jwtTokenRepository.RemoveTokenAsync(allAccessTokens, ct);
                }

                // Trả về empty để báo client phải login lại
                return new LoginResultDto();
            }

            // 5️⃣ RefreshToken CÒN HẠN → Kiểm tra user
            var user =  await _userRepository.GetByIdAsync(userId);

            if (user == null || !user.IsActive)
            {
                // User không tồn tại hoặc đã bị deactivate → Xóa tất cả tokens
                await _jwtTokenRepository.RemoveTokenAsync(storedRefreshToken, ct);
                
                var allAccessTokens = await _jwtTokenRepository.GetAccessTokenByUserIdAsync(userId, ct);
                if (allAccessTokens != null)
                {
                    await _jwtTokenRepository.RemoveTokenAsync(allAccessTokens, ct);
                }
                
                return new LoginResultDto();
            }

            // 6️⃣ Generate NEW tokens
            var newAccessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            // 7️⃣ Xóa OLD tokens
            // Option A: Nếu frontend gửi kèm AccessToken → Xóa chính xác token đó
            if (!string.IsNullOrWhiteSpace(request.token.AccessToken))
            {
                var specificAccessToken = await _jwtTokenRepository.GetByTokenAsync(request.token.AccessToken);
                if (specificAccessToken != null && specificAccessToken.UserId == userId)
                {
                    await _jwtTokenRepository.RemoveTokenAsync(specificAccessToken, ct);
                }
            }
            else
            {
                // Option B: Không có AccessToken → Xóa AccessToken gần nhất của user
                var oldAccessToken = await _jwtTokenRepository.GetAccessTokenByUserIdAsync(userId, ct);
                if (oldAccessToken != null)
                {
                    await _jwtTokenRepository.RemoveTokenAsync(oldAccessToken, ct);
                }
            }

            // Xóa OLD RefreshToken
            await _jwtTokenRepository.RemoveTokenAsync(storedRefreshToken, ct);

            // 8️⃣ Lưu NEW AccessToken vào database
            var newAccessTokenEntity = new JwtToken
            {
                Token = newAccessToken,
                TokenType = "AccessToken",
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtService.GetAccessTokenExpirationMinutes()),
                IsRevoked = false,
                UserId = user.UserId
            };
            await _jwtTokenRepository.SaveTokenAsync(newAccessTokenEntity, ct);

            // 9️⃣ Lưu NEW RefreshToken vào database
            var newRefreshTokenEntity = new JwtToken
            {
                Token = newRefreshToken,
                TokenType = "RefreshToken",
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtService.GetRefreshTokenExpirationDays()),
                IsRevoked = false,
                UserId = user.UserId
            };
            await _jwtTokenRepository.SaveTokenAsync(newRefreshTokenEntity, ct);
            _userRepository.Update(user);

            // ✅ Trả về tokens mới cho client
            return new LoginResultDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }
    }
}
