using PatientService.Application.Models.TestOrderDto;
using System.Collections.Generic;

namespace PatientService.Application.UseCases.TestOrderUC.ViewPatientTestOrder
{
    public class ViewPatientTestOrdersResult
    {
        public List<TestOrderListDto> Items { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }
}
