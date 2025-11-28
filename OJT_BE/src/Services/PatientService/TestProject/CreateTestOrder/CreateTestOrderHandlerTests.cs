using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PatientService.Application.UseCases.TestOrderUC.TestOrders.Commands.CreateTestOrder;
using PatientService.Domain.Entities.Patient;
using PatientService.Domain.Entities.TestOrder;
using PatientService.Domain.Interfaces;
using PatientService.Domain.Interfaces.TestOrderService;
using CreatePatientDto = PatientService.Application.Models.PatientDto.CreatePatient;

namespace TestProject.CreateTestOrder;

[TestFixture]
public class CreateTestOrderHandlerTests
{
    private Mock<ITestOrderRepository> _testOrderRepoMock = null!;
    private Mock<IPatientRepository> _patientRepoMock = null!;
    private Mock<IPatientMedicalRecordRepository> _medicalRecordRepoMock = null!;
    private Mock<IAuditLogRepository> _auditLogRepoMock = null!;
    private CreateTestOrderHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _testOrderRepoMock = new Mock<ITestOrderRepository>(MockBehavior.Strict);
        _patientRepoMock = new Mock<IPatientRepository>(MockBehavior.Strict);
        _medicalRecordRepoMock = new Mock<IPatientMedicalRecordRepository>(MockBehavior.Strict);
        _auditLogRepoMock = new Mock<IAuditLogRepository>(MockBehavior.Strict);

        // Setup default behaviors
        _patientRepoMock
            .Setup(r => r.AddPatientAsync(It.IsAny<Patient>()))
            .Returns(Task.CompletedTask);

        _testOrderRepoMock
            .Setup(r => r.AddTestOrderAsync(It.IsAny<TestOrder>()))
            .Returns(Task.CompletedTask);

        _medicalRecordRepoMock
            .Setup(r => r.AddMedicalRecordAsync(It.IsAny<PatientMedicalRecord>()))
            .Returns(Task.CompletedTask);

        _auditLogRepoMock
            .Setup(r => r.AddLogAsync(It.IsAny<TestOrderAuditLog>()))
            .Returns(Task.CompletedTask);

        _testOrderRepoMock
            .Setup(r => r.SaveChangeAsync())
            .Returns(Task.CompletedTask);

