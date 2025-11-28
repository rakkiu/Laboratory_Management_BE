using Microsoft.EntityFrameworkCore;
using PatientService.Domain.Entities.TestOrder;
using PatientService.Domain.Interfaces.TestOrderService;
using PatientService.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PatientService.Infrastructure.Repositories.TestOrderService
{
    public class TestOrderRepository : ITestOrderRepository
    {
        private readonly PatientDbContext _context;

        public TestOrderRepository(PatientDbContext context)
        {
            _context = context;
        }

        // ============================
        //            READ
        // ============================

        public async Task<IReadOnlyList<TestOrder>> GetAllAsync(CancellationToken ct)
        {
            return await _context.TestOrders
                .AsNoTracking()
                .Where(t => !t.IsDeleted)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(ct);
        }

        /// <summary>
        /// Lấy chi tiết TestOrder theo Id (bao gồm TestResults + Comments).
        /// </summary>
        public async Task<TestOrder?> GetDetailAsync(Guid testOrderId, CancellationToken ct)
        {
            return await _context.TestOrders
                .AsNoTracking()
                .Where(t => t.TestOrderId == testOrderId && !t.IsDeleted)
                .Include(t => t.TestResults!)
                    .ThenInclude(r => r.TestResultDetails)
                .Include(t => t.Comments)
                .FirstOrDefaultAsync(ct);
        }


        // ============================
        //            WRITE
        // ============================

        public async Task AddTestOrderAsync(TestOrder testOrder)
        {
            await _context.TestOrders.AddAsync(testOrder);
        }

        /// <summary>
        /// Lấy danh sách TestOrder theo MedicalRecordId
        /// </summary>
        public async Task<List<TestOrder>> GetTestOrdersByMedicalRecordIdAsync(Guid recordId)
        {
            return await _context.TestOrders
                .Where(to => !to.IsDeleted && to.RecordId == recordId)
                .ToListAsync();
        }

        public async Task SaveChangeAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<TestOrder> GetByIdWithResultsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.TestOrders
                .Include(to => to.TestResults)
                .FirstOrDefaultAsync(to => to.TestOrderId == id, cancellationToken);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public Task<bool> UpdateAsync(TestOrder testOrder)
        {
            // ✅ Removed all encryption - data is stored as plain text
            _context.TestOrders.Update(testOrder);
            return Task.FromResult(true);
        }
    }
}
