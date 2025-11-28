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
    public class TestResultDetailRepository : ITestResultDetailRepository
    {
        private readonly PatientDbContext _context;

        public TestResultDetailRepository(PatientDbContext context)
        {
            _context = context;
        }

        public async Task AddTestResultDetailAsync(TestResultDetail testResultDetail)
        {
            await _context.TestResultDetails.AddAsync(testResultDetail);
            await _context.SaveChangesAsync();
        }
    }
}
