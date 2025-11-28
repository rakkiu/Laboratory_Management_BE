using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace PatientService.Application.UseCases.TestOrderUC.TestOrders.Commands.CreateTestOrderExistPatient
{
    public class CreateTestOrderExistPatientCommand : IRequest
    {
        public Guid PatientId { get; set; }
        public string CreatedBy { get; set; } = null!;
    }
}
