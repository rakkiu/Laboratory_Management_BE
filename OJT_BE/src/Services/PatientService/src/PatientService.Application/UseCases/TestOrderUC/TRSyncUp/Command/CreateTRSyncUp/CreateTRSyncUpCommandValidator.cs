using FluentValidation;

namespace PatientService.Application.UseCases.TestOrderUC.TRSyncUp.Command.CreateTRSyncUp
{
    /// <summary>
    /// Validator for CreateTestResultCommand
    /// </summary>
    public class CreateTRSyncUpCommandValidator : AbstractValidator<CreateTRSyncUpCommand>
    {
        public CreateTRSyncUpCommandValidator()
        {
            RuleFor(x => x.TestOrderId)
                .NotEmpty()
                .WithMessage("TestOrderId is required.")
                .NotEqual(Guid.Empty)
                .WithMessage("TestOrderId cannot be empty.");

            RuleFor(x => x.TestName)
                .NotEmpty()
                .WithMessage("TestName is required.")
                .MaximumLength(200)
                .WithMessage("TestName cannot exceed 200 characters.");

            RuleFor(x => x.Value)
                .NotEmpty()
                .WithMessage("Value is required.")
                .MaximumLength(100)
                .WithMessage("Value cannot exceed 100 characters.");

            RuleFor(x => x.ReferenceRange)
                .MaximumLength(100)
                .WithMessage("ReferenceRange cannot exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.ReferenceRange));

            RuleFor(x => x.Interpretation)
                .MaximumLength(500)
                .WithMessage("Interpretation cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Interpretation));

            RuleFor(x => x.InstrumentUsed)
                .MaximumLength(200)
                .WithMessage("InstrumentUsed cannot exceed 200 characters.")
                .When(x => !string.IsNullOrEmpty(x.InstrumentUsed));

            RuleFor(x => x.Flag)
                .NotEmpty()
                .WithMessage("Flag is required.");
        }
    }

}
