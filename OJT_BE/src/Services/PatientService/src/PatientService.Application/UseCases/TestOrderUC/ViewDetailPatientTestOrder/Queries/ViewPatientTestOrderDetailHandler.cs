using MediatR;
using PatientService.Application.Models.TestOrderDto;
using PatientService.Domain.Interfaces.TestOrderService;
using PatientService.Application.Interfaces; 
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PatientService.Application.UseCases.TestOrderUC.ViewDetailPatientTestOrder.Queries
{
    public class ViewPatientTestOrderDetailHandler
        : IRequestHandler<ViewPatientTestOrderDetailQuery, ViewPatientTestOrderDetailResult>
    {
        private readonly ITestOrderRepository _repository;
        private readonly IIamUserClient _iamUserClient;

        public ViewPatientTestOrderDetailHandler(
            ITestOrderRepository repository,
            IIamUserClient iamUserClient)
        {
            _repository = repository;
            _iamUserClient = iamUserClient;
        }

        public async Task<ViewPatientTestOrderDetailResult> Handle(
            ViewPatientTestOrderDetailQuery request,
            CancellationToken cancellationToken)
        {
            // ---------------------------------------------------------------------
            // 1. GET TEST ORDER
            // ---------------------------------------------------------------------
            var order = await _repository.GetDetailAsync(request.TestOrderId, cancellationToken);

            if (order == null)
            {
                return new ViewPatientTestOrderDetailResult
                {
                    Message = "Test order not found."
                };
            }

            // ---------------------------------------------------------------------
            // 2. COLLECT GUIDs ONLY FOR CreatedBy + ReviewedBy
            // ---------------------------------------------------------------------
            var guidSet = new HashSet<Guid>();

            void AddGuid(string? raw)
            {
                if (!string.IsNullOrWhiteSpace(raw) && Guid.TryParse(raw, out var g))
                    guidSet.Add(g);
            }

            AddGuid(order.CreatedBy);
            AddGuid(order.ReviewedBy);

            // ---------------------------------------------------------------------
            // 3. gRPC IAM ? Resolve names
            // ---------------------------------------------------------------------
            var userMap = new Dictionary<Guid, string?>();

            foreach (var g in guidSet)
            {
                var name = await _iamUserClient.GetUserFullNameAsync(g, cancellationToken);
                userMap[g] = name;
            }

            string? ResolveName(string? raw)
            {
                if (string.IsNullOrWhiteSpace(raw)) return null;

                if (Guid.TryParse(raw, out var g))
                {
                    return userMap.TryGetValue(g, out var fullName) && !string.IsNullOrWhiteSpace(fullName)
                        ? fullName
                        : raw;
                }

                return raw;
            }

            // ---------------------------------------------------------------------
            // 4. MAP ORDER
            // ---------------------------------------------------------------------
            var response = new ViewPatientTestOrderDetailResult
            {
                TestOrder = new TestOrderDetailDto
                {
                    TestOrderId = order.TestOrderId,
                    PatientId = order.PatientId,
                    PatientName = order.PatientName,
                    DateOfBirth = order.DateOfBirth,
                    Age = order.Age,
                    Gender = order.Gender,
                    PhoneNumber = order.PhoneNumber,
                    Status = order.Status,

                    CreatedBy = ResolveName(order.CreatedBy),
                    CreatedAt = order.CreatedAt,

                    ReviewedBy = ResolveName(order.ReviewedBy),
                    ReviewedAt = order.ReviewedAt,

                    RunBy = order.RunBy,
                    RunOn = order.RunOn
                }
            };

            // ---------------------------------------------------------------------
            // 5. MAP RESULTS
            // ---------------------------------------------------------------------
            var results = new List<TestResultDetailDto>();

            if (order.Status == "Complete" && order.TestResults != null)
            {
                results = order.TestResults
                    .Where(r => r.TestResultDetails != null)
                    .SelectMany(r => r.TestResultDetails!.Select(d => new TestResultDetailDto
                    {
                        ResultId = r.ResultId,
                        TestName = d.Type,
                        Value = d.Value.ToString(),
                        ReferenceRange = d.ReferenceRange,
                        Interpretation = r.Interpretation,
                        InstrumentUsed = r.InstrumentUsed,
                        Flag = d.Flag,
                        CreatedAt = r.CreatedAt
                    }))
                    .OrderByDescending(r => r.CreatedAt)
                    .ToList();
            }

            response.TestResults = results;

            // ---------------------------------------------------------------------
            // 6. MAP COMMENTS
            // ---------------------------------------------------------------------
            response.Comments = order.Comments != null
                ? order.Comments
                    .OrderBy(c => c.CreatedAt)
                    .Select(c => new TestOrderCommentDto
                    {
                        CommentId = c.CommentId,
                        ResultId = c.ResultId,
                        UserName = c.UserName,
                        Content = c.Content,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    })
                    .ToList()
                : new List<TestOrderCommentDto>();

            // ---------------------------------------------------------------------
            // 7. Message theo status
            // ---------------------------------------------------------------------
            response.Message = order.Status switch
            {
                "Pending" => "Test order is pending. No test results available yet.",
                "Complete" => results.Count > 0
                    ? "Test order is complete. Test results retrieved."
                    : "Test order is complete but has no test results.",
                "Cancel" => "Test order has been canceled. No test results available.",
                _ => "Test order detail retrieved."
            };

            return response;
        }
    }
}
