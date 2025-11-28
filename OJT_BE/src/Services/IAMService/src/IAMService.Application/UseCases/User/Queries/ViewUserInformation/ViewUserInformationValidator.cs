namespace IAMService.Application.UseCases.User.Queries.ViewUserInformation
{
    /// <summary>
    /// Validator for ViewUserInformationCommand
    /// </summary>
    /// <seealso cref="AbstractValidator&lt;ViewUserInformationCommand&gt;" />
    public class ViewUserInformationValidator : AbstractValidator<ViewUserInformationCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewUserInformationValidator"/> class.
        /// </summary>
        public ViewUserInformationValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.")
                .NotEqual(Guid.Empty).WithMessage("UserId must be valid.");

            RuleFor(x => x.CurrentUserRole)
                .NotEmpty().WithMessage("User role is required.")
                .Must(role => role == "ADMIN" || role == "LAB_MANAGER")
                .WithMessage("Only Admin and Lab Manager can view user information.");
        }
    }
}

