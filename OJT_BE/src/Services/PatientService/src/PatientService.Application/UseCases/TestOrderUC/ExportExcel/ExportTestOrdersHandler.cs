using MediatR;
using Microsoft.EntityFrameworkCore;
using PatientService.Application.Interfaces;
using PatientService.Application.Models.TestOrderReportDto;
using PatientService.Domain.Entities.TestOrder;
using System.IO;

namespace PatientService.Application.UseCases.TestOrderUC.ExportExcel
{
    public class ExportTestOrdersHandler : IRequestHandler<ExportTestOrdersQuery, ExportFileResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly IExcelService _excelService;

        public ExportTestOrdersHandler(IApplicationDbContext context, IExcelService excelService)
        {
            _context = context;
            _excelService = excelService;
        }

        public async Task<ExportFileResult> Handle(ExportTestOrdersQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            IQueryable<TestOrder> query = _context.Set<TestOrder>().Where(x => !x.IsDeleted);

            // 🔹 Nếu có PatientId → lọc theo bệnh nhân
            if (request.PatientId.HasValue && request.PatientId.Value != Guid.Empty)
            {
                query = query.Where(x => x.PatientId == request.PatientId.Value);
            }

            var testOrders = await query.ToListAsync(cancellationToken);

            // 🔹 Nếu trống → lấy trong tháng hiện tại
            if (!testOrders.Any())
            {
                query = query.Where(x => x.CreatedAt.Month == now.Month);
                testOrders = await query.ToListAsync(cancellationToken);
            }

            if (!testOrders.Any())
                throw new Exception("No test orders found.");

            var dtoList = testOrders.Select(x => new TestOrderExportDto
            {
                Id = x.TestOrderId,
                PatientName = x.PatientName,
                Gender = x.Gender,
                DateOfBirth = x.DateOfBirth,
                PhoneNumber = x.PhoneNumber,
                Status = x.Status,
                CreatedBy = x.CreatedBy,
                CreatedOn = x.CreatedAt,
                RunBy = x.Status == "Complete" ? x.RunBy : "",
                RunOn = x.Status == "Complete" ? x.RunOn : null
            }).ToList();

            string fileName;

            // 🔹 Nếu có bệnh nhân → tên theo format Test Orders-{PatientName}-{Date}
            if (request.PatientId.HasValue && request.PatientId.Value != Guid.Empty)
            {
                var patientName = dtoList.First().PatientName.Replace(" ", "_");
                var safeName = string.Concat(patientName.Split(Path.GetInvalidFileNameChars()));
                fileName = $"Test Orders-{safeName}-{now:yyyyMMdd}.xlsx";
            }
            else
            {
                // 🔹 Không có PatientId → xuất toàn hệ thống
                fileName = "All_TestOrder.xlsx";
            }

            var bytes = await _excelService.ExportTestOrdersAsync(dtoList, fileName);

            return new ExportFileResult
            {
                FileBytes = bytes,
                FileName = fileName
            };
        }
    }
}
