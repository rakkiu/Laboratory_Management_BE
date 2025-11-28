namespace IAMService.Application.Features.Users.Commands;

/// <summary>
/// Defines validation rules for the <see cref="UpdateUserCommand"/>.
/// </summary>
/// <remarks>
/// This validator ensures that all required user fields are properly filled and formatted 
/// before the update operation is processed. It validates properties such as full name, 
/// email, phone number, gender, and age according to business rules.
/// </remarks>
public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateUserCommandValidator"/> class
    /// and defines validation rules for user update requests.
    /// </summary>
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            // Simple regex validation for Vietnamese phone numbers
            .Matches(@"(84|0[3|5|7|8|9])+([0-9]{8})\b")
            .WithMessage("Invalid phone number format.");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Gender is required.")
            .Must(g => g.Equals("male", StringComparison.OrdinalIgnoreCase) ||
                       g.Equals("female", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Gender must be 'male' or 'female'.");

        RuleFor(x => x.Age)
            .GreaterThan(0).WithMessage("Age must be greater than 0.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.");
    }
}
