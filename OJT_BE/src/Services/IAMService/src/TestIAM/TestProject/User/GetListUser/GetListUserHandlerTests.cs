using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IAMService.Application.Models.User;
using IAMService.Application.UseCases.User.Queries.GetListUser;
using IAMService.Domain.Entities;
using IAMService.Domain.Interfaces;
using Moq;
using NUnit.Framework;

namespace TestIAM.TestProject.User.GetListUser
{
    /// <summary>
    /// Unit tests for the <see cref="GetListUserHandler"/> class.
    /// Verifies the behavior of the handler when retrieving user lists.
    /// </summary>
    [TestFixture]
    public class GetListUserHandlerTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private GetListUserHandler _handler;

        /// <summary>
        /// Initializes test dependencies before each test runs.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetListUserHandler(_userRepositoryMock.Object, _mapperMock.Object);
        }

        /// <summary>
        /// Verifies that <see cref="GetListUserHandler.Handle"/> returns a list of users
        /// when users exist in the repository.
        /// </summary>
        [Test]
        public async Task Handle_ReturnsUserList_WhenUsersExist()
        {
            // Arrange
            var users = new List<IAMService.Domain.Entities.User>
            {
                new IAMService.Domain.Entities.User
                {
                    UserId = System.Guid.NewGuid(),
                    FullName = "Test User",
                    Email = "test@example.com",
                    PhoneNumber = "0123456789",
                    IdentifyNumber = "123456789",
                    Gender = "Male",
                    Age = 25,
                    Address = "Test Address",
                    DateOfBirth = System.DateTime.Now,
                    RoleCode = "ADMIN",
                    Password = "password",
                    IsActive = true,
                    Role = null
                }
            };

            var userDTOs = new List<UserDTO>
            {
                new UserDTO
                {
                    UserId = users[0].UserId,
                    FullName = users[0].FullName,
                    Email = users[0].Email,
                    PhoneNumber = users[0].PhoneNumber,
                    IdentifyNumber = users[0].IdentifyNumber,
                    Gender = users[0].Gender,
                    Age = users[0].Age,
                    Address = users[0].Address,
                    DateOfBirth = users[0].DateOfBirth.ToString("yyyy-MM-dd"),
                    Role = null
                }
            };

            // Mock repository and mapper responses
            _userRepositoryMock.Setup(r => r.GetListUser())
                .ReturnsAsync((IEnumerable<IAMService.Domain.Entities.User>)users);

            _mapperMock.Setup(m => m.Map<IEnumerable<UserDTO>>(users))
                .Returns(userDTOs);

            var query = new GetListUserQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Exactly(1).Items);
            Assert.That(result, Has.All.TypeOf<UserDTO>());
            Assert.That(((List<UserDTO>)result)[0].FullName, Is.EqualTo("Test User"));
        }

        /// <summary>
        /// Verifies that <see cref="GetListUserHandler.Handle"/> returns an empty list
        /// when no users exist in the repository.
        /// </summary>
        [Test]
        public async Task Handle_ReturnsEmptyList_WhenNoUsersExist()
        {
            // Arrange
            var users = new List<IAMService.Domain.Entities.User>();
            var userDTOs = new List<UserDTO>();

            _userRepositoryMock.Setup(r => r.GetListUser())
                .ReturnsAsync((IEnumerable<IAMService.Domain.Entities.User>)users);

            _mapperMock.Setup(m => m.Map<IEnumerable<UserDTO>>(users))
                .Returns(userDTOs);

            var query = new GetListUserQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        /// <summary>
        /// Verifies that <see cref="GetListUserHandler.Handle"/> throws an exception
        /// when the repository layer fails to retrieve data.
        /// </summary>
        [Test]
        public void Handle_ThrowsException_WhenRepositoryFails()
        {
            // Arrange
            _userRepositoryMock.Setup(r => r.GetListUser())
                .ThrowsAsync(new System.Exception("Repository error"));

            var query = new GetListUserQuery();

            // Act & Assert
            Assert.ThrowsAsync<System.Exception>(async () => await _handler.Handle(query, CancellationToken.None));
        }
    }
}
