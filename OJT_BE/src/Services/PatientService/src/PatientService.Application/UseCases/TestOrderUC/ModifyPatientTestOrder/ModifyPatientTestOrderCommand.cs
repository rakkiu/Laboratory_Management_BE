using System;
using MediatR;
using PatientService.Application.Interfaces;

namespace PatientService.Application.UseCases.TestOrderUC.Commands.ModifyPatientTestOrder
{
    /// <summary>
    /// Command to modify patient's test order information
    /// </summary>
    public class ModifyPatientTestOrderCommand : IRequest<bool>, IAuditableCommand
    {
        /// <summary>
        /// Gets or sets the test order identifier.
        /// </summary>
        public Guid TestOrderId { get; set; }

        /// <summary>
        /// Gets or sets the patient's name.
        /// </summary>
        public string PatientName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the patient's date of birth.
        /// </summary>
        public string DateOfBirth { get; set; } = null!;

        /// <summary>
        /// Gets or sets the patient's age.
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// Gets or sets the patient's gender.
        /// </summary>
        public string Gender { get; set; } = null!;

        /// <summary>
        /// Gets or sets the patient's address.
        /// </summary>
        public string Address { get; set; } = null!;

        /// <summary>
        /// Gets or sets the patient's phone number.
        /// </summary>
        public string PhoneNumber { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user who performed the update.
        /// </summary>
        public Guid UpdatedBy { get; set; }

        // IAuditableCommand implementation
        public Guid PerformedBy => UpdatedBy;

        public string GetAuditAction() => "UPDATE";

        public Guid? GetEntityId() => TestOrderId;
    }
}
