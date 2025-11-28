using FluentAssertions;
using Moq;
using NUnit.Framework;
using PatientService.Application.Models.PatientMedicalRecordDto;
using PatientService.Application.UseCases.MedicalRecord.Queries.ViewMedicalRecordDetail;
using PatientService.Domain.Entities.Patient;
using PatientService.Domain.Entities.TestOrder;
using PatientService.Domain.Interfaces;
using PatientService.Domain.Interfaces.TestOrderService;

namespace TestProject.ViewMedicalRecordDetail;

[TestFixture]
public class ViewMedicalRecordDetailHandlerTests
{
    private Mock<IPatientMedicalRecordRepository> _medicalRecordRepositoryMock = null!;
    private Mock<IPatientRepository> _patientRepositoryMock = null!;
    private Mock<ITestOrderRepository> _testOrderRepositoryMock = null!;
    private ViewMedicalRecordDetailHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _medicalRecordRepositoryMock = new Mock<IPatientMedicalRecordRepository>();
        _patientRepositoryMock = new Mock<IPatientRepository>();
        _testOrderRepositoryMock = new Mock<ITestOrderRepository>();

        _handler = new ViewMedicalRecordDetailHandler(
            _medicalRecordRepositoryMock.Object,
            _patientRepositoryMock.Object,
            _testOrderRepositoryMock.Object);
    }

    [Test]
    public async Task Handle_ShouldReturnMedicalRecordDetail_WhenPatientAndRecordExist()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var recordId = Guid.NewGuid();
        var request = new ViewMedicalRecordDetailQuery { PatientId = patientId };

        var medicalRecord = new PatientMedicalRecord
        {
            RecordId = recordId,
            PatientId = patientId,
            CreatedAt = DateTime.UtcNow.AddDays(-7),
            Version = 1,
            IsDeleted = false
        };

        var patient = new Patient
        {
            PatientId = patientId,
            FullName = "John Doe",
            DateOfBirth = new DateTime(1990, 1, 15),
            PhoneNumber = "0123456789",
            Gender = "Male",
            Email = "john@example.com",
            Address = "123 Main St",
            IsDeleted = false
        };

        var testOrders = new List<TestOrder>
        {
            new TestOrder
            {
                TestOrderId = Guid.NewGuid(),
                RecordId = recordId,
                PatientId = patientId,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                CreatedBy = "user123"
            },
            new TestOrder
            {
                TestOrderId = Guid.NewGuid(),
                RecordId = recordId,
                PatientId = patientId,
                Status = "Completed",
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                CreatedBy = "user456"
            }
        };

        _medicalRecordRepositoryMock
            .Setup(r => r.GetLatestMedicalRecordByPatientIdAsync(patientId))
            .ReturnsAsync(medicalRecord);

        _patientRepositoryMock
            .Setup(r => r.GetPatientByIdAsync(patientId))
            .ReturnsAsync(patient);

        _testOrderRepositoryMock
            .Setup(r => r.GetTestOrdersByMedicalRecordIdAsync(recordId))
            .ReturnsAsync(testOrders);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.PatientId.Should().Be(patientId);
        result.PatientName.Should().Be("John Doe");
        result.DateOfBirth.Should().Be(new DateTime(1990, 1, 15));
        result.PhoneNumber.Should().Be("0123456789");
        result.TestOrders.Should().HaveCount(2);
        result.TestOrders[0].TestOrderId.Should().Be(testOrders[0].TestOrderId);
        result.TestOrders[0].Status.Should().Be("Pending");
        result.TestOrders[1].Status.Should().Be("Completed");

        _medicalRecordRepositoryMock.Verify(
            r => r.GetLatestMedicalRecordByPatientIdAsync(patientId),
            Times.Once);
        _patientRepositoryMock.Verify(
            r => r.GetPatientByIdAsync(patientId),
            Times.Once);
        _testOrderRepositoryMock.Verify(
            r => r.GetTestOrdersByMedicalRecordIdAsync(recordId),
            Times.Once);
    }

    [Test]
    public void Handle_ShouldThrowKeyNotFoundException_WhenMedicalRecordNotFound()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var request = new ViewMedicalRecordDetailQuery { PatientId = patientId };

        _medicalRecordRepositoryMock
            .Setup(r => r.GetLatestMedicalRecordByPatientIdAsync(patientId))
            .ReturnsAsync((PatientMedicalRecord?)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(request, CancellationToken.None));

        ex.Message.Should().Contain($"Medical record for patient ID {patientId} not found.");

        _medicalRecordRepositoryMock.Verify(
            r => r.GetLatestMedicalRecordByPatientIdAsync(patientId),
            Times.Once);
        _patientRepositoryMock.Verify(
            r => r.GetPatientByIdAsync(It.IsAny<Guid>()),
            Times.Never);
        _testOrderRepositoryMock.Verify(
            r => r.GetTestOrdersByMedicalRecordIdAsync(It.IsAny<Guid>()),
            Times.Never);
    }

    [Test]
    public void Handle_ShouldThrowKeyNotFoundException_WhenPatientNotFound()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var recordId = Guid.NewGuid();
        var request = new ViewMedicalRecordDetailQuery { PatientId = patientId };

        var medicalRecord = new PatientMedicalRecord
        {
            RecordId = recordId,
            PatientId = patientId,
            CreatedAt = DateTime.UtcNow.AddDays(-7),
            Version = 1
        };

        _medicalRecordRepositoryMock
            .Setup(r => r.GetLatestMedicalRecordByPatientIdAsync(patientId))
            .ReturnsAsync(medicalRecord);

        _patientRepositoryMock
            .Setup(r => r.GetPatientByIdAsync(patientId))
            .ReturnsAsync((Patient?)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(request, CancellationToken.None));

        ex.Message.Should().Contain($"Patient with ID {patientId} not found.");

        _medicalRecordRepositoryMock.Verify(
            r => r.GetLatestMedicalRecordByPatientIdAsync(patientId),
            Times.Once);
        _patientRepositoryMock.Verify(
            r => r.GetPatientByIdAsync(patientId),
            Times.Once);
        _testOrderRepositoryMock.Verify(
            r => r.GetTestOrdersByMedicalRecordIdAsync(It.IsAny<Guid>()),
            Times.Never);
    }

    [Test]
    public async Task Handle_ShouldReturnEmptyTestOrders_WhenNoTestOrdersExist()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var recordId = Guid.NewGuid();
        var request = new ViewMedicalRecordDetailQuery { PatientId = patientId };

        var medicalRecord = new PatientMedicalRecord
        {
            RecordId = recordId,
            PatientId = patientId,
            CreatedAt = DateTime.UtcNow.AddDays(-7),
            Version = 1
        };

        var patient = new Patient
        {
            PatientId = patientId,
            FullName = "Jane Smith",
            DateOfBirth = new DateTime(1985, 5, 20),
            PhoneNumber = "0987654321",
            Gender = "Female",
            Email = "jane@example.com"
        };

        var emptyTestOrders = new List<TestOrder>();

        _medicalRecordRepositoryMock
            .Setup(r => r.GetLatestMedicalRecordByPatientIdAsync(patientId))
            .ReturnsAsync(medicalRecord);

        _patientRepositoryMock
            .Setup(r => r.GetPatientByIdAsync(patientId))
            .ReturnsAsync(patient);

        _testOrderRepositoryMock
            .Setup(r => r.GetTestOrdersByMedicalRecordIdAsync(recordId))
            .ReturnsAsync(emptyTestOrders);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.PatientId.Should().Be(patientId);
        result.PatientName.Should().Be("Jane Smith");
        result.TestOrders.Should().BeEmpty();

        _testOrderRepositoryMock.Verify(
            r => r.GetTestOrdersByMedicalRecordIdAsync(recordId),
            Times.Once);
    }

    [Test]
    public async Task Handle_ShouldMapTestOrdersCorrectly_WhenMultipleStatusesExist()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var recordId = Guid.NewGuid();
        var request = new ViewMedicalRecordDetailQuery { PatientId = patientId };

        var medicalRecord = new PatientMedicalRecord
        {
            RecordId = recordId,
            PatientId = patientId,
            CreatedAt = DateTime.UtcNow
        };

        var patient = new Patient
        {
            PatientId = patientId,
            FullName = "Test Patient",
            DateOfBirth = new DateTime(1995, 3, 10),
            PhoneNumber = "0111222333"
        };

        var orderDate1 = DateTime.UtcNow.AddDays(-10);
        var orderDate2 = DateTime.UtcNow.AddDays(-5);
        var orderDate3 = DateTime.UtcNow.AddDays(-1);

        var testOrders = new List<TestOrder>
        {
            new TestOrder
            {
                TestOrderId = Guid.NewGuid(),
                RecordId = recordId,
                Status = "Pending",
                CreatedAt = orderDate1,
                CreatedBy = "user1"
            },
            new TestOrder
            {
                TestOrderId = Guid.NewGuid(),
                RecordId = recordId,
                Status = "InProgress",
                CreatedAt = orderDate2,
                CreatedBy = "user2"
            },
            new TestOrder
            {
                TestOrderId = Guid.NewGuid(),
                RecordId = recordId,
                Status = "Completed",
                CreatedAt = orderDate3,
                CreatedBy = "user3"
            }
        };

        _medicalRecordRepositoryMock
            .Setup(r => r.GetLatestMedicalRecordByPatientIdAsync(patientId))
            .ReturnsAsync(medicalRecord);

        _patientRepositoryMock
            .Setup(r => r.GetPatientByIdAsync(patientId))
            .ReturnsAsync(patient);

        _testOrderRepositoryMock
            .Setup(r => r.GetTestOrdersByMedicalRecordIdAsync(recordId))
            .ReturnsAsync(testOrders);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.TestOrders.Should().HaveCount(3);
        result.TestOrders[0].OrderDate.Should().Be(orderDate1);
        result.TestOrders[0].Status.Should().Be("Pending");
        result.TestOrders[1].OrderDate.Should().Be(orderDate2);
        result.TestOrders[1].Status.Should().Be("InProgress");
        result.TestOrders[2].OrderDate.Should().Be(orderDate3);
        result.TestOrders[2].Status.Should().Be("Completed");
    }

    [Test]
    public async Task Handle_ShouldHandleCancellationToken_WhenProvided()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var recordId = Guid.NewGuid();
        var request = new ViewMedicalRecordDetailQuery { PatientId = patientId };
        var cancellationTokenSource = new CancellationTokenSource();

        var medicalRecord = new PatientMedicalRecord
        {
            RecordId = recordId,
            PatientId = patientId
        };

        var patient = new Patient
        {
            PatientId = patientId,
            FullName = "Test User",
            DateOfBirth = DateTime.UtcNow.AddYears(-30),
            PhoneNumber = "0123456789"
        };

        var testOrders = new List<TestOrder>();

        _medicalRecordRepositoryMock
            .Setup(r => r.GetLatestMedicalRecordByPatientIdAsync(patientId))
            .ReturnsAsync(medicalRecord);

        _patientRepositoryMock
            .Setup(r => r.GetPatientByIdAsync(patientId))
            .ReturnsAsync(patient);

        _testOrderRepositoryMock
            .Setup(r => r.GetTestOrdersByMedicalRecordIdAsync(recordId))
            .ReturnsAsync(testOrders);

        // Act
        var result = await _handler.Handle(request, cancellationTokenSource.Token);

        // Assert
        result.Should().NotBeNull();
        result.PatientId.Should().Be(patientId);
    }

    [Test]
    public async Task Handle_ShouldReturnCorrectPatientInformation_WithAllProperties()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var recordId = Guid.NewGuid();
        var dateOfBirth = new DateTime(1988, 7, 15);
        var request = new ViewMedicalRecordDetailQuery { PatientId = patientId };

        var medicalRecord = new PatientMedicalRecord
        {
            RecordId = recordId,
            PatientId = patientId
        };

        var patient = new Patient
        {
            PatientId = patientId,
            FullName = "Alice Johnson",
            DateOfBirth = dateOfBirth,
            PhoneNumber = "0999888777",
            Gender = "Female",
            Email = "alice@example.com",
            Address = "789 Pine St",
            IdentifyNumber = "ID999888"
        };

        var testOrders = new List<TestOrder>();

        _medicalRecordRepositoryMock
            .Setup(r => r.GetLatestMedicalRecordByPatientIdAsync(patientId))
            .ReturnsAsync(medicalRecord);

        _patientRepositoryMock
            .Setup(r => r.GetPatientByIdAsync(patientId))
            .ReturnsAsync(patient);

        _testOrderRepositoryMock
            .Setup(r => r.GetTestOrdersByMedicalRecordIdAsync(recordId))
            .ReturnsAsync(testOrders);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.PatientId.Should().Be(patientId);
        result.PatientName.Should().Be("Alice Johnson");
        result.DateOfBirth.Should().Be(dateOfBirth);
        result.PhoneNumber.Should().Be("0999888777");
    }
}