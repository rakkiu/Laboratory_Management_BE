using IAMService.Application.UseCases.Users.Commands.UnlockUser;
using IAMService.Domain.Errors;
using IAMService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

/// <summary>
/// Alias for the domain User entity to avoid namespace conflicts.
/// </summary>
using DomainUser = IAMService.Domain.Entities.User;

namespace TestIAM.TestProject.User.UnlockUser
{
    /// <summary>
    /// Defines test cases for the <see cref="UnlockUserCommandHandler"/>.
    /// </summary>
    [TestFixture]
    public class UnlockUserCommandHandlerTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<ILogger<UnlockUserCommandHandler>> _loggerMock;
        private UnlockUserCommandHandler _handler;

        /// <summary>
        /// Initializes the mocks and the handler instance before each test run.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<UnlockUserCommandHandler>>();
            _handler = new UnlockUserCommandHandler(_userRepositoryMock.Object, _loggerMock.Object);
        }

        /// <summary>
        /// Verifies that handling the command fails when the specified user is not found.
        /// </summary>
        [Test]
        public async Task Handle_WhenUserNotFound_ReturnsFailure()
        {
            // Arrange
            var command = new UnlockUserCommand(Guid.NewGuid(), Guid.NewGuid());
            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((DomainUser)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(DomainErrors.UserError.NotFound));

            _userRepositoryMock.Verify(x => x.Update(It.IsAny<DomainUser>()), Times.Never);
            _userRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _loggerMock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Verifies that handling the command fails when the user is already unlocked (IsActive is true).
        /// </summary>
        [Test]
        public async Task Handle_WhenUserAlreadyUnlocked_ReturnsFailure()
        {
            // Arrange
            var user = new DomainUser
            {
                UserId = Guid.NewGuid(),
                IsActive = true
            };

            var command = new UnlockUserCommand(user.UserId, Guid.NewGuid());
            _userRepositoryMock.Setup(x => x.GetByIdAsync(user.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(DomainErrors.UserError.AlreadyUnlocked));

            _userRepositoryMock.Verify(x => x.Update(It.IsAny<DomainUser>()), Times.Never);
            _userRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _loggerMock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Verifies that an exception thrown by the repository's GetByIdAsync method is propagated up the call stack.
        /// </summary>
        [Test]
        public void Handle_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var user = new DomainUser
            {
                UserId = Guid.NewGuid(),
                IsActive = false
            };
            var command = new UnlockUserCommand(user.UserId, Guid.NewGuid());

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Database failure"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _handler.Handle(command, CancellationToken.None));

            Assert.That(ex.Message, Does.Contain("Database failure"));
        }

        /// <summary>
        /// Verifies that an exception thrown by the repository's SaveChangesAsync method is propagated up the call stack.
        /// </summary>
        [Test]
        public async Task Handle_WhenSaveChangesThrows_ShouldStillThrow()
        {
            // Arrange
            var user = new DomainUser
            {
                UserId = Guid.NewGuid(),
                IsActive = false
            };
            var command = new UnlockUserCommand(user.UserId, Guid.NewGuid());

            _userRepositoryMock.Setup(x => x.GetByIdAsync(user.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _userRepositoryMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Save failed"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _handler.Handle(command, CancellationToken.None));

            Assert.That(ex.Message, Is.EqualTo("Save failed"));
        }
    }
}