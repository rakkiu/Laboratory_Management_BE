using MediatR;
using PatientService.Domain.Entities.TestOrder;
using PatientService.Domain.Interfaces;
using PatientService.Domain.Interfaces.TestOrderService;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PatientService.Application.UseCases.TestOrderUC.TestOrders.Commands.ReviewTestOrder
{
    public class ReviewTestOrderHandler : IRequestHandler<ReviewTestOrderCommand, bool>
    {
        private readonly ITestOrderRepository _testOrderRepository;
        private readonly IAuditLogRepository _auditLogRepository;

        public ReviewTestOrderHandler(
            ITestOrderRepository testOrderRepository,
            IAuditLogRepository auditLogRepository)
        {
            _testOrderRepository = testOrderRepository;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<bool> Handle(ReviewTestOrderCommand request, CancellationToken cancellationToken)
        {
            // 1. Lấy test order
            var testOrder = await _testOrderRepository
                .GetByIdWithResultsAsync(request.TestOrderId, cancellationToken);

            if (testOrder == null || testOrder.IsDeleted)
                return false;

            // 2. Ghi thông tin review
            testOrder.ReviewedBy = request.ReviewedBy.ToString();
            testOrder.ReviewedAt = DateTime.UtcNow;

            // 3. Tạo audit log
            var auditLog = new TestOrderAuditLog
            {
                TestOrderId = testOrder.TestOrderId,
                UserId = request.ReviewedBy.ToString(),
                ActionType = "REVIEW_TEST_ORDER",
                Timestamp = DateTime.UtcNow,
                ChangedFields = $"ReviewedBy = {testOrder.ReviewedBy}"
            };

            await _auditLogRepository.AddLogAsync(auditLog);

            // 4. Lưu database
            await _testOrderRepository.SaveChangeAsync();

            return true;
        }
    }


}
