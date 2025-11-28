using IAMService.Application.UseCases.Role.Queries.GetAllRole;
using IAMService.Domain.Entities;
using IAMService.Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.Role.GetAllRoles
{
    [TestFixture]
    public class GetAllRolesHandlerTests
    {
        private Mock<IRoleRepository> _roleRepositoryMock;
        private GetAllRolesHandler _handler;

        [SetUp]
        public void Setup()
        {
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _handler = new GetAllRolesHandler(_roleRepositoryMock.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnMappedRoleDtos_WhenRolesExist()
        {
            // Arrange
            var roles = new List<IAMService.Domain.Entities.Role>
            {
                new IAMService.Domain.Entities.Role
                {
                    RoleCode = "ADMIN",
                    RoleName = "Administrator",
                    RoleDescription = "Full access",
                    RolePrivileges = new List<RolePrivilege>
                    {
                        new RolePrivilege
                        {
                            Privilege = new Privilege
                            {
                                PrivilegeId = 1,
                                PrivilegeName = "CreateUser",
                                Description = "Can create users"
                            }
                        },
                        new RolePrivilege
                        {
                            Privilege = new Privilege
                            {
                                PrivilegeId = 2,
                                PrivilegeName = "DeleteUser",
                                Description = "Can delete users"
                            }
                        }
                    }
                },
                new IAMService.Domain.Entities.Role
                {
                    RoleCode = "USER",
                    RoleName = "Standard User",
                    RoleDescription = "Basic access",
                    RolePrivileges = new List<RolePrivilege>()
                }
            };

            _roleRepositoryMock
                .Setup(r => r.GetAllRolesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(roles);

            var query = new GetAllRolesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _roleRepositoryMock.Verify(r => r.GetAllRolesAsync(It.IsAny<CancellationToken>()), Times.Once);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));

            var adminRole = result.First(r => r.RoleCode == "ADMIN");
            Assert.That(adminRole.RoleName, Is.EqualTo("Administrator"));
            Assert.That(adminRole.Privileges.Count, Is.EqualTo(2));
            Assert.That(adminRole.Privileges.Any(p => p.PrivilegeName == "CreateUser"), Is.True);

            var userRole = result.First(r => r.RoleCode == "USER");
            Assert.That(userRole.Privileges, Is.Empty);
        }

        [Test]
        public async Task Handle_ShouldReturnEmptyList_WhenNoRolesExist()
        {
            // Arrange
            _roleRepositoryMock
                .Setup(r => r.GetAllRolesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<IAMService.Domain.Entities.Role>());

            var query = new GetAllRolesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _roleRepositoryMock.Verify(r => r.GetAllRolesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }
    }
}
