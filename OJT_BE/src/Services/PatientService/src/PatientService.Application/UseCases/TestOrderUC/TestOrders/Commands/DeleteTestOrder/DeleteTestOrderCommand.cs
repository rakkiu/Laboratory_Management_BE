using MediatR;

namespace PatientService.Application.UseCases.TestOrderUC.TestOrders.Commands.DeleteTestOrder
{
    /// <summary>
    /// Represents a MediatR command to delete a test order.
    /// </summary>
    public class DeleteTestOrderCommand : IRequest<bool>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the test order to be deleted.
        /// </summary>
        public Guid TestOrderId { get; set; }

        /// <summary>
        /// Gets or sets the username of the user performing the deletion.
        /// Used for audit logging purposes.
        /// </summary>
        public string DeletedBy { get; set; } = string.Empty;
    }
}