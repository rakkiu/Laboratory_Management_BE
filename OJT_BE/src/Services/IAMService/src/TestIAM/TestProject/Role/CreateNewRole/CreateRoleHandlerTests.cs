using IAMService.Application.Models.Privilege;
using IAMService.Application.UseCases.Role.Commands;
using IAMService.Application.UseCases.Role.Commands.CreateRole;
using IAMService.Domain.Entities;
using IAMService.Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.Role.CreateNewRole
{
    [TestFixture]
    public class CreateRoleHandlerTests
    {
        private Mock<IRoleRepository> _mockRoleRepo = null!;
        private Mock<IPrivilegeRepository> _mockPrivilegeRepo = null!;
        private Mock<IRolePrivilegeRepository> _mockRolePrivilegeRepo = null!;
        private CreateRoleHandler _handler = null!;

        [SetUp]
        public void Setup()
        {
            _mockRoleRepo = new Mock<IRoleRepository>();
            _mockPrivilegeRepo = new Mock<IPrivilegeRepository>();
            _mockRolePrivilegeRepo = new Mock<IRolePrivilegeRepository>();
            _handler = new CreateRoleHandler(_mockRoleRepo.Object, _mockPrivilegeRepo.Object, _mockRolePrivilegeRepo.Object);
        }

        private static CreateRoleCommand MakeValidCommand()
        {
            return new CreateRoleCommand
            {
                RoleCode = "NURSE",
                RoleName = "Nurse",
                RoleDescription = "Responsible for assisting doctors",
                Privileges = new List<PrivilegeDto>
                {
                    new PrivilegeDto { PrivilegeId = 15, PrivilegeName = "ViewPatients" },
                    new PrivilegeDto { PrivilegeId = 36, PrivilegeName = "UpdateRecords" }
                }
            };
        }

        #region SUCCESS CASE

        [Test]
        public async Task Handle_WithValidInput_ShouldCreateRole_AndReturnDto()
        {
            // Arrange
            var cmd = MakeValidCommand();
            var privilegeEntities = cmd.Privileges.Select(p => new Privilege
            {
                PrivilegeId = p.PrivilegeId,
                PrivilegeName = p.PrivilegeName,
                Description = p.PrivilegeName + " description"
            }).ToList();

            _mockRoleRepo.Setup(r => r.RoleExistsAsync(cmd.RoleCode)).ReturnsAsync(false);
            _mockRoleRepo.Setup(r => r.AddAsync(It.IsAny<IAMService.Domain.Entities.Role>())).Returns(Task.CompletedTask);

            foreach (var priv in privilegeEntities)
            {
                _mockPrivilegeRepo.Setup(p => p.GetByIdAsync(priv.PrivilegeId)).ReturnsAsync(priv);
            }

            _mockRolePrivilegeRepo
    .Setup(rp => rp.AddAsync(It.IsAny<RolePrivilege>()))
    .Callback<RolePrivilege>(rp =>
    {
        // gán Privilege thủ công để tránh null
        rp.Privilege = privilegeEntities.First(p => p.PrivilegeId == rp.PrivilegeId);
    })
    .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RoleCode, Is.EqualTo("NURSE"));
            Assert.That(result.RoleName, Is.EqualTo("Nurse"));
            Assert.That(result.Privileges.Count, Is.EqualTo(2));
            Assert.That(result.Privileges.Any(p => p.PrivilegeName == "ViewPatients"), Is.True);

            _mockRoleRepo.Verify(r => r.AddAsync(It.IsAny<IAMService.Domain.Entities.Role>()), Times.Once);
            _mockPrivilegeRepo.Verify(p => p.GetByIdAsync(It.IsAny<int>()), Times.Exactly(2));
            _mockRolePrivilegeRepo.Verify(rp => rp.AddAsync(It.IsAny<RolePrivilege>()), Times.Exactly(2));
        }

        #endregion

        #region VALIDATION CASES

        [Test]
        public void Handle_WhenRoleAlreadyExists_ShouldThrowArgumentException()
        {
            // Arrange
            var cmd = MakeValidCommand();
            _mockRoleRepo.Setup(r => r.RoleExistsAsync(cmd.RoleCode)).ReturnsAsync(true);

            // Act + Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(cmd, CancellationToken.None));
            Assert.That(ex!.Message, Does.Contain($"Role with RoleCode '{cmd.RoleCode}' already exists."));
            _mockRoleRepo.Verify(r => r.AddAsync(It.IsAny<IAMService.Domain.Entities.Role>()), Times.Never);
        }

        [Test]
        public void Handle_WhenPrivilegeDoesNotExist_ShouldThrowArgumentException()
        {
            // Arrange
            var cmd = MakeValidCommand();
            var invalidPrivId = cmd.Privileges.First().PrivilegeId;

            _mockRoleRepo.Setup(r => r.RoleExistsAsync(cmd.RoleCode)).ReturnsAsync(false);
            _mockRoleRepo.Setup(r => r.AddAsync(It.IsAny<IAMService.Domain.Entities.Role>())).Returns(Task.CompletedTask);
            _mockPrivilegeRepo.Setup(p => p.GetByIdAsync(invalidPrivId)).ReturnsAsync((Privilege?)null);

            // Act + Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(cmd, CancellationToken.None));
            Assert.That(ex!.Message, Does.Contain($"Privilege with Id '{invalidPrivId}' does not exist."));
            _mockRolePrivilegeRepo.Verify(rp => rp.AddAsync(It.IsAny<RolePrivilege>()), Times.Never);
        }

        [Test]
        public async Task Handle_WithEmptyPrivileges_ShouldStillCreateRole()
        {
            // Arrange
            var cmd = new CreateRoleCommand
            {
                RoleCode = "CASHIER",
                RoleName = "Cashier",
                RoleDescription = "Handles payments",
                Privileges = new List<PrivilegeDto>()
            };

            _mockRoleRepo.Setup(r => r.RoleExistsAsync(cmd.RoleCode)).ReturnsAsync(false);
            _mockRoleRepo.Setup(r => r.AddAsync(It.IsAny<IAMService.Domain.Entities.Role>())).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.That(result.RoleCode, Is.EqualTo("CASHIER"));
            Assert.That(result.Privileges, Is.Empty);
        }

        #endregion
    }
}
