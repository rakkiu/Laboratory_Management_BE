using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Application.UseCases.TestOrderUC.TestOrderResult.Command.CreateTestOrderResult
{
    public class CreateTestOrderResultCommand : IRequest
    {
        public Guid PatientId { get; set; }
        public Guid TestOrderId { get; set; }

        /// <summary>
        /// User performing the data entry via API layer.
        /// Populated in the controller using the authenticated principal.
        /// </summary>
        public string? EnteredBy { get; set; }
    }
}
