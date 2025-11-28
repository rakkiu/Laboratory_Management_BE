using Microsoft.EntityFrameworkCore;
using PatientService.Domain.Entities.TestOrder;
using PatientService.Domain.Interfaces.TestOrderService;
using PatientService.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Infrastructure.Repositories.TestOrderService
{
    public class TestResultRepository : ITestResultRepository
    {
        private readonly PatientDbContext _context;
        public TestResultRepository(PatientDbContext context)
        {
            _context = context;
        }

        public async Task AddTestResultAsync(TestResult testResult)
        {
            await _context.TestResults.AddAsync(testResult);
        }

        public async Task<TestResult?> GetTestResultByIdAsync(Guid resultId)
        {
            return await _context.TestResults
               .AsNoTracking()
               .Include(tr => tr.TestOrder)
               .FirstOrDefaultAsync(tr => tr.ResultId == resultId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> TestOrderExistsAsync(Guid testOrderId)
        {
            return await _context.TestOrders
                .AnyAsync(to => to.TestOrderId == testOrderId && !to.IsDeleted);
        }

    }
}
