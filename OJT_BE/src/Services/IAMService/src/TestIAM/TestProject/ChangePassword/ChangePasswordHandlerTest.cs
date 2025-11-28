using IAMService.Application.UseCases.Auth.ChangePassword;
using IAMService.Domain.Entities;
using IAMService.Domain.Interfaces;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.ChangePassword;

[TestFixture]
public class ChangePasswordHandlerTest
{
    private Mock<IUserRepository> _mockRepo = null!;
    private ChangePasswordHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _mockRepo = new Mock<IUserRepository>();
        _handler = new ChangePasswordHandler(_mockRepo.Object);
    }

    private static ChangePasswordCommand MakeValidCommand(
        Guid? userId = null,
        string? currentPassword = null,
        string? newPassword = null)
    {
        return new ChangePasswordCommand(
            userId ?? Guid.NewGuid(),
            currentPassword ?? "OldPass123!",
            newPassword ?? "NewPass456!"
        );
    }

    #region Successful Password Change Tests

    [Test]
    public async Task Handle_WithValidInput_ShouldChangePassword_AndReturnTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cmd = MakeValidCommand(userId: userId);

        var existingUser = new User
        {
            UserId = userId,
            Email = "user@example.com",
            Password = BCrypt.Net.BCrypt.HashPassword("OldPass123!"),
            IsActive = true
        };

        _mockRepo
            .Setup(r => r.GetByIdWithoutDecryptAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        _mockRepo
            .Setup(r => r.UpdatePasswordOnly(It.IsAny<User>()))
            .Verifiable();

        _mockRepo
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(Unit.Value));
        _mockRepo.Verify(r => r.GetByIdWithoutDecryptAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepo.Verify(r => r.UpdatePasswordOnly(It.IsAny<User>()), Times.Once);
        _mockRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_WithValidInput_ShouldHashNewPassword()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cmd = MakeValidCommand(userId: userId, newPassword: "SecurePass789!");
        User? capturedUser = null;

        var existingUser = new User
        {
            UserId = userId,
            Email = "user@example.com",
            Password = BCrypt.Net.BCrypt.HashPassword("OldPass123!"),
            IsActive = true
        };

        _mockRepo
            .Setup(r => r.GetByIdWithoutDecryptAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        _mockRepo
            .Setup(r => r.UpdatePasswordOnly(It.IsAny<User>()))
            .Callback<User>(u => capturedUser = u);

        _mockRepo
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.That(capturedUser, Is.Not.Null);
        Assert.That(capturedUser!.Password, Is.Not.EqualTo("SecurePass789!"));
        Assert.That(BCrypt.Net.BCrypt.Verify("SecurePass789!", capturedUser.Password), Is.True);
    }

    #endregion

    #region Validation Tests

    [Test]
    public void Handle_WhenUserNotFound_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var cmd = MakeValidCommand();

        _mockRepo
            .Setup(r => r.GetByIdWithoutDecryptAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(cmd, CancellationToken.None));
        Assert.That(ex!.Message, Does.Contain("User not found"));
    }
    #endregion
    [Test]
    public void Handle_WhenUserIsInactive_ShouldThrowArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cmd = MakeValidCommand(userId: userId);

        var inactiveUser = new User
        {
            UserId = userId,
            Email = "user@example.com",
            Password = BCrypt.Net.BCrypt.HashPassword("OldPass123!"),
            IsActive = false
        };

        _mockRepo
            .Setup(r => r.GetByIdWithoutDecryptAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(inactiveUser);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(cmd, CancellationToken.None));
        Assert.That(ex!.Message, Does.Contain("User is inactive"));
    }

    [Test]
    public void Handle_WhenCurrentPasswordIsIncorrect_ShouldThrowArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cmd = MakeValidCommand(userId: userId, currentPassword: "WrongPassword!");

        var existingUser = new User
        {
            UserId = userId,
            Email = "user@example.com",
            Password = BCrypt.Net.BCrypt.HashPassword("OldPass123!"),
            IsActive = true
        };

        _mockRepo
            .Setup(r => r.GetByIdWithoutDecryptAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act & Assert
        var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(cmd, CancellationToken.None));
        Assert.That(ex!.Message, Does.Contain("Current password is incorrect."));
    }

    [TestCase("")]
    [TestCase("   ")]
    [TestCase(null)]
    public void Handle_WhenNewPasswordIsEmpty_ShouldThrowArgumentException(string? newPassword)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cmd = MakeValidCommand(userId: userId, newPassword: newPassword!);

        var existingUser = new User
        {
            UserId = userId,
            Email = "user@example.com",
            Password = BCrypt.Net.BCrypt.HashPassword("OldPass123!"),
            IsActive = true
        };
    }
}
