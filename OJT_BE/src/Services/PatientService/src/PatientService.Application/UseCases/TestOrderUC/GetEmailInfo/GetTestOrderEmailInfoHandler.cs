using MediatR;
using PatientService.Application.Models.TestOrderReportDto;
using PatientService.Domain.Interfaces;
using PatientService.Domain.Interfaces.TestOrderService;

namespace PatientService.Application.UseCases.TestOrderUC.GetEmailInfo
{
    public class GetTestOrderEmailInfoHandler
        : IRequestHandler<GetTestOrderEmailInfoQuery, TestOrderEmailDto>
    {
        private readonly ITestOrderRepository _testOrderRepository;
        private readonly IPatientRepository _patientRepository;

        public GetTestOrderEmailInfoHandler(
            ITestOrderRepository testOrderRepository,
            IPatientRepository patientRepository)
        {
            _testOrderRepository = testOrderRepository;
            _patientRepository = patientRepository;
        }

        public async Task<TestOrderEmailDto> Handle(
            GetTestOrderEmailInfoQuery request,
            CancellationToken cancellationToken)
        {
            // 1. Lấy TestOrder
            var order = await _testOrderRepository.GetDetailAsync(request.TestOrderId, cancellationToken);

            if (order == null)
                throw new KeyNotFoundException("Test order not found.");

            // 2. Lấy Patient theo PatientId
            var patient = await _patientRepository.GetPatientByIdAsync(order.PatientId);

            if (patient == null)
                throw new KeyNotFoundException("Patient not found.");

            // 3. Trả về DTO chứa email + name
            return new TestOrderEmailDto
            {
                TestOrderId = order.TestOrderId,
                PatientName = patient.FullName,
                Email = patient.Email
            };
        }
    }
}
