using IAMService.Application.Interfaces;
using IAMService.Application.Models.Login;
using IAMService.Application.UseCases.Auth.Token;
using IAMService.Domain.Entities;
using IAMService.Domain.Interfaces;
using Moq;
using NUnit.Framework;

namespace TestIAM.TestProject.Token
{
    [TestFixture]
    public class RefreshAccessTokenHandlerTests
    {
        private Mock<IJwtService> _jwtServiceMock;
        private Mock<IJwtTokenRepository> _jwtTokenRepoMock;
        private Mock<IUserRepository> _userRepository;
        private RefreshAccessTokenHandler _handler;

        [SetUp]
        public void Setup()
        {
            _jwtServiceMock = new Mock<IJwtService>();
            _jwtTokenRepoMock = new Mock<IJwtTokenRepository>();
            _userRepository = new Mock<IUserRepository>();
            _handler = new RefreshAccessTokenHandler(_jwtServiceMock.Object, _jwtTokenRepoMock.Object, _userRepository.Object);
        }

        /// ✅ Test 1: Refresh token hợp lệ → sinh access + refresh token mới
        [Test]
        public async Task Handle_ValidRefreshToken_ReturnsNewTokens()
        {
            var userId = Guid.NewGuid();
            var fakeUser = new IAMService.Domain.Entities.User
            {
                UserId = userId,
                Email = "test@example.com",
                FullName = "Test User",
                RoleCode = "Admin",
                IsActive = true
            };

            var validRefreshToken = new JwtToken
            {
                Id = Guid.NewGuid(),
                Token = "valid-refresh-token",
                TokenType = "RefreshToken",
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsRevoked = false,
                UserId = userId,
                User = fakeUser
            };
            _userRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeUser);
            _jwtTokenRepoMock.Setup(r => r.GetRefreshTokenAsync("valid-refresh-token", It.IsAny<CancellationToken>()))
                .ReturnsAsync(validRefreshToken);
            _jwtTokenRepoMock.Setup(r => r.GetAccessTokenByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((JwtToken?)null);

            _jwtServiceMock.Setup(j => j.GenerateAccessToken(fakeUser)).Returns("new-access-token");
            _jwtServiceMock.Setup(j => j.GenerateRefreshToken()).Returns("new-refresh-token");
            _jwtServiceMock.Setup(j => j.GetAccessTokenExpirationMinutes()).Returns(15);
            _jwtServiceMock.Setup(j => j.GetRefreshTokenExpirationDays()).Returns(7);

            _jwtTokenRepoMock.Setup(r => r.RemoveTokenAsync(It.IsAny<JwtToken>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _jwtTokenRepoMock.Setup(r => r.SaveTokenAsync(It.IsAny<JwtToken>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var command = new RefreshAccessTokenCommand(new RefreshTokenRequest
            {
                RefreshToken = "valid-refresh-token"
            });

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.AccessToken, Is.EqualTo("new-access-token"));
            Assert.That(result.RefreshToken, Is.EqualTo("new-refresh-token"));

            _jwtServiceMock.Verify(j => j.GenerateAccessToken(fakeUser), Times.Once);
            _jwtServiceMock.Verify(j => j.GenerateRefreshToken(), Times.Once);
            _jwtTokenRepoMock.Verify(r => r.RemoveTokenAsync(validRefreshToken, It.IsAny<CancellationToken>()), Times.Once);
            _jwtTokenRepoMock.Verify(r => r.SaveTokenAsync(It.IsAny<JwtToken>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        /// ✅ Test 2: Refresh token hết hạn → xóa tất cả token và trả về empty
        [Test]
        public async Task Handle_ExpiredRefreshToken_RemovesAllTokensAndReturnsEmpty()
        {
            var userId = Guid.NewGuid();
            var expiredRefreshToken = new JwtToken
            {
                Id = Guid.NewGuid(),
                Token = "expired-refresh-token",
                TokenType = "RefreshToken",
                ExpiresAt = DateTime.UtcNow.AddDays(-1),
                IsRevoked = false,
                UserId = userId,
                User = new IAMService.Domain.Entities.User { UserId = userId, Email = "expired@example.com", IsActive = true }
            };

            var oldAccessToken = new JwtToken
            {
                Id = Guid.NewGuid(),
                Token = "old-access-token",
                TokenType = "AccessToken",
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                IsRevoked = false,
                UserId = userId
            };

            _jwtTokenRepoMock.Setup(r => r.GetRefreshTokenAsync("expired-refresh-token", It.IsAny<CancellationToken>()))
                .ReturnsAsync(expiredRefreshToken);
            _jwtTokenRepoMock.Setup(r => r.GetAccessTokenByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(oldAccessToken);
            _jwtTokenRepoMock.Setup(r => r.RemoveTokenAsync(It.IsAny<JwtToken>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var command = new RefreshAccessTokenCommand(new RefreshTokenRequest
            {
                RefreshToken = "expired-refresh-token"
            });

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.That(result.AccessToken, Is.Empty);
            Assert.That(result.RefreshToken, Is.Empty);
            _jwtTokenRepoMock.Verify(r => r.RemoveTokenAsync(expiredRefreshToken, It.IsAny<CancellationToken>()), Times.Once);
            _jwtTokenRepoMock.Verify(r => r.RemoveTokenAsync(oldAccessToken, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// ✅ Test 3: Refresh token không tồn tại
        [Test]
        public async Task Handle_NonExistentRefreshToken_ReturnsEmpty()
        {
            _jwtTokenRepoMock.Setup(r => r.GetRefreshTokenAsync("nonexistent-token", It.IsAny<CancellationToken>()))
                .ReturnsAsync((JwtToken?)null);

            var command = new RefreshAccessTokenCommand(new RefreshTokenRequest
            {
                RefreshToken = "nonexistent-token"
            });

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.That(result.AccessToken, Is.Empty);
            Assert.That(result.RefreshToken, Is.Empty);
        }

        /// ✅ Test 4: Refresh token bị revoke
        [Test]
        public async Task Handle_RevokedRefreshToken_ReturnsEmpty()
        {
            var revokedToken = new JwtToken
            {
                Id = Guid.NewGuid(),
                Token = "revoked-token",
                TokenType = "RefreshToken",
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsRevoked = true,
                UserId = Guid.NewGuid(),
                User = new IAMService.Domain.Entities.User { UserId = Guid.NewGuid(), Email = "revoked@example.com" }
            };

            _jwtTokenRepoMock.Setup(r => r.GetRefreshTokenAsync("revoked-token", It.IsAny<CancellationToken>()))
                .ReturnsAsync(revokedToken);

            var command = new RefreshAccessTokenCommand(new RefreshTokenRequest
            {
                RefreshToken = "revoked-token"
            });

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.That(result.AccessToken, Is.Empty);
            Assert.That(result.RefreshToken, Is.Empty);
        }

        /// ✅ Test 5: User bị deactivate → xóa token và trả về empty
        [Test]
        public async Task Handle_InactiveUser_RemovesAllTokensAndReturnsEmpty()
        {
            var userId = Guid.NewGuid();
            var inactiveUser = new IAMService.Domain.Entities.User { UserId = userId, Email = "inactive@example.com", IsActive = false };

            var refreshToken = new JwtToken
            {
                Id = Guid.NewGuid(),
                Token = "valid-refresh-token",
                TokenType = "RefreshToken",
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsRevoked = false,
                UserId = userId,
                User = inactiveUser
            };

            _jwtTokenRepoMock.Setup(r => r.GetRefreshTokenAsync("valid-refresh-token", It.IsAny<CancellationToken>()))
                .ReturnsAsync(refreshToken);
            _jwtTokenRepoMock.Setup(r => r.GetAccessTokenByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((JwtToken?)null);
            _jwtTokenRepoMock.Setup(r => r.RemoveTokenAsync(It.IsAny<JwtToken>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var command = new RefreshAccessTokenCommand(new RefreshTokenRequest
            {
                RefreshToken = "valid-refresh-token"
            });

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.That(result.AccessToken, Is.Empty);
            Assert.That(result.RefreshToken, Is.Empty);
            _jwtTokenRepoMock.Verify(r => r.RemoveTokenAsync(refreshToken, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// ✅ Test 6: Có gửi kèm AccessToken → xóa đúng token đó
        [Test]
        public async Task Handle_WithAccessToken_RemovesSpecificAccessToken()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var fakeUser = new IAMService.Domain.Entities.User
            {
                UserId = userId,
                Email = "test@example.com",
                FullName = "Test User",
                RoleCode = "Admin",
                IsActive = true
            };

            var refreshToken = new JwtToken
            {
                Id = Guid.NewGuid(),
                Token = "valid-refresh-token",
                TokenType = "RefreshToken",
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsRevoked = false,
                UserId = userId,
                User = fakeUser
            };

            var oldAccessToken = new JwtToken
            {
                Id = Guid.NewGuid(),
                Token = "old-access-token",
                TokenType = "AccessToken",
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                IsRevoked = false,
                UserId = userId
            };

            // Mock repositories and services
            _userRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeUser);

            _jwtTokenRepoMock.Setup(r => r.GetRefreshTokenAsync("valid-refresh-token", It.IsAny<CancellationToken>()))
                .ReturnsAsync(refreshToken);

            _jwtTokenRepoMock.Setup(r => r.GetByTokenAsync("old-access-token"))
                .ReturnsAsync(oldAccessToken);

            _jwtTokenRepoMock.Setup(r => r.RemoveTokenAsync(It.IsAny<JwtToken>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _jwtTokenRepoMock.Setup(r => r.SaveTokenAsync(It.IsAny<JwtToken>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _jwtServiceMock.Setup(j => j.GenerateAccessToken(It.IsAny<IAMService.Domain.Entities.User>()))
                .Returns("new-access-token");

            _jwtServiceMock.Setup(j => j.GenerateRefreshToken())
                .Returns("new-refresh-token");

            _jwtServiceMock.Setup(j => j.GetAccessTokenExpirationMinutes())
                .Returns(15);

            _jwtServiceMock.Setup(j => j.GetRefreshTokenExpirationDays())
                .Returns(7);

            var command = new RefreshAccessTokenCommand(new RefreshTokenRequest
            {
                RefreshToken = "valid-refresh-token",
                AccessToken = "old-access-token"
            });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.AccessToken, Is.EqualTo("new-access-token"));
            Assert.That(result.RefreshToken, Is.EqualTo("new-refresh-token"));

            _jwtTokenRepoMock.Verify(r => r.GetByTokenAsync("old-access-token"), Times.Once);
            _jwtTokenRepoMock.Verify(r => r.RemoveTokenAsync(oldAccessToken, It.IsAny<CancellationToken>()), Times.Once);
            _jwtTokenRepoMock.Verify(r => r.RemoveTokenAsync(refreshToken, It.IsAny<CancellationToken>()), Times.Once);
            _jwtTokenRepoMock.Verify(r => r.SaveTokenAsync(It.IsAny<JwtToken>(), It.IsAny<CancellationToken>()), Times.Exactly(2)); // 2 tokens saved
        }


        /// ✅ Test 7: Input rỗng/null → trả về empty
        [Test]
        public async Task Handle_EmptyRefreshToken_ReturnsEmpty()
        {
            var command = new RefreshAccessTokenCommand(new RefreshTokenRequest
            {
                RefreshToken = string.Empty
            });

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.That(result.AccessToken, Is.Empty);
            Assert.That(result.RefreshToken, Is.Empty);
            _jwtTokenRepoMock.Verify(r => r.GetRefreshTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
