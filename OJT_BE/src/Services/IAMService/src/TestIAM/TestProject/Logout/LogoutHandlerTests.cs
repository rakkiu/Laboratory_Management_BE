using IAMService.Application.UseCases.Auth.Logout;
using IAMService.Domain.Entities;
using IAMService.Domain.Interfaces;
using Moq;

namespace IAMService.Tests.UseCases.Logout
{
    /// <summary>
    /// Unit tests cho lớp <see cref="LogoutHandler"/>.
    /// Mục tiêu là đảm bảo hành vi logout hoạt động đúng cho tất cả trường hợp:
    /// - Token trống hoặc null
    /// - Token không tồn tại trong database
    /// - Token đã bị thu hồi
    /// - Token hợp lệ (được xóa thành công)
    /// </summary>
    [TestFixture]
    public class LogoutHandlerTests
    {
        private Mock<IJwtTokenRepository> _jwtTokenRepositoryMock;
        private LogoutHandler _handler;

        [SetUp]
        public void Setup()
        {
            _jwtTokenRepositoryMock = new Mock<IJwtTokenRepository>();
            _handler = new LogoutHandler(_jwtTokenRepositoryMock.Object);
        }

        [Test]
        public void Handle_ShouldThrowArgumentException_WhenTokenIsMissing()
        {
            // Arrange
            var command = new LogoutCommand { refreshToken = "" };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _handler.Handle(command, CancellationToken.None));

            Assert.That(ex!.Message, Is.EqualTo("Refresh token is missing."));
        }

        [Test]
        public void Handle_ShouldThrowInvalidOperationException_WhenTokenNotFound()
        {
            // Arrange
            var command = new LogoutCommand { refreshToken = "not-found-token" };

            _jwtTokenRepositoryMock
                .Setup(r => r.GetByTokenAsync(command.refreshToken))
                .ReturnsAsync((JwtToken?)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _handler.Handle(command, CancellationToken.None));

            Assert.That(ex!.Message, Is.EqualTo("Invalid or expired token."));
        }

        [Test]
        public void Handle_ShouldThrowInvalidOperationException_WhenTokenIsAlreadyRevoked()
        {
            // Arrange
            var command = new LogoutCommand { refreshToken = "revoked-token" };
            var revokedToken = new JwtToken { Token = command.refreshToken, IsRevoked = true };

            _jwtTokenRepositoryMock
                .Setup(r => r.GetByTokenAsync(command.refreshToken))
                .ReturnsAsync(revokedToken);

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _handler.Handle(command, CancellationToken.None));

            Assert.That(ex!.Message, Is.EqualTo("Invalid or expired token."));
        }

        [Test]
        public async Task Handle_ShouldRevokeTokenAndReturnTrue_WhenTokenIsValid()
        {
            // Arrange
            var command = new LogoutCommand { refreshToken = "valid-token" };
            var validToken = new JwtToken { Token = command.refreshToken, IsRevoked = false , ExpiresAt = DateTime.UtcNow.AddMinutes(30) };

            _jwtTokenRepositoryMock
                .Setup(r => r.GetByTokenAsync(command.refreshToken))
                .ReturnsAsync(validToken);

            _jwtTokenRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<JwtToken>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(validToken.IsRevoked, Is.True);

            _jwtTokenRepositoryMock.Verify(r => r.GetByTokenAsync("valid-token"), Times.Once);
            _jwtTokenRepositoryMock.Verify(r => r.UpdateAsync(It.Is<JwtToken>(t => t.IsRevoked), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldNotCallUpdate_WhenTokenInvalid()
        {
            // Arrange
            var command = new LogoutCommand { refreshToken = "invalid-token" };

            _jwtTokenRepositoryMock
                .Setup(r => r.GetByTokenAsync(command.refreshToken))
                .ReturnsAsync((JwtToken?)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _handler.Handle(command, CancellationToken.None));

            Assert.That(ex!.Message, Is.EqualTo("Invalid or expired token."));
            _jwtTokenRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<JwtToken>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
