using IAMService.Application.UseCases.Role.Commands.DeleteRole;
using IAMService.Domain.Entities;
using IAMService.Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.Role.DeleteRole
{
    [TestFixture]
    public class DeleteRoleHandlerTests
    {
        private Mock<IRoleRepository> _mockRoleRepository;
        private Mock<IRolePrivilegeRepository> _mockRolePrivilegeRepository;
        private Mock<IUserRepository> _mockUserRepository;
        private DeleteRoleHandler _handler;
        private CancellationToken _cancellationToken;

        [SetUp]
        public void SetUp()
        {
            _mockRoleRepository = new Mock<IRoleRepository>();
            _mockRolePrivilegeRepository = new Mock<IRolePrivilegeRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _handler = new DeleteRoleHandler(
                _mockRoleRepository.Object,
                _mockRolePrivilegeRepository.Object,
                _mockUserRepository.Object);
            _cancellationToken = CancellationToken.None;
        }

        // 🧩 Test case 1: Không cho phép xóa role mặc định
        [Test]
        public void Handle_ShouldThrow_WhenRoleCodeIsProtected()
        {
            // Arrange
            var command = new DeleteRoleCommand("ADMIN");

            // Act + Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, _cancellationToken));
            Assert.That(ex!.Message, Is.EqualTo("Role with code ADMIN cannot be deleted."));
        }

        // 🧩 Test case 2: Role không tồn tại → ném lỗi
        [Test]
        public void Handle_ShouldThrow_WhenRoleDoesNotExist()
        {
            // Arrange
            var command = new DeleteRoleCommand("TEST_ROLE");
            _mockRoleRepository
                .Setup(r => r.GetByIdAsync(command.RoleCode, _cancellationToken))
                .ReturnsAsync((IAMService.Domain.Entities.Role)null);

            // Act + Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, _cancellationToken));
            Assert.That(ex!.Message, Is.EqualTo("Role with code TEST_ROLE does not exist."));
        }

        // 🧩 Test case 3: Role có user đang sử dụng → update về CUSTOM_ROLE
        [Test]
        public async Task Handle_ShouldUpdateUsersAndDeleteRole_WhenUsersHaveThisRole()
        {
            // Arrange
            var command = new DeleteRoleCommand("NURSE");

            var mockRole = new IAMService.Domain.Entities.Role { RoleCode = "NURSE", RoleName = "Nurse" };
            var users = new List<User>
            {
                new User { UserId = Guid.NewGuid(), RoleCode = "NURSE" },
                new User { UserId = Guid.NewGuid(), RoleCode = "NURSE" }
            };

            _mockRoleRepository
                .Setup(r => r.GetByIdAsync(command.RoleCode, _cancellationToken))
                .ReturnsAsync(mockRole);

            _mockUserRepository
                .Setup(u => u.GetUsersByRoleCodeAsync(command.RoleCode, _cancellationToken))
                .ReturnsAsync(users);

            // Act
            await _handler.Handle(command, _cancellationToken);

            // Assert
            _mockUserRepository.Verify(u => u.UpdateV1(It.Is<User>(x => x.RoleCode == "CUSTOM_ROLE")), Times.Exactly(2));
            _mockRolePrivilegeRepository.Verify(r => r.DeleteByRoleCode(command.RoleCode), Times.Once);
            _mockRoleRepository.Verify(r => r.DeleteAsync(mockRole, _cancellationToken), Times.Once);
        }

        // 🧩 Test case 4: Role không có user nào → chỉ xóa role và quyền
        [Test]
        public async Task Handle_ShouldDeleteRoleAndPrivileges_WhenNoUsersAssigned()
        {
            // Arrange
            var command = new DeleteRoleCommand("TEMP_ROLE");
            var mockRole = new IAMService.Domain.Entities.Role { RoleCode = "TEMP_ROLE", RoleName = "Temporary" };

            _mockRoleRepository
                .Setup(r => r.GetByIdAsync(command.RoleCode, _cancellationToken))
                .ReturnsAsync(mockRole);

            _mockUserRepository
                .Setup(u => u.GetUsersByRoleCodeAsync(command.RoleCode, _cancellationToken))
                .ReturnsAsync(new List<User>());

            // Act
            await _handler.Handle(command, _cancellationToken);

            // Assert
            _mockRolePrivilegeRepository.Verify(r => r.DeleteByRoleCode(command.RoleCode), Times.Once);
            _mockRoleRepository.Verify(r => r.DeleteAsync(mockRole, _cancellationToken), Times.Once);
            _mockUserRepository.Verify(u => u.UpdateV1(It.IsAny<User>()), Times.Never);
        }
    }
}
