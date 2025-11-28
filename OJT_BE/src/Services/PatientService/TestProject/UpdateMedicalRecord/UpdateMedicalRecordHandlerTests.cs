using Moq;
using NUnit.Framework;
using PatientService.Application.Models.PatientDto;
using PatientService.Application.Models.PatientMedicalRecordDto;
using PatientService.Application.UseCases.MedicalRecord.Commands.UpdateMedicalRecord;
using PatientService.Domain.Entities;
using PatientService.Domain.Entities.Patient;
using PatientService.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PatientService.Tests.UseCases.MedicalRecord.Commands
{
    [TestFixture]
    public class UpdateMedicalRecordHandlerTests
    {
        private Mock<IPatientRepository> _patientRepositoryMock;
        private Mock<IPatientMedicalRecordRepository> _medicalRecordRepositoryMock;
        private UpdateMedicalRecordHandler _handler;

        [SetUp]
        public void Setup()
        {
            _patientRepositoryMock = new Mock<IPatientRepository>();
            _medicalRecordRepositoryMock = new Mock<IPatientMedicalRecordRepository>();
            _handler = new UpdateMedicalRecordHandler(_medicalRecordRepositoryMock.Object, _patientRepositoryMock.Object);
        }

        [Test]
        public async Task Handle_ShouldUpdateSuccessfully()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var updatedBy = Guid.NewGuid();
            var request = new UpdatePatientMedicalRecordCommand
            {
                PatientId = patientId,
                UpdatedBy = updatedBy,
                Patient = new CreatePatient
                {
                    FullName = "John Doe",
                    DateOfBirth = "05/15/1990",
                    IdentifyNumber = "123456789",
                    Address = "123 Main St",
                    Email = "john@example.com",
                    Gender = "Male",
                    PhoneNumber = "0123456789",
                    UserId = Guid.NewGuid(), // ✅ đúng kiểu Guid
                    LastTestDate = "05/01/2023"
                }
            };

            var patient = new Patient { PatientId = patientId, FullName = "Old Name" };
            var record = new PatientMedicalRecord
            {
                RecordId = Guid.NewGuid(),
                PatientId = patientId,
                Version = 1,
                CreatedAt = DateTime.UtcNow
            };

            _patientRepositoryMock
                .Setup(r => r.GetPatientForUpdateAsync(patientId))
                .ReturnsAsync(patient);

            _medicalRecordRepositoryMock
                .Setup(r => r.GetLatestMedicalRecordByPatientIdAsync(patientId))
                .ReturnsAsync(record);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Version, Is.EqualTo(2));
            Assert.That(result.Patient.FullName, Is.EqualTo("John Doe"));
            _patientRepositoryMock.Verify(r => r.UpdatePatient(It.IsAny<Patient>()), Times.Once);
            _medicalRecordRepositoryMock.Verify(r => r.UpdateMedicalRecord(It.IsAny<PatientMedicalRecord>()), Times.Once);

        }

        [Test]
        public void Handle_ShouldThrow_WhenPatientNotFound()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var updatedBy = Guid.NewGuid();
            var request = new UpdatePatientMedicalRecordCommand
            {
                PatientId = patientId,
                UpdatedBy = updatedBy,
                Patient = new CreatePatient
                {
                    FullName = "Jane Doe",
                    DateOfBirth = "05/15/1990",
                    IdentifyNumber = "123456789",
                    Address = "456 Main St",
                    Email = "jane@example.com",
                    Gender = "Female",
                    PhoneNumber = "0987654321",
                    UserId = Guid.NewGuid()
                }
            };

            _patientRepositoryMock
                .Setup(r => r.GetPatientForUpdateAsync(patientId))
                .ReturnsAsync((Patient)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(request, CancellationToken.None));
            Assert.That(ex.Message, Does.Contain("Patient with ID"));
        }

        [Test]
        public void Handle_ShouldThrow_WhenMedicalRecordNotFound()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var updatedBy = Guid.NewGuid();
            var request = new UpdatePatientMedicalRecordCommand
            {
                PatientId = patientId,
                UpdatedBy = updatedBy,
                Patient = new CreatePatient
                {
                    FullName = "Jane Doe",
                    DateOfBirth = "05/15/1990",
                    IdentifyNumber = "123456789",
                    Address = "456 Main St",
                    Email = "jane@example.com",
                    Gender = "Female",
                    PhoneNumber = "0987654321",
                    UserId = Guid.NewGuid()
                }
            };

            _patientRepositoryMock
                .Setup(r => r.GetPatientForUpdateAsync(patientId))
                .ReturnsAsync(new Patient());

            _medicalRecordRepositoryMock
                .Setup(r => r.GetLatestMedicalRecordByPatientIdAsync(patientId))
                .ReturnsAsync((PatientMedicalRecord)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(request, CancellationToken.None));
            Assert.That(ex.Message, Does.Contain("No medical record found"));
        }

        [Test]
        public void Handle_ShouldThrow_WhenInvalidDateFormat()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var updatedBy = Guid.NewGuid();
            var request = new UpdatePatientMedicalRecordCommand
            {
                PatientId = patientId,
                UpdatedBy = updatedBy,
                Patient = new CreatePatient
                {
                    FullName = "John Doe",
                    DateOfBirth = "2023-05-01", // ❌ Sai format, đáng ra MM/dd/yyyy
                    IdentifyNumber = "123456789",
                    Address = "123 Main St",
                    Email = "john@example.com",
                    Gender = "Male",
                    PhoneNumber = "0123456789",
                    UserId = Guid.NewGuid(),
                    LastTestDate = "05/01/2023"
                }
            };

            _patientRepositoryMock
                .Setup(r => r.GetPatientForUpdateAsync(patientId))
                .ReturnsAsync(new Patient());

            _medicalRecordRepositoryMock
                .Setup(r => r.GetLatestMedicalRecordByPatientIdAsync(patientId))
                .ReturnsAsync(new PatientMedicalRecord());

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(request, CancellationToken.None));
            Assert.That(ex.Message, Does.Contain("Invalid date of birth"));
        }
    }
}
