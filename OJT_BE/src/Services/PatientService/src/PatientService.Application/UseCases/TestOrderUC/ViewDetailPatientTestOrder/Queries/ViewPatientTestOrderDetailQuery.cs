using System;
using System.Collections.Generic;
using MediatR;
using PatientService.Application.Models.TestOrderDto;

namespace PatientService.Application.UseCases.TestOrderUC.ViewDetailPatientTestOrder.Queries
{
    public class ViewPatientTestOrderDetailQuery : IRequest<ViewPatientTestOrderDetailResult>
    {
        public ViewPatientTestOrderDetailQuery(Guid testOrderId)
        {
            TestOrderId = testOrderId;
        }

        public Guid TestOrderId { get; }
    }

    public class ViewPatientTestOrderDetailResult
    {
        public TestOrderDetailDto? TestOrder { get; set; }
        public IReadOnlyList<TestResultDetailDto> TestResults { get; set; } = Array.Empty<TestResultDetailDto>();
        public IReadOnlyList<TestOrderCommentDto> Comments { get; set; } = Array.Empty<TestOrderCommentDto>();
        public string Message { get; set; } = "Success";
    }
}


