using NUnit.Framework;
using FluentValidation.TestHelper;
using PatientService.Application.Models.PatientDto;
using PatientService.Application.UseCases.MedicalRecord.Commands.UpdateMedicalRecord;
using System;

namespace PatientService.Tests.UseCases.MedicalRecord.Commands
{
    [TestFixture]
    public class UpdateMedicalRecordValidatorTests
    {
        private UpdateMedicalRecordValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new UpdateMedicalRecordValidator();
        }

        [Test]
        public void Should_Have_Error_When_PatientId_Is_Empty()
        {
            var command = new UpdatePatientMedicalRecordCommand
            {
                PatientId = Guid.Empty,
                UpdatedBy = Guid.NewGuid(),
                Patient = CreateValidPatient()
            };

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.PatientId)
                  .WithErrorMessage("Patient ID is required.");
        }

        [Test]
        public void Should_Have_Error_When_UpdatedBy_Is_Empty()
        {
            var command = new UpdatePatientMedicalRecordCommand
            {
                PatientId = Guid.NewGuid(),
                UpdatedBy = Guid.Empty, // invalid
                Patient = CreateValidPatient()
            };

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.UpdatedBy)
                  .WithErrorMessage("UpdatedBy is required.");
        }

        [Test]
        public void Should_Have_Error_When_Patient_Is_Null()
        {
            var command = new UpdatePatientMedicalRecordCommand
            {
                PatientId = Guid.NewGuid(),
                UpdatedBy = Guid.NewGuid(),
                Patient = null
            };

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Patient)
                  .WithErrorMessage("Patient information is required.");
        }

        [Test]
        public void Should_Have_Error_When_Required_Fields_Are_Missing()
        {
            var patient = new CreatePatient(); // All fields empty
            var command = new UpdatePatientMedicalRecordCommand
            {
                PatientId = Guid.NewGuid(),
                UpdatedBy = Guid.NewGuid(),
                Patient = patient
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Patient.FullName);
            result.ShouldHaveValidationErrorFor(x => x.Patient.Email);
            result.ShouldHaveValidationErrorFor(x => x.Patient.PhoneNumber);
            result.ShouldHaveValidationErrorFor(x => x.Patient.Gender);
            result.ShouldHaveValidationErrorFor(x => x.Patient.Address);
            result.ShouldHaveValidationErrorFor(x => x.Patient.IdentifyNumber);
            result.ShouldHaveValidationErrorFor(x => x.Patient.UserId);
            result.ShouldHaveValidationErrorFor(x => x.Patient.DateOfBirth);
        }

        [Test]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            var patient = CreateValidPatient();
            patient.Email = "invalid-email"; // ❌ not a valid format

            var command = new UpdatePatientMedicalRecordCommand
            {
                PatientId = Guid.NewGuid(),
                UpdatedBy = Guid.NewGuid(),
                Patient = patient
            };

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Patient.Email)
                  .WithErrorMessage("A valid email address is required.");
        }

        [Test]
        public void Should_Have_Error_When_DateOfBirth_Is_Invalid_Format()
        {
            var patient = CreateValidPatient();
            patient.DateOfBirth = "2023-01-01"; // ❌ wrong format (should be MM/dd/yyyy)

            var command = new UpdatePatientMedicalRecordCommand
            {
                PatientId = Guid.NewGuid(),
                UpdatedBy = Guid.NewGuid(),
                Patient = patient
            };

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Patient.DateOfBirth)
                  .WithErrorMessage("Invalid date of birth. Accepted format: MM/dd/yyyy.");
        }

        [Test]
        public void Should_Have_Error_When_LastTestDate_Is_Invalid_Format()
        {
            var patient = CreateValidPatient();
            patient.LastTestDate = "2024-12-01"; // ❌ wrong format

            var command = new UpdatePatientMedicalRecordCommand
            {
                PatientId = Guid.NewGuid(),
                UpdatedBy = Guid.NewGuid(),
                Patient = patient
            };

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Patient.LastTestDate)
                  .WithErrorMessage("Invalid last test date. Accepted format: MM/dd/yyyy.");
        }

        [Test]
        public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
        {
            var command = new UpdatePatientMedicalRecordCommand
            {
                PatientId = Guid.NewGuid(),
                UpdatedBy = Guid.NewGuid(),
                Patient = CreateValidPatient()
            };

            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        private CreatePatient CreateValidPatient()
        {
            return new CreatePatient
            {
                FullName = "John Doe",
                Email = "john@example.com",
                PhoneNumber = "0123456789",
                Gender = "Male",
                Address = "123 Main St",
                IdentifyNumber = "123456789",
                UserId = Guid.NewGuid(),
                DateOfBirth = "05/15/1990",
                LastTestDate = "05/01/2023"
            };
        }
    }
}
