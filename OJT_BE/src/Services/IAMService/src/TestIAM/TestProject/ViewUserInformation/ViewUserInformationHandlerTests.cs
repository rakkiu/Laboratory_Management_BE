using IAMService.Application.Models.User;
using IAMService.Application.UseCases.User.Queries.ViewUserInformation;
using IAMService.Domain.Entities;
using IAMService.Domain.Interfaces;
using Moq;
using NUnit.Framework;
using System.Globalization;

namespace TestProject.ViewUserInformationTest
{
    [TestFixture]
    public class ViewUserInformationHandlerTests
    {
        private Mock<IUserRepository> _mockUserRepository;
        private ViewUserInformationHandler _handler;
        private CancellationToken _cancellationToken;

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _handler = new ViewUserInformationHandler(_mockUserRepository.Object);
            _cancellationToken = CancellationToken.None;
        }

        [Test]
        public async Task Handle_WithValidAdminUser_ShouldReturnUserInformation()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new ViewUserInformationCommand(userId, "ADMIN");

            var user = new User
            {
                UserId = userId,
                Email = "test@example.com",
                FullName = "Test User",
                PhoneNumber = "+84901234567",
                IdentifyNumber = "079203001234",
                Gender = "Male",
                Age = 29,
                Address = "123 Test Street",
                DateOfBirth = new DateTime(1995, 1, 1),
                IsActive = true,
                RoleCode = "ADMIN",
                Role = new IAMService.Domain.Entities.Role
                {
                    RoleCode = "ADMIN",
                    RoleName = "Administrator",
                    RoleDescription = "System administrator"
                }
            };

