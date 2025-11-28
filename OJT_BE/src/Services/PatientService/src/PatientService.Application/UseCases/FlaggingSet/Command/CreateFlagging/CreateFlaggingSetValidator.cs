using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using PatientService.Application.UseCases.FlaggingSet.Command;

namespace PatientService.Application.UseCases.FlaggingSet.Command.CreateFlagging
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator&lt;PatientService.Application.UseCases.FlaggingSet.Command.CreateFlaggingSetCommand&gt;" />
    public class CreateOrUpdateFlaggingSetCommandValidator : AbstractValidator<CreateFlaggingSetCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateOrUpdateFlaggingSetCommandValidator"/> class.
        /// </summary>
        public CreateOrUpdateFlaggingSetCommandValidator()
        {
            RuleFor(v => v.TestName)
                .NotEmpty().WithMessage("TestName is required.")
                .MaximumLength(100).WithMessage("TestName must not exceed 100 characters.");

            RuleFor(v => v.Version)
                .NotEmpty().WithMessage("Version is required.")
                .MaximumLength(20).WithMessage("Version must not exceed 20 characters.");

            RuleFor(v => v.HighThreshold)
                .GreaterThan(v => v.LowThreshold)
                .When(v => v.LowThreshold.HasValue && v.HighThreshold.HasValue)
                .WithMessage("HighThreshold must be greater than LowThreshold.");
        }
    }
}
