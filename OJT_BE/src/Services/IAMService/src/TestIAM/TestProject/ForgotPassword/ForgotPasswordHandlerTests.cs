using IAMService.Application.Interfaces;
using IAMService.Application.UseCases.Auth.ForgotPassword;
using IAMService.Domain.Entities;
using IAMService.Domain.Interfaces;
using MediatR;
using Moq;

namespace TestProject.ForgotPassword;

[TestFixture]
public class ForgotPasswordHandlerTests
{
    private Mock<IUserRepository> _userRepo = null!;
    private Mock<IJwtTokenRepository> _tokenRepo = null!;
    private Mock<IEmailService> _email = null!;
    private ForgotPasswordHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _userRepo = new Mock<IUserRepository>();
        _tokenRepo = new Mock<IJwtTokenRepository>();
        _email = new Mock<IEmailService>();
        _handler = new ForgotPasswordHandler(_userRepo.Object, _tokenRepo.Object, _email.Object);
    }

    [Test]
    public async Task Handle_ValidEmail_ShouldCreateResetToken_AndSendEmail()
    {
        // Arrange
        var user = new User { UserId = Guid.NewGuid(), Email = "user@test.com", IsActive = true };
        _userRepo.Setup(r => r.GetByEmailWithoutDecryptAsync("user@test.com", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(user);

        JwtToken? captured = null;
        _tokenRepo.Setup(r => r.SaveResetTokenAsync(It.IsAny<JwtToken>(), It.IsAny<CancellationToken>()))
                  .Callback<JwtToken, CancellationToken>((t, _) => captured = t)
                  .Returns(Task.CompletedTask);

        _email.Setup(e => e.SendAsync("user@test.com", It.IsAny<string>(), It.IsAny<string>()))
              .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(new ForgotPasswordCommand("user@test.com"), CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(Unit.Value));
        Assert.That(captured, Is.Not.Null);
        Assert.That(captured!.TokenType, Is.EqualTo("PasswordReset"));
        Assert.That(captured.UserId, Is.EqualTo(user.UserId));
        Assert.That(captured.IsRevoked, Is.False);
        Assert.That(captured.ExpiresAt, Is.GreaterThan(DateTime.UtcNow));
        _email.Verify(e => e.SendAsync("user@test.com", It.IsAny<string>(), It.Is<string>(b => b.Contains("Reset Password"))), Times.Once);
    }

    [Test]
    public void Handle_EmptyEmail_ShouldThrowArgumentException()
    {
        var ex = Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(new ForgotPasswordCommand(""), CancellationToken.None));

        Assert.That(ex!.Message, Does.Contain("Email is missing."));
    }

    [Test]
    public void Handle_EmailNotFound_ShouldThrowKeyNotFound()
    {
        _userRepo.Setup(r => r.GetByEmailWithoutDecryptAsync("no@test.com", It.IsAny<CancellationToken>()))
                 .ReturnsAsync((User?)null);

        var ex = Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(new ForgotPasswordCommand("no@test.com"), CancellationToken.None));

        Assert.That(ex!.Message, Does.Contain("Email not found"));
        _tokenRepo.Verify(r => r.SaveResetTokenAsync(It.IsAny<JwtToken>(), It.IsAny<CancellationToken>()), Times.Never);
        _email.Verify(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}