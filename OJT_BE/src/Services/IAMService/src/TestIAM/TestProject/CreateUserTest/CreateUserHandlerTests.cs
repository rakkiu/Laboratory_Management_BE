using IAMService.Application.Interfaces;
using IAMService.Application.UseCases.User.Commands.CreateUser;
using IAMService.Domain.Entities;
using IAMService.Domain.Interfaces;
using Moq;
using NUnit.Framework;

namespace TestProject.CreateUserTest
{
    [TestFixture]
    public class CreateUserHandlerTests
    {
        private Mock<IUserRepository> _mockRepo = null!;
        private Mock<IEmailService> _mockEmailService = null!;
        private Mock<IRoleRepository> _mockRoleRepo = null!;
        private CreateUserHandler _handler = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IUserRepository>();
            _mockEmailService = new Mock<IEmailService>();
            _mockRoleRepo = new Mock<IRoleRepository>();

            // ✅ Mặc định giả lập rằng mọi RoleCode đều tồn tại
            _mockRoleRepo
                .Setup(r => r.RoleExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            _handler = new CreateUserHandler(_mockRepo.Object, _mockEmailService.Object, _mockRoleRepo.Object);
        }

        private static CreateUserCommand MakeValidCommand(
            string? email = null,
            string? dob = null,
            string? gender = null,
            string? role = null,
            string? phone = null,
            string? identifyNumber = null)
        {
            return new CreateUserCommand
            {
                RoleCode = role ?? "LAB_MANAGER",
                Email = email ?? "newuser@example.com",
                PhoneNumber = phone ?? "0987654321",
                FullName = "New User",
                IdentifyNumber = identifyNumber ?? "ID999999",
                Gender = gender ?? "Male",
                Age = 28,
                Address = "HCM City",
                DateOfBirth = dob ?? "01/31/1995"
            };
        }

        #region Successful Creation Tests

        [Test]
        public async Task Handle_WithValidInput_ShouldCreateUser_AndReturnDto()
        {
            // Arrange
            var cmd = MakeValidCommand();
            User? capturedUser = null;

            _mockRepo
                .Setup(r => r.GetByEmailAsync(cmd.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            _mockRepo
                .Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((u, ct) => capturedUser = u)
                .Returns(Task.CompletedTask);

            _mockRepo
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _mockRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid id, CancellationToken ct) => capturedUser!);

            _mockEmailService
                .Setup(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Email, Is.EqualTo("newuser@example.com"));
            Assert.That(result.RoleCode, Is.EqualTo("LAB_MANAGER"));
            Assert.That(result.Gender, Is.EqualTo("Male"));

            _mockRepo.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockRoleRepo.Verify(r => r.RoleExistsAsync(cmd.RoleCode!), Times.Once);
        }

        [TestCase("Male")]
        [TestCase("male")]
        [TestCase("M")]
        [TestCase("m")]
        public async Task Handle_WithVariousMaleGenderFormats_ShouldNormalizeToMale(string gender)
        {
            var cmd = MakeValidCommand(gender: gender);
            User? capturedUser = null;

            _mockRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((u, ct) => capturedUser = u)
                .Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid id, CancellationToken ct) => capturedUser!);

            var result = await _handler.Handle(cmd, CancellationToken.None);

            Assert.That(result.Gender, Is.EqualTo("Male"));
            _mockRoleRepo.Verify(r => r.RoleExistsAsync(cmd.RoleCode!), Times.Once);
        }

        [TestCase("Female")]
        [TestCase("female")]
        [TestCase("F")]
        [TestCase("f")]
        public async Task Handle_WithVariousFemaleGenderFormats_ShouldNormalizeToFemale(string gender)
        {
            var cmd = MakeValidCommand(gender: gender);
            User? capturedUser = null;

            _mockRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((u, ct) => capturedUser = u)
                .Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid id, CancellationToken ct) => capturedUser!);

            var result = await _handler.Handle(cmd, CancellationToken.None);

            Assert.That(result.Gender, Is.EqualTo("Female"));
            _mockRoleRepo.Verify(r => r.RoleExistsAsync(cmd.RoleCode!), Times.Once);
        }

        #endregion

        #region Validation Tests

        [Test]
        public void Handle_WhenRoleDoesNotExist_ShouldThrowArgumentException()
        {
            // Arrange
            _mockRepo
                .Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            _mockRoleRepo
                .Setup(r => r.RoleExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false); // ❌ Role không tồn tại

            var cmd = MakeValidCommand(role: "INVALID_ROLE");

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(cmd, CancellationToken.None));
            Assert.That(ex!.Message, Does.Contain("Role does not exist."));
        }

        #endregion
    }
}
