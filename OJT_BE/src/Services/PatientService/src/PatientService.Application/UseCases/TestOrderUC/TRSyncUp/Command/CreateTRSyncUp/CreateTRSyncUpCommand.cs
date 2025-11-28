using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Application.UseCases.TestOrderUC.TRSyncUp.Command.CreateTRSyncUp
{
    /// <summary>
    /// Command to create a new test result
    /// </summary>
    public class CreateTRSyncUpCommand : IRequest<Guid>
    {
        /// <summary>
        /// The test order identifier
        /// </summary>
        public Guid TestOrderId { get; set; }

        /// <summary>
        /// Name of the test
        /// </summary>
        public string TestName { get; set; } = null!;

        /// <summary>
        /// Test result value
        /// </summary>
        public string Value { get; set; } = null!;

        /// <summary>
        /// Reference range for the test (optional)
        /// </summary>
        public string? ReferenceRange { get; set; }

        /// <summary>
        /// Interpretation of the result (optional)
        /// </summary>
        public string? Interpretation { get; set; }

        /// <summary>
        /// Instrument used for testing (optional)
        /// </summary>
        public string? InstrumentUsed { get; set; }

        /// <summary>
        /// Flag indicating result status (e.g., Normal, High, Low)
        /// </summary>
        public string Flag { get; set; } = "Normal";
    }

}
