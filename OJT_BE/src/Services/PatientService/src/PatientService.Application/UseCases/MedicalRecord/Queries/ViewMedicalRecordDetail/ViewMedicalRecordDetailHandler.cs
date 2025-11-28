using PatientService.Application.Models.PatientMedicalRecordDto;
using PatientService.Domain.Interfaces;
using PatientService.Domain.Interfaces.TestOrderService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Application.UseCases.MedicalRecord.Queries.ViewMedicalRecordDetail
{
    public class ViewMedicalRecordDetailHandler : IRequestHandler<ViewMedicalRecordDetailQuery, ViewMedicalRecordDetailResponse>
    {
        private readonly IPatientMedicalRecordRepository _medicalRecordRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly ITestOrderRepository _testOrderRepository;
        public ViewMedicalRecordDetailHandler(IPatientMedicalRecordRepository medicalRecordRepository,
            IPatientRepository patientRepository,
            ITestOrderRepository testOrderRepository)
        {
            _medicalRecordRepository = medicalRecordRepository;
            _patientRepository = patientRepository;
            _testOrderRepository = testOrderRepository;
        }
        public async Task<ViewMedicalRecordDetailResponse> Handle(ViewMedicalRecordDetailQuery request, CancellationToken cancellationToken)
        {
            var medicalRecord = await _medicalRecordRepository.GetLatestMedicalRecordByPatientIdAsync(request.PatientId);
            if (medicalRecord == null)
            {
                throw new KeyNotFoundException($"Medical record for patient ID {request.PatientId} not found.");
            }
            var patient = await _patientRepository.GetPatientByIdAsync(request.PatientId);
            if (patient == null)
            {
                throw new KeyNotFoundException($"Patient with ID {request.PatientId} not found.");
            }
            var testOrders = await _testOrderRepository.GetTestOrdersByMedicalRecordIdAsync(medicalRecord.RecordId);

            var response = new ViewMedicalRecordDetailResponse
            {
                PatientId = patient.PatientId,
                PatientName = patient.FullName,
                DateOfBirth = patient.DateOfBirth,
                PhoneNumber = patient.PhoneNumber,
                Email = patient.Email,
                Address = patient.Address,
                IdentifyNumber = patient.IdentifyNumber,
                Gender = patient.Gender,
                LastTestDate = patient.LastTestDate,
                TestOrders = testOrders.Select(to => new TestOrderReponse
                {
                    TestOrderId = to.TestOrderId,
                    OrderDate = to.CreatedAt,
                    Status = to.Status
                }).ToList()
            };
            return response;
        }
    }
}
