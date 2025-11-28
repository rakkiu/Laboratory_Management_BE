using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace PatientService.Application.UseCases.FlaggingSet.Command.UpdateFlagging
{
    public class UpdateFlaggingSetValidator : AbstractValidator<UpdateFlaggingSetCommand>
    {
        public UpdateFlaggingSetValidator()
        {
            // Rule for ConfigId (from route)
            //RuleFor(x => x.ConfigId)
            //    .GreaterThan(0).WithMessage("Configuration ID must be a positive number.");

            // Rule for TestName
            RuleFor(x => x.TestName)
                .NotEmpty().WithMessage("Test name is required.")
                .MaximumLength(100).WithMessage("Test name cannot exceed 100 characters.");

            // Rule for Version
            RuleFor(x => x.Version)
                .NotEmpty().WithMessage("Version is required.")
                .MaximumLength(20).WithMessage("Version cannot exceed 20 characters.");

            // Conditional validation for thresholds
            RuleFor(x => x.LowThreshold)
                .LessThan(x => x.HighThreshold)
                .When(x => x.LowThreshold.HasValue && x.HighThreshold.HasValue)
                .WithMessage("Low threshold must be less than high threshold.");
        }
    }
}
