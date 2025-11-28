using PatientService.Domain.Entities.TestOrder;
using PatientService.Domain.Interfaces;
using PatientService.Domain.Interfaces.TestOrderService;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace PatientService.Application.UseCases.TestOrderUC.TestOrders.Commands.DeleteTestOrder
{
    public class DeleteTestOrderHandler : IRequestHandler<DeleteTestOrderCommand, bool>
    {
        private readonly ITestOrderRepository _testOrderRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteTestOrderHandler(
            ITestOrderRepository testOrderRepository,
            IAuditLogRepository auditLogRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _testOrderRepository = testOrderRepository;
            _auditLogRepository = auditLogRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> Handle(DeleteTestOrderCommand request, CancellationToken cancellationToken)
        {
            // Lấy userId từ JWT
            var userId = _httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value
                ?? _httpContextAccessor.HttpContext?.User?.FindFirst("userId")?.Value
                ?? "Unknown";

            var testOrder = await _testOrderRepository.GetByIdWithResultsAsync(request.TestOrderId, cancellationToken);

            if (testOrder == null || testOrder.IsDeleted)
            {
                return false;
            }

            testOrder.IsDeleted = true;

            // === ADD AUDIT LOG ===
            var auditLog = new TestOrderAuditLog
            {
                TestOrderId = testOrder.TestOrderId,
                UserId = userId,                      
                ActionType = "DELETE_TEST_ORDER",
                Timestamp = DateTime.UtcNow,
                ChangedFields =
                    $"Deleted TestOrder. | PatientName: {testOrder.PatientName} | Status: {testOrder.Status} | CreatedBy: {testOrder.CreatedBy}"
            };

            await _auditLogRepository.AddLogAsync(auditLog);

            await _testOrderRepository.SaveChangeAsync();

            return true;
        }
    }
}