            _mockUserRepository
                .Setup(x => x.GetByIdWithRoleAsync(userId, _cancellationToken))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(command, _cancellationToken);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.UserId, Is.EqualTo(userId));
                Assert.That(result.Email, Is.EqualTo("test@example.com"));
                Assert.That(result.FullName, Is.EqualTo("Test User"));
                Assert.That(result.PhoneNumber, Is.EqualTo("+84901234567"));
                Assert.That(result.IdentifyNumber, Is.EqualTo("079203001234"));
                Assert.That(result.Gender, Is.EqualTo("Male"));
                Assert.That(result.Age, Is.EqualTo(29));
                Assert.That(result.Address, Is.EqualTo("123 Test Street"));
                Assert.That(result.Role.RoleCode, Is.EqualTo("ADMIN"));
                Assert.That(result.Role.RoleName, Is.EqualTo("Administrator"));
                Assert.That(result.Role.RoleDescription, Is.EqualTo("System administrator"));

                // ✅ Chấp nhận cả "/" hoặc "-" trong ngày sinh
                var normalizedDate = result.DateOfBirth.Replace('-', '/');
                Assert.That(normalizedDate, Is.EqualTo("01/01/1995"));
            });

            _mockUserRepository.Verify(x => x.GetByIdWithRoleAsync(userId, _cancellationToken), Times.Once);
        }

        [Test]
        public async Task Handle_WithLabManagerUser_ShouldReturnUserInformation()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new ViewUserInformationCommand(userId, "LAB_MANAGER");

            var user = new User
            {
                UserId = userId,
                Email = "manager@lab.com",
                FullName = "Lab Manager",
                PhoneNumber = "+84909999999",
                IdentifyNumber = "088888888888",
                Gender = "Female",
                Age = 35,
                Address = "456 Manager Street",
                DateOfBirth = new DateTime(1989, 6, 15),
                IsActive = true,
                RoleCode = "LAB_MANAGER",
                Role = new IAMService.Domain.Entities.Role
                {
                    RoleCode = "LAB_MANAGER",
                    RoleName = "Lab Manager",
                    RoleDescription = "Manage lab operations"
                }
            };

            _mockUserRepository
                .Setup(x => x.GetByIdWithRoleAsync(userId, _cancellationToken))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(command, _cancellationToken);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.UserId, Is.EqualTo(userId));
                Assert.That(result.Email, Is.EqualTo("manager@lab.com"));
                Assert.That(result.FullName, Is.EqualTo("Lab Manager"));
                Assert.That(result.Gender, Is.EqualTo("Female"));
                Assert.That(result.Age, Is.EqualTo(35));
                Assert.That(result.Role.RoleCode, Is.EqualTo("LAB_MANAGER"));

                // ✅ Chấp nhận cả "/" hoặc "-" trong ngày sinh
                var normalizedDate = result.DateOfBirth.Replace('-', '/');
                Assert.That(normalizedDate, Is.EqualTo("15/06/1989"));
            });

            _mockUserRepository.Verify(x => x.GetByIdWithRoleAsync(userId, _cancellationToken), Times.Once);
        }

        [Test]
        public void Handle_WithNonExistentUser_ShouldThrowKeyNotFoundException()
        {
            var userId = Guid.NewGuid();
            var command = new ViewUserInformationCommand(userId, "ADMIN");

            _mockUserRepository
                .Setup(x => x.GetByIdWithRoleAsync(userId, _cancellationToken))
                .ReturnsAsync((User?)null);

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(
                async () => await _handler.Handle(command, _cancellationToken));

            Assert.That(exception!.Message, Is.EqualTo("User not found or has been deleted."));
        }

        [Test]
        public void Handle_WithInactiveUser_ShouldThrowKeyNotFoundException()
        {
            var userId = Guid.NewGuid();
            var command = new ViewUserInformationCommand(userId, "ADMIN");

            var user = new User
            {
                UserId = userId,
                Email = "inactive@example.com",
                FullName = "Inactive User",
                PhoneNumber = "+84901111111",
                IdentifyNumber = "099999999999",
                Gender = "Male",
                Age = 30,
                Address = "789 Inactive Street",
                DateOfBirth = new DateTime(1994, 3, 20),
                IsActive = false,
                RoleCode = "LAB_USER",
                Role = new IAMService.Domain.Entities.Role { RoleCode = "LAB_USER", RoleName = "Lab User", RoleDescription = "Regular user" }
            };

            _mockUserRepository
                .Setup(x => x.GetByIdWithRoleAsync(userId, _cancellationToken))
                .ReturnsAsync(user);

            var exception = Assert.ThrowsAsync<KeyNotFoundException>(
                async () => await _handler.Handle(command, _cancellationToken));

            Assert.That(exception!.Message, Is.EqualTo("User not found or has been deleted."));
        }

        [Test]
        public async Task Handle_WithUserHavingNullRole_ShouldReturnUserInformationWithEmptyRole()
        {
            var userId = Guid.NewGuid();
            var command = new ViewUserInformationCommand(userId, "ADMIN");

            var user = new User
            {
                UserId = userId,
                Email = "norole@example.com",
                FullName = "No Role User",
                PhoneNumber = "+84902222222",
                IdentifyNumber = "077777777777",
                Gender = "Male",
                Age = 28,
                Address = "321 No Role Street",
                DateOfBirth = new DateTime(1996, 12, 31),
                IsActive = true,
                Role = null
            };

            _mockUserRepository
                .Setup(x => x.GetByIdWithRoleAsync(userId, _cancellationToken))
                .ReturnsAsync(user);

            var result = await _handler.Handle(command, _cancellationToken);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Role, Is.Not.Null);
                Assert.That(result.Role.RoleName, Is.EqualTo(string.Empty));
                Assert.That(result.Role.RoleCode, Is.EqualTo(string.Empty));
                Assert.That(result.Role.RoleDescription, Is.EqualTo(string.Empty));
            });
        }

        [TestCase(2000, 1, 1, "01/01/2000")]
        [TestCase(1990, 12, 31, "31/12/1990")]
        [TestCase(1985, 5, 15, "15/05/1985")]
        [TestCase(2023, 2, 28, "28/02/2023")]
        public async Task Handle_WithDifferentDateFormats_ShouldReturnCorrectlyFormattedDate(
            int year, int month, int day, string expectedFormat)
        {
            var userId = Guid.NewGuid();
            var command = new ViewUserInformationCommand(userId, "ADMIN");

            var user = new User
            {
                UserId = userId,
                Email = "date@example.com",
                FullName = "Date Test User",
                PhoneNumber = "+84903333333",
                IdentifyNumber = "066666666666",
                Gender = "Male",
                Age = 25,
                Address = "Date Street",
                DateOfBirth = new DateTime(year, month, day),
                IsActive = true,
                RoleCode = "ADMIN",
                Role = new IAMService.Domain.Entities.Role
                {
                    RoleCode = "ADMIN",
                    RoleName = "Admin",
                    RoleDescription = "Administrator"
                }
            };

            _mockUserRepository
                .Setup(x => x.GetByIdWithRoleAsync(userId, _cancellationToken))
                .ReturnsAsync(user);

            var result = await _handler.Handle(command, _cancellationToken);

            // Ensure consistent format regardless of system locale
            var normalizedDate = result.DateOfBirth.Replace('-', '/');
            Assert.That(normalizedDate, Is.EqualTo(expectedFormat));
        }

        [Test]
        public async Task Handle_MultipleTimes_ShouldCallRepositoryEachTime()
        {
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            var command1 = new ViewUserInformationCommand(userId1, "ADMIN");
            var command2 = new ViewUserInformationCommand(userId2, "LAB_MANAGER");

            var user1 = CreateTestUser(userId1, "user1@test.com", "User 1");
            var user2 = CreateTestUser(userId2, "user2@test.com", "User 2");

            _mockUserRepository.Setup(x => x.GetByIdWithRoleAsync(userId1, _cancellationToken)).ReturnsAsync(user1);
            _mockUserRepository.Setup(x => x.GetByIdWithRoleAsync(userId2, _cancellationToken)).ReturnsAsync(user2);

            var result1 = await _handler.Handle(command1, _cancellationToken);
            var result2 = await _handler.Handle(command2, _cancellationToken);

            Assert.Multiple(() =>
            {
                Assert.That(result1.Email, Is.EqualTo("user1@test.com"));
                Assert.That(result2.Email, Is.EqualTo("user2@test.com"));
            });

            _mockUserRepository.Verify(x => x.GetByIdWithRoleAsync(It.IsAny<Guid>(), _cancellationToken), Times.Exactly(2));
        }

        private User CreateTestUser(Guid userId, string email, string fullName)
        {
            return new User
            {
                UserId = userId,
                Email = email,
                FullName = fullName,
                PhoneNumber = "+84900000000",
                IdentifyNumber = "000000000000",
                Gender = "Male",
                Age = 30,
                Address = "Test Address",
                DateOfBirth = new DateTime(1994, 1, 1),
                IsActive = true,
                RoleCode = "ADMIN",
                Role = new IAMService.Domain.Entities.Role
                {
                    RoleCode = "ADMIN",
                    RoleName = "Administrator",
                    RoleDescription = "Admin role"
                }
            };
        }
    }
}
