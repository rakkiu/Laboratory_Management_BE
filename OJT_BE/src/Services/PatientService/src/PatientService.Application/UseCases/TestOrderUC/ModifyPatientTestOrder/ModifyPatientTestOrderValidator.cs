using FluentValidation;
using System.Globalization;

namespace PatientService.Application.UseCases.TestOrderUC.Commands.ModifyPatientTestOrder
{
    /// <summary>
    /// Validator for ModifyPatientTestOrderCommand
    /// Validates patient information according to business requirements
    /// </summary>
    public class ModifyPatientTestOrderValidator : AbstractValidator<ModifyPatientTestOrderCommand>
    {
        private static readonly string[] AcceptableDobFormats = { "MM/dd/yyyy" };

        public ModifyPatientTestOrderValidator()
        {
            RuleFor(x => x.TestOrderId)
                .NotEmpty()
                .WithMessage("Test order ID is required.");

            RuleFor(x => x.PatientName)
                .NotEmpty()
                .WithMessage("Patient name is required.")
                .MaximumLength(100)
                .WithMessage("Patient name must not exceed 100 characters.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty()
                .WithMessage("Date of birth is required.")
                .Must(BeAValidDate)
                .WithMessage("Invalid date of birth. Accepted format: MM/DD/YYYY.");

            RuleFor(x => x.Age)
                .GreaterThan(0)
                .WithMessage("Age must be greater than 0.")
                .LessThanOrEqualTo(120)
                .WithMessage("Age must be 120 or less.");

            RuleFor(x => x.Gender)
                .NotEmpty()
                .WithMessage("Gender is required.")
                .Must(g =>
                {
                    var v = g?.Trim().ToLowerInvariant();
                    return v is "male" or "female" or "m" or "f";
                })
                .WithMessage("Gender must be one of: Male, Female, M, F.");

            RuleFor(x => x.Address)
                .NotEmpty()
                .WithMessage("Address is required.")
                .MaximumLength(200)
                .WithMessage("Address must not exceed 200 characters.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .MinimumLength(8)
                .MaximumLength(20)
                .WithMessage("Phone number must be 8 to 20 characters.")
                .Matches(@"^[0-9+\-\s()]+$")
                .WithMessage("Phone number can only contain digits, +, -, spaces, and parentheses.");

            RuleFor(x => x.UpdatedBy)
                .NotEmpty()
                .WithMessage("UpdatedBy is required.");
        }

        /// <summary>
        /// Validates if the date string is in the correct format (MM/dd/yyyy)
        /// </summary>
        private bool BeAValidDate(string date)
        {
            return DateTime.TryParseExact(
                date,
                AcceptableDobFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _);
        }
    }
}
