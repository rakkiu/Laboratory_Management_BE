using System.Collections.Generic;
using System.Threading.Tasks;
using PatientService.Application.Models.TestOrderReportDto;

namespace PatientService.Application.Interfaces
{
    public interface IExcelService
    {
        Task<byte[]> ExportTestOrdersAsync(IEnumerable<TestOrderExportDto> testOrders, string fileName);
    }
}
