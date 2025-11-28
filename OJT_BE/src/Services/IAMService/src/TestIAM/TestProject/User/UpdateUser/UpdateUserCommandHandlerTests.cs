using IAMService.Application.Features.Users.Commands;
using IAMService.Application.UseCases.Users.Commands;
using IAMService.Domain.Entities;
using IAMService.Domain.Interfaces;
using Moq;

namespace IAMService.Tests.Application.Users
{
    [TestFixture]
    public class UpdateUserCommandHandlerTests
    {
        private Mock<IUserRepository> _userRepositoryMock = null!;
        private UpdateUserCommandHandler _handler = null!;

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new UpdateUserCommandHandler(_userRepositoryMock.Object);
        }

        [Test]
        public void Should_ThrowException_When_User_NotFound()
        {
            // Arrange
            var command = new UpdateUserCommand(
                Guid.NewGuid(), "John", DateTime.UtcNow.AddYears(-20), 20, "male", "Hanoi", "john@example.com", "0912345678");

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(command.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.That(ex!.Message, Does.Contain("not found"));
        }

        [Test]
        public async Task Should_UpdateUser_When_UserExists()
        {
            // Arrange
            var user = new User
            {
                UserId = Guid.NewGuid(),
                FullName = "Old",
                Email = "old@example.com"
            };

            var command = new UpdateUserCommand(
                user.UserId, "New", DateTime.UtcNow.AddYears(-22), 22, "female", "HCM", "new@example.com", "0987654321");

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(user.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // ✅ Nếu SaveChangesAsync trả về Task<int> (EF Core)
            _userRepositoryMock
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(user.FullName, Is.EqualTo("New"));
            Assert.That(user.Email, Is.EqualTo("new@example.com"));
            Assert.That(user.Address, Is.EqualTo("HCM"));
            Assert.That(user.Age, Is.EqualTo(22));
            Assert.That(user.Gender, Is.EqualTo("female"));
            _userRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
