using FluentValidation;
using System.Globalization;

namespace PatientService.Application.UseCases.MedicalRecord.Commands.UpdateMedicalRecord
{
    /// <summary>
    /// Validator for the UpdatePatientMedicalRecordCommand
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator&lt;PatientService.Application.UseCases.MedicalRecord.Commands.UpdateMedicalRecord.UpdatePatientMedicalRecordCommand&gt;" />
    public class UpdateMedicalRecordValidator : AbstractValidator<UpdatePatientMedicalRecordCommand>
    {
        private static readonly string[] AcceptableDobFormats = { "MM/dd/yyyy" };

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateMedicalRecordValidator"/> class.
        /// </summary>
        public UpdateMedicalRecordValidator()
        {
            RuleFor(x => x.PatientId)
                .NotEmpty().WithMessage("Patient ID is required.");

            RuleFor(x => x.UpdatedBy)
                .NotEmpty().WithMessage("UpdatedBy is required.");

            RuleFor(x => x.Patient)
                .NotNull().WithMessage("Patient information is required.")
                .SetValidator(new PatientValidator());
        }

        /// <summary>
        /// Validator for the nested CreatePatient object
        /// </summary>
        public class PatientValidator : AbstractValidator<Models.PatientDto.CreatePatient>
        {
            public PatientValidator()
            {
                RuleFor(p => p.FullName)
                    .NotEmpty().WithMessage("Full name is required.");

                RuleFor(p => p.Email)
                    .NotEmpty().WithMessage("Email is required.")
                    .EmailAddress().WithMessage("A valid email address is required.");

                RuleFor(p => p.PhoneNumber)
                    .NotEmpty().WithMessage("Phone number is required.");

                RuleFor(p => p.Gender)
                    .NotEmpty().WithMessage("Gender is required.");

                RuleFor(p => p.Address)
                    .NotEmpty().WithMessage("Address is required.");

                RuleFor(p => p.IdentifyNumber)
                    .NotEmpty().WithMessage("Identify number is required.");

                RuleFor(p => p.UserId)
                    .NotEmpty().WithMessage("User ID is required.");

                RuleFor(p => p.DateOfBirth)
                    .NotEmpty().WithMessage("Date of birth is required.")
                    .Must(BeAValidDate).WithMessage("Invalid date of birth. Accepted format: MM/dd/yyyy.");

                RuleFor(p => p.LastTestDate)
                    .Must(BeAValidDate).When(p => !string.IsNullOrWhiteSpace(p.LastTestDate))
                    .WithMessage("Invalid last test date. Accepted format: MM/dd/yyyy.");
            }

            private bool BeAValidDate(string date)
            {
                return DateTime.TryParseExact(date, AcceptableDobFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
            }
        }
    }
}