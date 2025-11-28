using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PatientService.Application.Interfaces;
using PatientService.Application.UseCases.TestOrderUC.Commands.ModifyPatientTestOrder;
using PatientService.Domain.Entities.TestOrder;
using PatientService.Domain.Interfaces.TestOrderService;
using System.Globalization;

namespace TestProject;

[TestFixture]
public class ModifyPatientTestOrderHandlerTest
{
    private Mock<ITestOrderRepository> _testOrderRepositoryMock;
    private Mock<IApplicationDbContext> _dbContextMock;
    private Mock<ILogger<ModifyPatientTestOrderHandler>> _loggerMock;
    private Mock<DbSet<TestOrder>> _testOrderDbSetMock;
    private ModifyPatientTestOrderHandler _handler;

    [SetUp]
    public void Setup()
    {
        _testOrderRepositoryMock = new Mock<ITestOrderRepository>();
        _dbContextMock = new Mock<IApplicationDbContext>();
        _loggerMock = new Mock<ILogger<ModifyPatientTestOrderHandler>>();
        _testOrderDbSetMock = new Mock<DbSet<TestOrder>>();

        _handler = new ModifyPatientTestOrderHandler(
            _testOrderRepositoryMock.Object,
            _dbContextMock.Object,
            _loggerMock.Object
        );
    }

    [Test]
    public async Task Handle_ShouldReturnTrue_WhenNoChangesDetected()
    {
        // Arrange
        var testOrderId = Guid.NewGuid();

        var dob = DateTime.ParseExact("05/15/1990", "MM/dd/yyyy", CultureInfo.InvariantCulture)
                          .ToUniversalTime();

        var request = new ModifyPatientTestOrderCommand
        {
            TestOrderId = testOrderId,
            PatientName = "John Doe",
            DateOfBirth = "05/15/1990",
            Age = 33,
            Gender = "Male",
            Address = "123 Main St",
            PhoneNumber = "0123456789",
            UpdatedBy = Guid.NewGuid()
        };

        var testOrder = new TestOrder
        {
            TestOrderId = testOrderId,
            PatientName = "John Doe",
            DateOfBirth = dob,
            Age = 33,
            Gender = "Male",
            Address = "123 Main St",
            PhoneNumber = "0123456789",
            IsDeleted = false
        };

        _dbContextMock.Setup(d => d.Set<TestOrder>()).Returns(_testOrderDbSetMock.Object);

        _testOrderDbSetMock
            .Setup(d => d.FindAsync(new object[] { testOrderId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testOrder);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.True);
        _testOrderRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TestOrder>()), Times.Never);
    }

    [Test]
    public void Handle_ShouldThrow_WhenTestOrderNotFound()
    {
        var testOrderId = Guid.NewGuid();
        var request = new ModifyPatientTestOrderCommand
        {
            TestOrderId = testOrderId,
            PatientName = "John Doe",
            DateOfBirth = "05/15/1990",
            Age = 33,
            Gender = "Male",
            Address = "123 Main St",
            PhoneNumber = "0123456789",
            UpdatedBy = Guid.NewGuid()
        };

        _dbContextMock.Setup(d => d.Set<TestOrder>()).Returns(_testOrderDbSetMock.Object);

        _testOrderDbSetMock
            .Setup(d => d.FindAsync(new object[] { testOrderId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TestOrder?)null);

        var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(request, CancellationToken.None));
        Assert.That(ex.Message, Does.Contain($"Test order with ID {testOrderId} not found"));
    }

    [Test]
    public void Handle_ShouldThrow_WhenTestOrderIsDeleted()
    {
        var testOrderId = Guid.NewGuid();
        var request = new ModifyPatientTestOrderCommand
        {
            TestOrderId = testOrderId,
            PatientName = "John Doe",
            DateOfBirth = "05/15/1990",
            Age = 33,
            Gender = "Male",
            Address = "123 Main St",
            PhoneNumber = "0123456789",
            UpdatedBy = Guid.NewGuid()
        };

        var testOrder = new TestOrder
        {
            TestOrderId = testOrderId,
            IsDeleted = true
        };

        _dbContextMock.Setup(d => d.Set<TestOrder>()).Returns(_testOrderDbSetMock.Object);

        _testOrderDbSetMock
            .Setup(d => d.FindAsync(new object[] { testOrderId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testOrder);

        var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(request, CancellationToken.None));
        Assert.That(ex.Message, Does.Contain("Cannot modify a deleted test order"));
    }

    [Test]
    public void Handle_ShouldThrow_WhenInvalidDateFormat()
    {
        var testOrderId = Guid.NewGuid();
        var request = new ModifyPatientTestOrderCommand
        {
            TestOrderId = testOrderId,
            PatientName = "John Doe",
            DateOfBirth = "2023-05-01",
            Age = 33,
            Gender = "Male",
            Address = "123 Main St",
            PhoneNumber = "0123456789",
            UpdatedBy = Guid.NewGuid()
        };

        var testOrder = new TestOrder
        {
            TestOrderId = testOrderId,
            IsDeleted = false
        };

        _dbContextMock.Setup(d => d.Set<TestOrder>()).Returns(_testOrderDbSetMock.Object);

        _testOrderDbSetMock
            .Setup(d => d.FindAsync(new object[] { testOrderId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testOrder);

        var ex = Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(request, CancellationToken.None));
        Assert.That(ex.Message, Does.Contain("Invalid date of birth"));
    }

    [Test]
    public async Task Handle_ShouldUpdateSuccessfully()
    {
        var testOrderId = Guid.NewGuid();
        var request = new ModifyPatientTestOrderCommand
        {
            TestOrderId = testOrderId,
            PatientName = "John Doe Updated",
            DateOfBirth = "05/15/1990",
            Age = 40,
            Gender = "Male",
            Address = "123 Updated",
            PhoneNumber = "0999999999",
            UpdatedBy = Guid.NewGuid()
        };

        var dob = DateTime.ParseExact("05/15/1990", "MM/dd/yyyy", CultureInfo.InvariantCulture).ToUniversalTime();

        var testOrder = new TestOrder
        {
            TestOrderId = testOrderId,
            PatientName = "Old Name",
            DateOfBirth = dob,
            Age = 32,
            Gender = "Male",
            Address = "Old address",
            PhoneNumber = "0111111111",
            IsDeleted = false
        };

        _dbContextMock.Setup(d => d.Set<TestOrder>()).Returns(_testOrderDbSetMock.Object);

        _testOrderDbSetMock
            .Setup(d => d.FindAsync(new object[] { testOrderId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testOrder);

        _testOrderRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<TestOrder>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.That(result, Is.True);
        Assert.That(testOrder.PatientName, Is.EqualTo("John Doe Updated"));
        Assert.That(testOrder.Age, Is.EqualTo(40));
        _testOrderRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TestOrder>()), Times.Once);
    }
}
