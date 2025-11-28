using FluentAssertions;
using Moq;
using NUnit.Framework;
using PatientService.Application.UseCases.MedicalRecord.Commands.CreateMedicalRecord;
using PatientService.Domain.Entities.Patient;
using PatientService.Domain.Interfaces;
// Alias để tránh đụng tên namespace CreatePatient trong TestProject
using CreatePatientDto = PatientService.Application.Models.PatientDto.CreatePatient;

namespace TestProject.CreatePatient;

[TestFixture]
public class CreatePatientMedicalRecordHandlerTests
{
    private Mock<IPatientMedicalRecordRepository> _medicalRecordRepo = null!;
    private Mock<IPatientRepository> _patientRepo = null!;
    private CreatePatientMedicalRecordHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _medicalRecordRepo = new Mock<IPatientMedicalRecordRepository>(MockBehavior.Strict);
        _patientRepo = new Mock<IPatientRepository>(MockBehavior.Strict);

        _patientRepo
            .Setup(r => r.AddPatientAsync(It.IsAny<Patient>()))
            .Returns(Task.CompletedTask);

        _medicalRecordRepo
            .Setup(r => r.AddMedicalRecordAsync(It.IsAny<PatientMedicalRecord>()))
            .Returns(Task.CompletedTask);

        _handler = new CreatePatientMedicalRecordHandler(_medicalRecordRepo.Object, _patientRepo.Object);
    }

    [Test]
    public async Task Handle_ShouldCreatePatientAndRecord_WhenInputValid()
    {
        var cmd = new CreatePatientMedicalRecordCommad
        {
            CreatedBy = Guid.NewGuid(),
            Patient = new CreatePatientDto
            {
                FullName = "John Doe",
                DateOfBirth = "01/31/1990", // MM/dd/yyyy
                Gender = "Male",
                PhoneNumber = "0123456789",
                Email = "john@example.com",
                Address = "1 Main St",
                IdentifyNumber = "ID123",
                UserId = Guid.NewGuid(),
                LastTestDate = null
            }
        };

        var result = await _handler.Handle(cmd, CancellationToken.None);

        _patientRepo.Verify(r => r.AddPatientAsync(It.Is<Patient>(p =>
            p.FullName == cmd.Patient.FullName &&
            p.IdentifyNumber == cmd.Patient.IdentifyNumber &&
            p.UserId == cmd.Patient.UserId &&
            p.LastTestDate == null
        )), Times.Once);

        _medicalRecordRepo.Verify(r => r.AddMedicalRecordAsync(It.Is<PatientMedicalRecord>(m =>
            m.PatientId != Guid.Empty &&
            m.RecordId != Guid.Empty &&
            m.Version == 1 &&
            m.IsDeleted == false
        )), Times.Once);

        result.Should().NotBeNull();
        result.Patient.FullName.Should().Be("John Doe");
        result.RecordId.Should().NotBe(Guid.Empty);
        result.PatientId.Should().NotBe(Guid.Empty);
        result.CreatedAt.Should().BeAfter(DateTime.UtcNow.AddMinutes(-2));
        result.Patient.LastTestDate.Should().Be(string.Empty); // null -> ""
    }

    [Test]
    public async Task Handle_ShouldThrow_WhenDobInvalid()
    {
        var cmd = new CreatePatientMedicalRecordCommad
        {
            CreatedBy = Guid.NewGuid(),
            Patient = new CreatePatientDto
            {
                FullName = "Jane",
                DateOfBirth = "1990-01-31", // sai định dạng
                Gender = "Female",
                PhoneNumber = "0987654321"
            }
        };

        Func<Task> act = async () => await _handler.Handle(cmd, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>()
                 .WithMessage("*Invalid date of birth*");
    }
}