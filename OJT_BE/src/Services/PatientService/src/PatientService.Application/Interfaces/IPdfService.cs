using System.Threading.Tasks;
using PatientService.Application.Models.TestOrderReportDto; // ✅ thêm dòng này

namespace PatientService.Application.Interfaces
{
    public interface IPdfService
    {
        Task<byte[]> GenerateTestOrderReportAsync(TestOrderPdfDto testOrder, string fileName);
    }
}
