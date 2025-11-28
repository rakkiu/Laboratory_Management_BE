using IAMService.Application.Interfaces;
using IAMService.Application.UseCases.Auth.Login;
using IAMService.Domain.Entities;
using IAMService.Domain.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestProject.LoginTest
{
    /// <summary>
    /// Unit tests for <see cref="LoginHandler"/>.
    /// </summary>
    [TestFixture]
    public class LoginHandlerTests
    {
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<IJwtService> _mockJwtService;
        private Mock<IJwtTokenRepository> _mockJwtTokenRepository;
        private LoginHandler _handler;
        private CancellationToken _cancellationToken;

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockJwtService = new Mock<IJwtService>();
            _mockJwtTokenRepository = new Mock<IJwtTokenRepository>();
            _handler = new LoginHandler(_mockUserRepository.Object, _mockJwtService.Object, _mockJwtTokenRepository.Object);
            _cancellationToken = CancellationToken.None;
        }

        /// <summary>
        /// Valid credentials should generate access and refresh tokens successfully and save both to database.
        /// </summary>
        [Test]
        public async Task Handle_WithValidCredentials_ShouldReturnTokensAndSaveBothToDatabase()
        {
            // Arrange
            var command = new LoginCommand("admin@system.com", "Admin@123");
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Admin@123");

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = "admin@system.com",
                Password = hashedPassword,
                FullName = "System Admin",
                RoleCode = "ADMIN",
                IsActive = true
            };

            var accessToken = "test-access-token";
            var refreshToken = "test-refresh-token";

            _mockUserRepository
                .Setup(x => x.GetByEmailAsync(command.Email, _cancellationToken))
                .ReturnsAsync(user);

            _mockJwtService.Setup(x => x.GenerateAccessToken(user)).Returns(accessToken);
            _mockJwtService.Setup(x => x.GenerateRefreshToken()).Returns(refreshToken);
            _mockJwtService.Setup(x => x.GetAccessTokenExpirationMinutes()).Returns(15);
            _mockJwtService.Setup(x => x.GetRefreshTokenExpirationDays()).Returns(7);

            _mockJwtTokenRepository
                .Setup(x => x.SaveTokenAsync(It.IsAny<JwtToken>(), _cancellationToken))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, _cancellationToken);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.AccessToken, Is.EqualTo(accessToken));
            Assert.That(result.RefreshToken, Is.EqualTo(refreshToken));

            _mockUserRepository.Verify(x => x.GetByEmailAsync(command.Email, _cancellationToken), Times.Once);
            _mockJwtService.Verify(x => x.GenerateAccessToken(user), Times.Once);
            _mockJwtService.Verify(x => x.GenerateRefreshToken(), Times.Once);
            
            // Verify that SaveRefreshTokenAsync is called twice (once for AccessToken, once for RefreshToken)
            _mockJwtTokenRepository.Verify(x => x.SaveTokenAsync(It.IsAny<JwtToken>(), _cancellationToken), Times.Exactly(2));
        }

        /// <summary>
        /// Non-existent email should throw UnauthorizedAccessException.
        /// </summary>
        [Test]
        public void Handle_WithNonExistentEmail_ShouldThrowUnauthorizedException()
        {
            // Arrange
            var command = new LoginCommand("nonexistent@example.com", "Password123");

            _mockUserRepository
                .Setup(x => x.GetByEmailAsync(command.Email, _cancellationToken))
                .ReturnsAsync((User)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(
                async () => await _handler.Handle(command, _cancellationToken));

            Assert.That(ex.Message, Is.EqualTo("Invalid email or password."));
        }

        /// <summary>
        /// Incorrect password should throw UnauthorizedAccessException.
        /// </summary>
        [Test]
        public void Handle_WithIncorrectPassword_ShouldThrowUnauthorizedException()
        {
            // Arrange
            var command = new LoginCommand("admin@system.com", "WrongPassword");
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("CorrectPassword");

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = "admin@system.com",
                Password = hashedPassword,
                FullName = "System Admin",
                RoleCode = "ADMIN",
                IsActive = true
            };

            _mockUserRepository
                .Setup(x => x.GetByEmailAsync(command.Email, _cancellationToken))
                .ReturnsAsync(user);

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(
                async () => await _handler.Handle(command, _cancellationToken));

            Assert.That(ex.Message, Is.EqualTo("Invalid email or password."));
        }

        /// <summary>
        /// Ensure both AccessToken and RefreshToken are correctly saved with valid data and proper TokenType.
        /// </summary>
        [Test]
        public async Task Handle_ShouldSaveBothAccessTokenAndRefreshTokenWithCorrectProperties()
        {
            // Arrange
            var command = new LoginCommand("admin@system.com", "Admin@123");
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Admin@123");
            var userId = Guid.NewGuid();

            var user = new User
            {
                UserId = userId,
                Email = "admin@system.com",
                Password = hashedPassword,
                FullName = "System Admin",
                RoleCode = "ADMIN",
                IsActive = true
            };

            var accessToken = "test-access-token";
            var refreshToken = "test-refresh-token";
            var savedTokens = new List<JwtToken>();

            _mockUserRepository
                .Setup(x => x.GetByEmailAsync(command.Email, _cancellationToken))
                .ReturnsAsync(user);

            _mockJwtService.Setup(x => x.GenerateAccessToken(user)).Returns(accessToken);
            _mockJwtService.Setup(x => x.GenerateRefreshToken()).Returns(refreshToken);
            _mockJwtService.Setup(x => x.GetAccessTokenExpirationMinutes()).Returns(15);
            _mockJwtService.Setup(x => x.GetRefreshTokenExpirationDays()).Returns(7);

            _mockJwtTokenRepository
                .Setup(x => x.SaveTokenAsync(It.IsAny<JwtToken>(), _cancellationToken))
                .Callback<JwtToken, CancellationToken>((token, ct) => savedTokens.Add(token))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, _cancellationToken);

            // Assert
            Assert.That(savedTokens.Count, Is.EqualTo(2), "Should save 2 tokens (AccessToken and RefreshToken)");

            // Verify AccessToken
            var savedAccessToken = savedTokens.FirstOrDefault(t => t.TokenType == "AccessToken");
            Assert.That(savedAccessToken, Is.Not.Null, "AccessToken should be saved");
            Assert.That(savedAccessToken.Token, Is.EqualTo(accessToken));
            Assert.That(savedAccessToken.UserId, Is.EqualTo(userId));
            Assert.That(savedAccessToken.IsRevoked, Is.False);
            Assert.That(savedAccessToken.TokenType, Is.EqualTo("AccessToken"));

            // Verify RefreshToken
            var savedRefreshToken = savedTokens.FirstOrDefault(t => t.TokenType == "RefreshToken");
            Assert.That(savedRefreshToken, Is.Not.Null, "RefreshToken should be saved");
            Assert.That(savedRefreshToken.Token, Is.EqualTo(refreshToken));
            Assert.That(savedRefreshToken.UserId, Is.EqualTo(userId));
            Assert.That(savedRefreshToken.IsRevoked, Is.False);
            Assert.That(savedRefreshToken.TokenType, Is.EqualTo("RefreshToken"));
            Assert.That(savedRefreshToken.ExpiresAt, Is.GreaterThan(DateTime.UtcNow));
        }

        /// <summary>
        /// Inactive user should throw UnauthorizedAccessException.
        /// </summary>
        [Test]
        public void Handle_WithInactiveUser_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var command = new LoginCommand("inactive@system.com", "Admin@123");
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Admin@123");

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = "inactive@system.com",
                Password = hashedPassword,
                FullName = "Inactive User",
                RoleCode = "ADMIN",
                IsActive = false // purposely deactivated
            };

            _mockUserRepository
                .Setup(x => x.GetByEmailAsync(command.Email, _cancellationToken))
                .ReturnsAsync(user);

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(
                async () => await _handler.Handle(command, _cancellationToken));

            Assert.That(ex.Message, Is.EqualTo("User account is deactivated."));
        }

        /// <summary>
        /// Different valid credentials should still return valid tokens.
        /// </summary>
        [TestCase("admin@system.com", "Admin@123")]
        [TestCase("user@test.com", "UserPass123")]
        public async Task Handle_WithDifferentValidCredentials_ShouldReturnTokens(string email, string password)
        {
            // Arrange
            var command = new LoginCommand(email, password);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = email,
                Password = hashedPassword,
                FullName = "Test User",
                RoleCode = "USER",
                IsActive = true
            };

            _mockUserRepository
                .Setup(x => x.GetByEmailAsync(command.Email, _cancellationToken))
                .ReturnsAsync(user);

            _mockJwtService.Setup(x => x.GenerateAccessToken(user)).Returns("access-token");
            _mockJwtService.Setup(x => x.GenerateRefreshToken()).Returns("refresh-token");
            _mockJwtService.Setup(x => x.GetAccessTokenExpirationMinutes()).Returns(15);
            _mockJwtService.Setup(x => x.GetRefreshTokenExpirationDays()).Returns(7);

            _mockJwtTokenRepository
                .Setup(x => x.SaveTokenAsync(It.IsAny<JwtToken>(), _cancellationToken))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, _cancellationToken);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.AccessToken, Is.Not.Null.And.Not.Empty);
            Assert.That(result.RefreshToken, Is.Not.Null.And.Not.Empty);
        }
    }
}
