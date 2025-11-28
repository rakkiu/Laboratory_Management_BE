using System.Globalization;

namespace IAMService.Application.UseCases.User.Commands.CreateUser
{
    /// <summary>
    /// Validate dữ liệu tạo người dùng theo yêu cầu.
    /// - Gender chấp nhận: Male/Female/M/F (không phân biệt hoa thường).
    /// - DateOfBirth chấp nhận:MM/dd/yyyy
    /// - Phone: 8-20 ký tự.
    /// - IdentifyNumber: 6-20 ký tự.
    /// </summary>
    public class CreateUserValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Email must be a valid email address.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(20)
                .WithMessage("Phone number must be 8 to 20 characters.");

            RuleFor(x => x.FullName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.IdentifyNumber)
                .NotEmpty()
                .MinimumLength(6)
                .MaximumLength(20)
                .WithMessage("Identification number must be 6 to 20 characters.");

            RuleFor(x => x.Gender)
                .NotEmpty()
                .Must(g =>
                {
                    var v = g?.Trim().ToLowerInvariant();
                    return v is "male" or "female" or "m" or "f";
                })
                .WithMessage("Gender must be one of: Male, Female, M, F.");

            RuleFor(x => x.Age)
                .GreaterThan(0)
                .LessThanOrEqualTo(120);

            RuleFor(x => x.Address)
                .NotEmpty();

            RuleFor(x => x.DateOfBirth)
                .NotEmpty()
                .Must(dob =>
                        DateTime.TryParseExact(
                        dob,
                        "MM/dd/yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out _))
                .WithMessage("Invalid date of birth. Accepted format: MM/DD/YYYY.");
        }
    }
}