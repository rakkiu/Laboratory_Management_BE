using PatientService.Domain.Entities.TestOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PatientService.Domain.Interfaces.TestOrderService
{   
    public interface ITestOrderRepository
    {
        Task<IReadOnlyList<TestOrder>> GetAllAsync(CancellationToken ct);
        Task<TestOrder?> GetDetailAsync(Guid testOrderId, CancellationToken ct);
        Task AddTestOrderAsync(TestOrder testOrder);
        Task<List<TestOrder>> GetTestOrdersByMedicalRecordIdAsync(Guid recordId);
        Task SaveChangeAsync();
        Task<TestOrder> GetByIdWithResultsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(TestOrder testOrder);
        Task SaveChangesAsync();
    }
}
