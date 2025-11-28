using MediatR;
using PatientService.Application.Models.TestOrderReportDto;

namespace PatientService.Application.UseCases.TestOrderUC.GetEmailInfo
{
    public record GetTestOrderEmailInfoQuery(Guid TestOrderId)
        : IRequest<TestOrderEmailDto>;
}
