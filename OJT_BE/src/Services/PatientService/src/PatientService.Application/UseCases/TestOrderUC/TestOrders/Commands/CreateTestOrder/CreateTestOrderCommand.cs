using MediatR;
using PatientService.Application.Models.PatientDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Application.UseCases.TestOrderUC.TestOrders.Commands.CreateTestOrder
{
    public class CreateTestOrderCommand : IRequest
    {
        public CreatePatient Patient { get; set; } = null!;
        public string CreatedBy { get; set; } = null!;
    }
}
