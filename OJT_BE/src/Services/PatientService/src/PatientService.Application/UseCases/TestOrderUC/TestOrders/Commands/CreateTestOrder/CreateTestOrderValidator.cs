using FluentValidation;
using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Application.UseCases.TestOrderUC.TestOrders.Commands.CreateTestOrder
{
    public class CreateTestOrderValidator : AbstractValidator<CreateTestOrderCommand>
    {
        public CreateTestOrderValidator()
        {
            RuleFor(x => x.Patient)
                .NotNull().WithMessage("Patient information is required.");

            When(x => x.Patient != null, () =>
            {
                RuleFor(x => x.Patient.FullName)
                    .NotEmpty().WithMessage("Full name is required.")
                    .MaximumLength(200);

                RuleFor(x => x.Patient.DateOfBirth)
                    .NotEmpty()
                .Must(dob =>
                        DateTime.TryParseExact(
                        dob,
                        "MM/dd/yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out _))
                .WithMessage("Invalid date of birth. Accepted format: MM/DD/YYYY.");

                RuleFor(x => x.Patient.Gender)
                    .NotEmpty().WithMessage("Gender is required.")
                    .Must(g => g == "male" || g == "female")
                    .WithMessage("Gender must be 'male', 'female'");

                RuleFor(x => x.Patient.PhoneNumber)
                    .NotEmpty().WithMessage("Phone number is required.")
                    .Matches(@"^\+?[0-9]{9,15}$")
                    .WithMessage("Invalid phone number format.");

                RuleFor(x => x.Patient.Email)
                    .NotEmpty().EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Patient.Email))
                    .WithMessage("Invalid email format.")
                    .MaximumLength(200);

                RuleFor(x => x.Patient.Address)
                    .MaximumLength(300)
                    .When(x => x.Patient.Address != null);

                RuleFor(x => x.Patient.IdentifyNumber)
                    .NotEmpty().MaximumLength(50)
                    .When(x => x.Patient.IdentifyNumber != null);
            });

        }
    }
}
