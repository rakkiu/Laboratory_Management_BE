using IAMService.Application.UseCases.Auth.ForgotPassword;
using IAMService.Domain.Entities;
using IAMService.Domain.Interfaces;
using MediatR;
using Moq;

namespace TestProject.ResetPassword;

[TestFixture]
public class ResetPasswordHandlerTests
{
    private Mock<IUserRepository> _userRepo = null!;
    private Mock<IJwtTokenRepository> _tokenRepo = null!;
    private ResetPasswordHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _userRepo = new Mock<IUserRepository>();
        _tokenRepo = new Mock<IJwtTokenRepository>();
        _handler = new ResetPasswordHandler(_userRepo.Object, _tokenRepo.Object);
    }

    [Test]
    public async Task Handle_ValidToken_ShouldResetPassword_AndRevokeToken()
    {
        var user = new User { UserId = Guid.NewGuid(), Email = "user@test.com", IsActive = true, Password = "old" };
        var token = new JwtToken
        {
            Token = "valid-token",
            TokenType = "PasswordReset",
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IsRevoked = false,
            UserId = user.UserId
        };

        _tokenRepo.Setup(r => r.GetByTokenAsync("valid-token")).ReturnsAsync(token);
        _userRepo.Setup(r => r.GetByIdWithoutDecryptAsync(user.UserId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _userRepo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _handler.Handle(new ResetPasswordCommand("valid-token", "NewPass@123"), CancellationToken.None);

        Assert.That(result, Is.EqualTo(Unit.Value));
        Assert.That(user.Password, Is.Not.EqualTo("old"));
        _userRepo.Verify(r => r.UpdatePasswordOnly(user), Times.Once);
        Assert.That(token.IsRevoked, Is.True);
        _tokenRepo.Verify(r => r.Update(token), Times.Once);
    }

    [Test]
    public void Handle_MissingToken_ShouldThrowArgumentException()
    {
        var ex = Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(new ResetPasswordCommand("", "NewPass@123"), CancellationToken.None));

        Assert.That(ex!.Message, Does.Contain("Reset token is required."));
    }

    [Test]
    public void Handle_MissingPassword_ShouldThrowArgumentException()
    {
        var ex = Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(new ResetPasswordCommand("abc", ""), CancellationToken.None));

        Assert.That(ex!.Message, Does.Contain("New password is required."));
    }

    [Test]
    public void Handle_InvalidToken_ShouldThrowUnauthorized()
    {
        _tokenRepo.Setup(r => r.GetByTokenAsync("invalid")).ReturnsAsync((JwtToken?)null);

        var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _handler.Handle(new ResetPasswordCommand("invalid", "NewPass@123"), CancellationToken.None));

        Assert.That(ex!.Message, Does.Contain("Invalid reset token."));
    }

    [Test]
    public void Handle_RevokedToken_ShouldThrowUnauthorized()
    {
        var token = new JwtToken
        {
            Token = "revoked",
            TokenType = "PasswordReset",
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IsRevoked = true,
            UserId = Guid.NewGuid()
        };
        _tokenRepo.Setup(r => r.GetByTokenAsync("revoked")).ReturnsAsync(token);

        var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _handler.Handle(new ResetPasswordCommand("revoked", "NewPass@123"), CancellationToken.None));

        Assert.That(ex!.Message, Does.Contain("revoked"));
    }

    [Test]
    public void Handle_ExpiredToken_ShouldThrowUnauthorized()
    {
        var token = new JwtToken
        {
            Token = "expired",
            TokenType = "PasswordReset",
            ExpiresAt = DateTime.UtcNow.AddMinutes(-1),
            IsRevoked = false,
            UserId = Guid.NewGuid()
        };
        _tokenRepo.Setup(r => r.GetByTokenAsync("expired")).ReturnsAsync(token);

        var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _handler.Handle(new ResetPasswordCommand("expired", "NewPass@123"), CancellationToken.None));

        Assert.That(ex!.Message, Does.Contain("expired"));
    }

    [Test]
    public void Handle_UserNotFound_ShouldThrowKeyNotFound()
    {
        var token = new JwtToken
        {
            Token = "valid",
            TokenType = "PasswordReset",
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IsRevoked = false,
            UserId = Guid.NewGuid()
        };
        _tokenRepo.Setup(r => r.GetByTokenAsync("valid")).ReturnsAsync(token);
        _userRepo.Setup(r => r.GetByIdWithoutDecryptAsync(token.UserId, It.IsAny<CancellationToken>()))
                 .ReturnsAsync((User?)null);

        var ex = Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(new ResetPasswordCommand("valid", "NewPass@123"), CancellationToken.None));

        Assert.That(ex!.Message, Does.Contain("User not found"));
    }
}