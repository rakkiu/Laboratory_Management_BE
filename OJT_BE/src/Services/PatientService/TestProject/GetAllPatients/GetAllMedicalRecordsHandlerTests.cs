using FluentAssertions;
using Moq;
using NUnit.Framework;
using PatientService.Application.UseCases.MedicalRecord.Queries.GetAllMedicalRecord;
using PatientService.Domain.Entities.Patient;

using PatientService.Domain.Interfaces;

namespace TestProject.GetAllPatients;

[TestFixture]
public class GetAllMedicalRecordsHandlerTests
{
    private Mock<IPatientRepository> _patientRepo = null!;
    private GetAllMedicalRecordsHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _patientRepo = new Mock<IPatientRepository>(MockBehavior.Strict);
        _handler = new GetAllMedicalRecordsHandler(_patientRepo.Object);
    }

    [Test]
    public async Task Handle_ShouldReturnMappedList()
    {
        // Patients must have MedicalRecords with IsDeleted = false
        var patients = new List<Patient>
        {
            new()
            {
                PatientId = Guid.NewGuid(),
                FullName = "Alice",
                DateOfBirth = new DateTime(1995, 2, 10),
                Gender = "Female",
                PhoneNumber = "111",
                LastTestDate = new DateTime(2024, 12, 1),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                MedicalRecords = new List<PatientMedicalRecord>
                {
                    new() { RecordId = Guid.NewGuid(), IsDeleted = false }
                }
            },
            new()
            {
                PatientId = Guid.NewGuid(),
                FullName = "Bob",
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = "Male",
                PhoneNumber = "222",
                LastTestDate = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                MedicalRecords = new List<PatientMedicalRecord>
                {
                    new() { RecordId = Guid.NewGuid(), IsDeleted = false }
                }
            }
        };

        _patientRepo.Setup(r => r.GetAllPatientsAsync()).ReturnsAsync(patients);

        var result = await _handler.Handle(new GetAllMedicalRecordsQuery(), CancellationToken.None);

        result.Should().HaveCount(2);

        result[0]!.FullName.Should().Be("Alice");
        result[0]!.PatientId.Should().Be(patients[0].PatientId);
        result[0]!.DateOfBirth.Should().Be(patients[0].DateOfBirth);
        result[0]!.LastTestDate.Should().Be(patients[0].LastTestDate);

        result[1]!.FullName.Should().Be("Bob");
        result[1]!.LastTestDate.Should().BeNull();

        _patientRepo.Verify(r => r.GetAllPatientsAsync(), Times.Once);
    }

    [Test]
    public async Task Handle_ShouldReturnEmpty_WhenNoPatients()
    {
        _patientRepo.Setup(r => r.GetAllPatientsAsync()).ReturnsAsync(new List<Patient>());

        var result = await _handler.Handle(new GetAllMedicalRecordsQuery(), CancellationToken.None);

        result.Should().NotBeNull().And.BeEmpty();
    }
}
