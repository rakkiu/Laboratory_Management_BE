using PatientService.Domain.Entities.TestOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Domain.Interfaces.TestOrderService
{
    public interface ITestResultRepository
    {
        Task AddTestResultAsync(TestResult testResult);
        Task SaveChangesAsync();
        Task<TestResult?> GetTestResultByIdAsync(Guid resultId);
        Task<bool> TestOrderExistsAsync(Guid testOrderId);

    }
}