        _handler = new CreateTestOrderHandler(
            _testOrderRepoMock.Object,
            _patientRepoMock.Object,
            _medicalRecordRepoMock.Object,
            _auditLogRepoMock.Object);
    }

    [Test]
    public async Task Handle_ShouldCreatePatientAndTestOrder_WhenInputValid()
    {
        // Arrange
        var command = new CreateTestOrderCommand
        {
            CreatedBy = "user123",
            Patient = new CreatePatientDto
            {
                FullName = "John Doe",
                DateOfBirth = "01/15/1990", // MM/dd/yyyy
                Gender = "Male",
                PhoneNumber = "0123456789",
                Email = "john@example.com",
                Address = "123 Main St",
                IdentifyNumber = "ID123456",
                UserId = Guid.NewGuid(),
                LastTestDate = null
            }
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _patientRepoMock.Verify(r => r.AddPatientAsync(It.Is<Patient>(p =>
            p.FullName == "John Doe" &&
            p.Gender == "Male" &&
            p.PhoneNumber == "0123456789" &&
            p.Email == "john@example.com" &&
            p.IdentifyNumber == "ID123456" &&
            p.IsDeleted == false &&
            p.LastTestDate == null
        )), Times.Once);

        _medicalRecordRepoMock.Verify(r => r.AddMedicalRecordAsync(It.Is<PatientMedicalRecord>(m =>
            m.RecordId != Guid.Empty &&
            m.PatientId != Guid.Empty &&
            m.Version == 1 &&
            m.IsDeleted == false &&
            m.UpdatedBy == null
        )), Times.Once);

        _testOrderRepoMock.Verify(r => r.AddTestOrderAsync(It.Is<TestOrder>(t =>
            t.TestOrderId != Guid.Empty &&
            t.PatientId != Guid.Empty &&
            t.RecordId != Guid.Empty &&
            t.PatientName == "John Doe" &&
            t.Status == "Pending" &&
            t.CreatedBy == "user123" &&
            t.Gender == "Male" &&
            t.PhoneNumber == "0123456789" &&
            t.IsDeleted == false
        )), Times.Once);

        _auditLogRepoMock.Verify(r => r.AddLogAsync(It.Is<TestOrderAuditLog>(log =>
            log.TestOrderId != Guid.Empty &&
            log.UserId == "user123" &&
            log.ActionType == "CREATE_TEST_ORDER" &&
            log.ChangedFields.Contains("John Doe")
        )), Times.Once);

        _testOrderRepoMock.Verify(r => r.SaveChangeAsync(), Times.Once);
    }

    [Test]
    public async Task Handle_ShouldCreatePatientWithLastTestDate_WhenLastTestDateProvided()
    {
        // Arrange
        var command = new CreateTestOrderCommand
        {
            CreatedBy = "user456",
            Patient = new CreatePatientDto
            {
                FullName = "Jane Smith",
                DateOfBirth = "05/20/1985",
                Gender = "Female",
                PhoneNumber = "0987654321",
                Email = "jane@example.com",
                Address = "456 Oak Ave",
                IdentifyNumber = "ID789012",
                UserId = Guid.NewGuid(),
                LastTestDate = "12/01/2024" // Has previous test date
            }
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _patientRepoMock.Verify(r => r.AddPatientAsync(It.Is<Patient>(p =>
            p.FullName == "Jane Smith" &&
            p.LastTestDate != null
        )), Times.Once);

        _testOrderRepoMock.Verify(r => r.AddTestOrderAsync(It.IsAny<TestOrder>()), Times.Once);
        _auditLogRepoMock.Verify(r => r.AddLogAsync(It.IsAny<TestOrderAuditLog>()), Times.Once);
        _testOrderRepoMock.Verify(r => r.SaveChangeAsync(), Times.Once);
    }

    [Test]
    public void Handle_ShouldThrowArgumentException_WhenDateOfBirthInvalid()
    {
        // Arrange
        var command = new CreateTestOrderCommand
        {
            CreatedBy = "user123",
            Patient = new CreatePatientDto
            {
                FullName = "Invalid User",
                DateOfBirth = "1990-01-15", // Wrong format (should be MM/dd/yyyy)
                Gender = "Male",
                PhoneNumber = "0123456789",
                Email = "test@example.com",
                Address = "Test Address",
                IdentifyNumber = "TEST123",
                UserId = Guid.NewGuid()
            }
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        act.Should().ThrowAsync<ArgumentException>()
           .WithMessage("*Invalid date of birth*");
    }

    [Test]
    public void Handle_ShouldThrowArgumentException_WhenLastTestDateInvalid()
    {
        // Arrange
        var command = new CreateTestOrderCommand
        {
            CreatedBy = "user123",
            Patient = new CreatePatientDto
            {
                FullName = "Test User",
                DateOfBirth = "01/15/1990",
                Gender = "Male",
                PhoneNumber = "0123456789",
                Email = "test@example.com",
                Address = "Test Address",
                IdentifyNumber = "TEST123",
                UserId = Guid.NewGuid(),
                LastTestDate = "2024-12-01" // Wrong format
            }
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        act.Should().ThrowAsync<ArgumentException>()
           .WithMessage("*Invalid last test date*");
    }

    [Test]
    public async Task Handle_ShouldCalculateAgeCorrectly()
    {
        // Arrange
        var command = new CreateTestOrderCommand
        {
            CreatedBy = "user123",
            Patient = new CreatePatientDto
            {
                FullName = "Age Test",
                DateOfBirth = "01/01/1990", // Should be ~34 years old
                Gender = "Male",
                PhoneNumber = "0123456789",
                Email = "test@example.com",
                Address = "Test Address",
                IdentifyNumber = "TEST123",
                UserId = Guid.NewGuid()
            }
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _testOrderRepoMock.Verify(r => r.AddTestOrderAsync(It.Is<TestOrder>(t =>
            t.Age >= 30 && t.Age <= 40 // Age should be approximately 34
        )), Times.Once);
    }

    [Test]
    public async Task Handle_ShouldSetCorrectTimestamps()
    {
        // Arrange
        var beforeTest = DateTime.UtcNow.AddSeconds(-1);
        var command = new CreateTestOrderCommand
        {
            CreatedBy = "user123",
            Patient = new CreatePatientDto
            {
                FullName = "Timestamp Test",
                DateOfBirth = "01/01/1990",
                Gender = "Male",
                PhoneNumber = "0123456789",
                Email = "test@example.com",
                Address = "Test Address",
                IdentifyNumber = "TEST123",
                UserId = Guid.NewGuid()
            }
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);
        var afterTest = DateTime.UtcNow.AddSeconds(1);

        // Assert
        _patientRepoMock.Verify(r => r.AddPatientAsync(It.Is<Patient>(p =>
            p.CreatedAt >= beforeTest &&
            p.CreatedAt <= afterTest &&
            p.UpdatedAt >= beforeTest &&
            p.UpdatedAt <= afterTest
        )), Times.Once);

        _testOrderRepoMock.Verify(r => r.AddTestOrderAsync(It.Is<TestOrder>(t =>
            t.CreatedAt >= beforeTest &&
            t.CreatedAt <= afterTest
        )), Times.Once);

        _medicalRecordRepoMock.Verify(r => r.AddMedicalRecordAsync(It.Is<PatientMedicalRecord>(m =>
            m.CreatedAt >= beforeTest &&
            m.CreatedAt <= afterTest &&
            m.UpdatedAt == null &&
            m.UpdatedBy == null
        )), Times.Once);
    }

    [Test]
    public async Task Handle_ShouldLinkPatientTestOrderAndMedicalRecord()
    {
        // Arrange
        var command = new CreateTestOrderCommand
        {
            CreatedBy = "user123",
            Patient = new CreatePatientDto
            {
                FullName = "Link Test",
                DateOfBirth = "01/01/1990",
                Gender = "Male",
                PhoneNumber = "0123456789",
                Email = "test@example.com",
                Address = "Test Address",
                IdentifyNumber = "TEST123",
                UserId = Guid.NewGuid()
            }
        };

        Guid capturedPatientId = Guid.Empty;
        Guid capturedRecordId = Guid.Empty;

        _patientRepoMock
            .Setup(r => r.AddPatientAsync(It.IsAny<Patient>()))
            .Callback<Patient>(p => capturedPatientId = p.PatientId)
            .Returns(Task.CompletedTask);

        _medicalRecordRepoMock
            .Setup(r => r.AddMedicalRecordAsync(It.IsAny<PatientMedicalRecord>()))
            .Callback<PatientMedicalRecord>(m =>
            {
                capturedRecordId = m.RecordId;
                // Verify PatientId matches
                m.PatientId.Should().Be(capturedPatientId);
            })
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert - TestOrder should have correct PatientId and RecordId
        _testOrderRepoMock.Verify(r => r.AddTestOrderAsync(It.Is<TestOrder>(t =>
            t.PatientId == capturedPatientId &&
            t.RecordId == capturedRecordId
        )), Times.Once);
    }

    [Test]
    public async Task Handle_ShouldCreateAuditLogWithCorrectDetails()
    {
        // Arrange
        var command = new CreateTestOrderCommand
        {
            CreatedBy = "user123",
            Patient = new CreatePatientDto
            {
                FullName = "Audit Test",
                DateOfBirth = "01/01/1990",
                Gender = "Male",
                PhoneNumber = "0123456789",
                Email = "audit@example.com",
                Address = "Audit Address",
                IdentifyNumber = "AUDIT123",
                UserId = Guid.NewGuid()
            }
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _auditLogRepoMock.Verify(r => r.AddLogAsync(It.Is<TestOrderAuditLog>(log =>
            log.UserId == "user123" &&
            log.ActionType == "CREATE_TEST_ORDER" &&
            log.ChangedFields.Contains("Audit Test") &&
            log.ChangedFields.Contains("Status: Pending")
        )), Times.Once);
    }
}
