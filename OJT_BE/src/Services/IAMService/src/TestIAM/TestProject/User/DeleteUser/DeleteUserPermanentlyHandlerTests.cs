using IAMService.Application.UseCases.Users.Commands.DeleteUserPermanently;
using IAMService.Domain.Entities;
using IAMService.Domain.Errors;
using IAMService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TestProject.UserTests
{
    /// <summary>
    /// Unit tests for <see cref="DeleteUserPermanentlyCommandHandler"/>.
    /// </summary>
    [TestFixture]
    public class DeleteUserPermanentlyHandlerTests
    {
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<IJwtTokenRepository> _mockRefreshTokenRepository;
        private Mock<ILogger<DeleteUserPermanentlyCommandHandler>> _mockLogger;
        private DeleteUserPermanentlyCommandHandler _handler;
        private CancellationToken _cancellationToken;

        /// <summary>
        /// Initializes mocks and the handler before each test.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockRefreshTokenRepository = new Mock<IJwtTokenRepository>();
            _mockLogger = new Mock<ILogger<DeleteUserPermanentlyCommandHandler>>();

            _handler = new DeleteUserPermanentlyCommandHandler(
                _mockUserRepository.Object,
                _mockRefreshTokenRepository.Object,
                _mockLogger.Object);

            _cancellationToken = CancellationToken.None;
        }

        /// <summary>
        /// Verifies that when a user exists and is already soft-deleted,
        /// the handler removes refresh tokens, deletes the user and saves changes successfully.
        /// </summary>
        [Test]
        public async Task Handle_UserExistsAndSoftDeleted_ShouldRemoveAndSave()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var adminId = Guid.NewGuid();

            var user = new User
            {
                UserId = userId,
                IsActive = false,
                JwtTokens = new List<JwtToken>
                {
                    new JwtToken { Token = "A", IsRevoked = true },
                    new JwtToken { Token = "B", IsRevoked = true }
                }
            };

            _mockUserRepository
                .Setup(x => x.GetByIdAsync(userId, _cancellationToken))
                .ReturnsAsync(user);

            _mockUserRepository
                .Setup(x => x.SaveChangesAsync(_cancellationToken))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(new DeleteUserPermanentlyCommand(userId, adminId), _cancellationToken);

            // Assert
            Assert.That(result.IsSuccess, Is.True);

            // Verify refresh tokens removed
            _mockRefreshTokenRepository.Verify(
                x => x.RemoveTokenAsync(It.IsAny<JwtToken>(), _cancellationToken),
                Times.Exactly(user.JwtTokens.Count));

            // Verify user removed and saved
            _mockUserRepository.Verify(x => x.Remove(user), Times.Once);
            _mockUserRepository.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Once);
        }

        /// <summary>
        /// Verifies that when the user does not exist,
        /// the handler returns NotFound and performs no repository changes.
        /// </summary>
        [Test]
        public async Task Handle_UserDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var adminId = Guid.NewGuid();

            _mockUserRepository
                .Setup(x => x.GetByIdAsync(userId, _cancellationToken))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _handler.Handle(new DeleteUserPermanentlyCommand(userId, adminId), _cancellationToken);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(DomainErrors.UserError.NotFound));

            _mockRefreshTokenRepository.Verify(
                x => x.RemoveTokenAsync(It.IsAny<JwtToken>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _mockUserRepository.Verify(x => x.Remove(It.IsAny<User>()), Times.Never);
            _mockUserRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        
    }
}
