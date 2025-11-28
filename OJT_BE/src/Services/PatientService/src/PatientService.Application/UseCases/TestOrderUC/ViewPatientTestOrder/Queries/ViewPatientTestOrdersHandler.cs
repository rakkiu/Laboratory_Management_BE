using MediatR;
using PatientService.Application.Interfaces;
using PatientService.Application.Models.TestOrderDto;
using PatientService.Domain.Entities.TestOrder;
using PatientService.Domain.Interfaces.TestOrderService;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PatientService.Application.UseCases.TestOrderUC.ViewPatientTestOrder.Queries
{
    public class ViewPatientTestOrdersHandler
        : IRequestHandler<ViewPatientTestOrdersQuery, ViewPatientTestOrdersResult>
    {
        private readonly ITestOrderRepository _repository;
        private readonly IIamUserClient _iamUserClient;

        public ViewPatientTestOrdersHandler(ITestOrderRepository repository, IIamUserClient iamUserClient)
        {
            _repository = repository;
            _iamUserClient = iamUserClient;
        }

        public async Task<ViewPatientTestOrdersResult> Handle(
            ViewPatientTestOrdersQuery request,
            CancellationToken cancellationToken)
        {
            var orders = await _repository.GetAllAsync(cancellationToken);

            if (orders == null || orders.Count == 0)
            {
                return new ViewPatientTestOrdersResult
                {
                    Items = new(),
                    Message = "No Data"
                };
            }

            // --- Resolve CreatedBy GUIDs to full names via IAM ---
            var uniqueCreatedByGuids = orders
                .Where(o => !string.IsNullOrWhiteSpace(o.CreatedBy) && Guid.TryParse(o.CreatedBy, out _))
                .Select(o => Guid.Parse(o.CreatedBy))
                .Distinct()
                .ToList();

            var createdByNameMap = new Dictionary<Guid, string?>();
            foreach (var guid in uniqueCreatedByGuids)
            {
                var name = await _iamUserClient.GetUserFullNameAsync(guid, cancellationToken);
                createdByNameMap[guid] = name;
            }

            var mapped = orders.Select(o =>
            {
                string? createdByDisplay = o.CreatedBy;
                if (!string.IsNullOrWhiteSpace(o.CreatedBy) && Guid.TryParse(o.CreatedBy, out var guid))
                {
                    createdByDisplay = createdByNameMap.TryGetValue(guid, out var name) && !string.IsNullOrWhiteSpace(name)
                        ? name
                        : o.CreatedBy;
                }

                return new TestOrderListDto
                {
                    TestOrderId = o.TestOrderId,
                    PatientId = o.PatientId,
                    PatientName = o.PatientName,
                    Age = o.Age,
                    Gender = o.Gender,
                    PhoneNumber = o.PhoneNumber,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    CreatedBy = createdByDisplay ?? "Unknown",
                    RunBy = o.RunBy,
                    RunOn = o.RunOn
                };
            })
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

            return new ViewPatientTestOrdersResult
            {
                Items = mapped,
                Message = "Success"
            };
        }
    }
}
