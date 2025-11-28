using FluentAssertions;
using Moq;
using NUnit.Framework;
using PatientService.Application.UseCases.MedicalRecord.Commands.DeleteMedicalRecord;
using PatientService.Domain.Entities.Patient;
using PatientService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TestProject.DeleteMedicalRecord
{
    [TestFixture]
    public class DeleteMedicalRecordHandlerTests
    {
        private Mock<IPatientMedicalRecordRepository> _mockRepo = null!;
        private DeleteMedicalRecordHandler _handler = null!;

        [SetUp]
        public void SetUp()
        {
            _mockRepo = new Mock<IPatientMedicalRecordRepository>();
            _handler = new DeleteMedicalRecordHandler(_mockRepo.Object);
        }

        [Test]
        public async Task Handle_NoRecordsFoundForPatient_ReturnsFalse()
        {
            // Arrange
            var command = new DeleteMedicalRecordCommand
            {
                PatientId = Guid.NewGuid(),
                DeletedBy = Guid.NewGuid()
            };

            _mockRepo
                .Setup(r => r.GetMedicalRecordsByPatientIdAsync(command.PatientId))
                .ReturnsAsync(new List<PatientMedicalRecord>());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.False);
            _mockRepo.Verify(r => r.UpdateMedicalRecord(It.IsAny<PatientMedicalRecord>()), Times.Never);
        }

        [Test]
        public async Task Handle_RecordsExistForPatient_SoftDeletesAllAndReturnsTrue()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var existingRecords = new List<PatientMedicalRecord>
            {
                new PatientMedicalRecord { RecordId = Guid.NewGuid(), PatientId = patientId, IsDeleted = false },
                new PatientMedicalRecord { RecordId = Guid.NewGuid(), PatientId = patientId, IsDeleted = false }
            };

            var command = new DeleteMedicalRecordCommand
            {
                PatientId = patientId,
                DeletedBy = Guid.NewGuid()
            };

            _mockRepo
                .Setup(r => r.GetMedicalRecordsByPatientIdAsync(patientId))
                .ReturnsAsync(existingRecords);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.True);
            foreach (var record in existingRecords)
            {
                Assert.That(record.IsDeleted, Is.True);
                Assert.That(record.UpdatedBy, Is.EqualTo(command.DeletedBy));
                Assert.That(record.UpdatedAt, Is.Not.Null);
            }

            _mockRepo.Verify(r => r.UpdateMedicalRecord(It.Is<PatientMedicalRecord>(
                m => m.IsDeleted &&
                     m.UpdatedBy == command.DeletedBy &&
                     m.UpdatedAt != default
            )), Times.Exactly(existingRecords.Count));
        }
    }
}