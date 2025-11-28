using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using PatientService.Application.Interfaces;
using PatientService.Application.UseCases.TestOrderUC.ViewPatientTestOrder.Queries;
using PatientService.Domain.Interfaces.TestOrderService;

namespace UnitTests.TestOrder
{
    [TestFixture]
    public class ViewPatientTestOrdersHandlerTests
    {
        private Mock<ITestOrderRepository> _repositoryMock = null!;
        private Mock<IIamUserClient> _iamUserClientMock = null!;
        private ViewPatientTestOrdersHandler _handler = null!;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<ITestOrderRepository>();
            _iamUserClientMock = new Mock<IIamUserClient>();

            // Setup default behavior for IIamUserClient
            _iamUserClientMock
                .Setup(client => client.GetUserFullNameAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid id, CancellationToken ct) => $"User-{id}");

            _handler = new ViewPatientTestOrdersHandler(_repositoryMock.Object, _iamUserClientMock.Object);
        }

        [Test]
        public async Task Handle_NoOrders_ReturnsNoData()
        {
            // Arrange
            var emptyList = new List<PatientService.Domain.Entities.TestOrder.TestOrder>();

            _repositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _handler.Handle(new ViewPatientTestOrdersQuery(), CancellationToken.None);

            // Assert
            result.Items.Should().BeEmpty();
            result.Message.Should().Be("No Data");
        }

        [Test]
        public async Task Handle_HasOrders_ReturnsMappedList()
        {
            // Arrange
            var order = new PatientService.Domain.Entities.TestOrder.TestOrder
            {
                TestOrderId = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                PatientName = "John Doe",
                Age = 30,
                Gender = "Male",
                PhoneNumber = "123456789",
                Status = "Completed",
                CreatedAt = DateTime.UtcNow
            };

            var list = new List<PatientService.Domain.Entities.TestOrder.TestOrder> { order };

            _repositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(list);

            // Act
            var result = await _handler.Handle(new ViewPatientTestOrdersQuery(), CancellationToken.None);

            // Assert
            result.Items.Should().HaveCount(1);
            result.Items[0].PatientName.Should().Be("John Doe");
            result.Items[0].Status.Should().Be("Completed");
            result.Message.Should().Be("Success");
        }

        [Test]
        public async Task Handle_MultipleOrders_ReturnsSortedList()
        {
            // Arrange
            var order1 = new PatientService.Domain.Entities.TestOrder.TestOrder
            {
                TestOrderId = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                PatientName = "A",
                CreatedAt = DateTime.UtcNow.AddHours(-2)
            };

            var order2 = new PatientService.Domain.Entities.TestOrder.TestOrder
            {
                TestOrderId = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                PatientName = "B",
                CreatedAt = DateTime.UtcNow
            };

            var list = new List<PatientService.Domain.Entities.TestOrder.TestOrder> { order1, order2 };

            _repositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(list);

            // Act
            var result = await _handler.Handle(new ViewPatientTestOrdersQuery(), CancellationToken.None);

            // Assert
            result.Items.Should().HaveCount(2);
            result.Items[0].PatientName.Should().Be("B");  // newest first
            result.Items[1].PatientName.Should().Be("A");
        }

        [Test]
        public async Task Handle_RepositoryReturnsNull_ReturnsNoData()
        {
            // Arrange
            _repositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((List<PatientService.Domain.Entities.TestOrder.TestOrder>)null!);

            // Act
            var result = await _handler.Handle(new ViewPatientTestOrdersQuery(), CancellationToken.None);

            // Assert
            result.Items.Should().BeEmpty();
            result.Message.Should().Be("No Data");
        }

        [Test]
        public async Task Handle_OrderMapping_CopiesAllFieldsCorrectly()
        {
            // Arrange
            var order = new PatientService.Domain.Entities.TestOrder.TestOrder
            {
                TestOrderId = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                PatientName = "Jane Doe",
                Age = 45,
                Gender = "Female",
                PhoneNumber = "999999999",
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "staff01",
                RunBy = "lab01",
                RunOn = DateTime.UtcNow.AddMinutes(-10)
            };

            var list = new List<PatientService.Domain.Entities.TestOrder.TestOrder> { order };

            _repositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(list);

            // Act
            var result = await _handler.Handle(new ViewPatientTestOrdersQuery(), CancellationToken.None);

            // Assert
            var dto = result.Items[0];

            dto.TestOrderId.Should().Be(order.TestOrderId);
            dto.PatientId.Should().Be(order.PatientId);
            dto.PatientName.Should().Be(order.PatientName);
            dto.Age.Should().Be(order.Age);
            dto.Gender.Should().Be(order.Gender);
            dto.PhoneNumber.Should().Be(order.PhoneNumber);
            dto.Status.Should().Be(order.Status);
            dto.CreatedBy.Should().Be(order.CreatedBy);
            dto.RunBy.Should().Be(order.RunBy);
        }

        [Test]
        public async Task Handle_WithUserIds_ShouldCallIamUserClientToGetUserNames()
        {
            // Arrange
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();

            var order1 = new PatientService.Domain.Entities.TestOrder.TestOrder
            {
                TestOrderId = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                PatientName = "Patient 1",
                CreatedBy = userId1.ToString(),   // ⭐ MUST HAVE
                CreatedAt = DateTime.UtcNow
            };

            var order2 = new PatientService.Domain.Entities.TestOrder.TestOrder
            {
                TestOrderId = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                PatientName = "Patient 2",
                CreatedBy = userId2.ToString(),   // ⭐ MUST HAVE
                CreatedAt = DateTime.UtcNow
            };

            var list = new List<PatientService.Domain.Entities.TestOrder.TestOrder> { order1, order2 };

            _repositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(list);

            _iamUserClientMock
                .Setup(client => client.GetUserFullNameAsync(userId1, It.IsAny<CancellationToken>()))
                .ReturnsAsync("John Smith");

            _iamUserClientMock
                .Setup(client => client.GetUserFullNameAsync(userId2, It.IsAny<CancellationToken>()))
                .ReturnsAsync("Jane Doe");

            // Act
            var result = await _handler.Handle(new ViewPatientTestOrdersQuery(), CancellationToken.None);

            // Assert
            result.Items.Should().HaveCount(2);

            // IAM must be called exactly twice (once per GUID)
            _iamUserClientMock.Verify(
                client => client.GetUserFullNameAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Exactly(2)
            );
        }


        [Test]
        public async Task Handle_IamUserClientReturnsNull_ShouldHandleGracefully()
        {
            // Arrange
            var order = new PatientService.Domain.Entities.TestOrder.TestOrder
            {
                TestOrderId = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                PatientName = "Test Patient",
                CreatedAt = DateTime.UtcNow
            };

            var list = new List<PatientService.Domain.Entities.TestOrder.TestOrder> { order };

            _repositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(list);

            _iamUserClientMock
                .Setup(client => client.GetUserFullNameAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((string?)null);

            // Act
            var result = await _handler.Handle(new ViewPatientTestOrdersQuery(), CancellationToken.None);

            // Assert
            result.Items.Should().HaveCount(1);
            result.Message.Should().Be("Success");
        }
    }
}
