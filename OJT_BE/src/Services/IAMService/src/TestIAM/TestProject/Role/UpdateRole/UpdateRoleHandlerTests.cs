using IAMService.Application.Models.Privilege;
using IAMService.Application.UseCases.Role.Commands.UpdateRole;
using IAMService.Domain.Entities;
using IAMService.Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.Role.UpdateRole
{
    [TestFixture]
    public class UpdateRoleHandlerTests
    {
        private Mock<IRoleRepository> _roleRepositoryMock;
        private Mock<IRolePrivilegeRepository> _rolePrivilegeRepositoryMock;
        private UpdateRoleHandler _handler;

        [SetUp]
        public void Setup()
        {
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _rolePrivilegeRepositoryMock = new Mock<IRolePrivilegeRepository>();
            _handler = new UpdateRoleHandler(_roleRepositoryMock.Object, _rolePrivilegeRepositoryMock.Object);
        }

        // ✅ Test 1: Cập nhật role thành công
        [Test]
        public async Task Handle_ShouldUpdateRole_WhenRoleExists()
        {
            // Arrange
            var role = new IAMService.Domain.Entities.Role
            {
                RoleCode = "ADMIN",
                RoleName = "Administrator",
                RoleDescription = "Old description"
            };

            var command = new UpdateRoleCommand
            {
                RoleCode = "ADMIN",
                RoleName = "Updated Admin",
                RoleDescription = "New Description",
                Privileges = new List<PrivilegeDto>
                {
                    new PrivilegeDto { PrivilegeId = 1, PrivilegeName = "Manage Users", Description = "Can manage users" }
                }
            };

            _roleRepositoryMock.Setup(r => r.GetByIdAsync("ADMIN", It.IsAny<CancellationToken>())).ReturnsAsync(role);
            _rolePrivilegeRepositoryMock.Setup(r => r.DeleteByRoleCode("ADMIN")).Returns(Task.CompletedTask);
            _rolePrivilegeRepositoryMock.Setup(r => r.AddAsync(It.IsAny<RolePrivilege>())).Returns(Task.CompletedTask);
            _roleRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<IAMService.Domain.Entities.Role>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.RoleName, Is.EqualTo("Updated Admin"));
            Assert.That(result.RoleDescription, Is.EqualTo("New Description"));
            Assert.That(result.Privileges.Count, Is.EqualTo(1));

            _roleRepositoryMock.Verify(r => r.UpdateAsync(role, It.IsAny<CancellationToken>()), Times.Once);
            _rolePrivilegeRepositoryMock.Verify(r => r.DeleteByRoleCode("ADMIN"), Times.Once);
            _rolePrivilegeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<RolePrivilege>()), Times.Once);
        }

        // ✅ Test 2: Ném lỗi khi Role không tồn tại
        [Test]
        public void Handle_ShouldThrow_WhenRoleNotFound()
        {
            // Arrange
            _roleRepositoryMock.Setup(r => r.GetByIdAsync("UNKNOWN", It.IsAny<CancellationToken>()))
                               .ReturnsAsync((IAMService.Domain.Entities.Role)null);

            var command = new UpdateRoleCommand
            {
                RoleCode = "UNKNOWN",
                RoleName = "Fake Role",
                Privileges = new List<PrivilegeDto>()
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.That(ex.Message, Does.Contain("Role with ID UNKNOWN not found."));
        }

        // ✅ Test 3: Role có danh sách quyền rỗng
        [Test]
        public async Task Handle_ShouldUpdateRole_WhenPrivilegesEmpty()
        {
            // Arrange
            var role = new IAMService.Domain.Entities.Role { RoleCode = "USER", RoleName = "User", RoleDescription = "Basic user" };

            var command = new UpdateRoleCommand
            {
                RoleCode = "USER",
                RoleName = "User Updated",
                RoleDescription = "Updated description",
                Privileges = new List<PrivilegeDto>() // không có quyền nào
            };

            _roleRepositoryMock.Setup(r => r.GetByIdAsync("USER", It.IsAny<CancellationToken>())).ReturnsAsync(role);
            _rolePrivilegeRepositoryMock.Setup(r => r.DeleteByRoleCode("USER")).Returns(Task.CompletedTask);
            _roleRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<IAMService.Domain.Entities.Role>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.RoleName, Is.EqualTo("User Updated"));
            Assert.That(result.Privileges, Is.Empty);
            _rolePrivilegeRepositoryMock.Verify(r => r.DeleteByRoleCode("USER"), Times.Once);
        }

        // ✅ Test 4: Kiểm tra có xóa quyền cũ và thêm quyền mới
        [Test]
        public async Task Handle_ShouldDeleteOldPrivilegesAndAddNewOnes()
        {
            // Arrange
            var role = new IAMService.Domain.Entities.Role { RoleCode = "STAFF", RoleName = "Staff" };
            var privileges = new List<PrivilegeDto>
            {
                new PrivilegeDto { PrivilegeId = 1, PrivilegeName = "Read Only" },
                new PrivilegeDto { PrivilegeId = 2, PrivilegeName = "Edit Data" }
            };

            var command = new UpdateRoleCommand
            {
                RoleCode = "STAFF",
                RoleName = "Staff Updated",
                RoleDescription = "Updated",
                Privileges = privileges
            };

            _roleRepositoryMock.Setup(r => r.GetByIdAsync("STAFF", It.IsAny<CancellationToken>())).ReturnsAsync(role);
            _rolePrivilegeRepositoryMock.Setup(r => r.DeleteByRoleCode("STAFF")).Returns(Task.CompletedTask);
            _rolePrivilegeRepositoryMock.Setup(r => r.AddAsync(It.IsAny<RolePrivilege>())).Returns(Task.CompletedTask);
            _roleRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<IAMService.Domain.Entities.Role>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _rolePrivilegeRepositoryMock.Verify(r => r.DeleteByRoleCode("STAFF"), Times.Once);
            _rolePrivilegeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<RolePrivilege>()), Times.Exactly(2));
        }

        // ✅ Test 5: Kiểm tra role được cập nhật đúng thông tin
        [Test]
        public async Task Handle_ShouldUpdateRoleProperties()
        {
            // Arrange
            var role = new IAMService.Domain.Entities.Role { RoleCode = "MOD", RoleName = "Moderator", RoleDescription = "Can moderate" };

            var command = new UpdateRoleCommand
            {
                RoleCode = "MOD",
                RoleName = "Moderator Updated",
                RoleDescription = "Has updated privileges",
                Privileges = new List<PrivilegeDto>()
            };

            _roleRepositoryMock.Setup(r => r.GetByIdAsync("MOD", It.IsAny<CancellationToken>())).ReturnsAsync(role);
            _rolePrivilegeRepositoryMock.Setup(r => r.DeleteByRoleCode("MOD")).Returns(Task.CompletedTask);
            _roleRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<IAMService.Domain.Entities.Role>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(role.RoleName, Is.EqualTo("Moderator Updated"));
            Assert.That(role.RoleDescription, Is.EqualTo("Has updated privileges"));
            _roleRepositoryMock.Verify(r => r.UpdateAsync(role, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
