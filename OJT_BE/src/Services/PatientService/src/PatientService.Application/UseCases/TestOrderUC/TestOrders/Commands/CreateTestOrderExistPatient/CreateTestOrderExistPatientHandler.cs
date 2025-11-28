using Microsoft.AspNetCore.Http.HttpResults;
using PatientService.Domain.Entities.TestOrder;
using PatientService.Domain.Interfaces;
using PatientService.Domain.Interfaces.TestOrderService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Application.UseCases.TestOrderUC.TestOrders.Commands.CreateTestOrderExistPatient
{
    public class CreateTestOrderExistPatientHandler : IRequestHandler<CreateTestOrderExistPatientCommand>
    {
        private readonly ITestOrderRepository _testOrderRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IPatientMedicalRecordRepository _patientMedicalRecordRepository;
        private readonly IAuditLogRepository _auditLogRepository;

        public CreateTestOrderExistPatientHandler(
            ITestOrderRepository testOrderRepository,
            IPatientRepository patientRepository,
            IPatientMedicalRecordRepository patientMedicalRecordRepository,
            IAuditLogRepository auditLogRepository)
        {
            _testOrderRepository = testOrderRepository;
            _patientRepository = patientRepository;
            _patientMedicalRecordRepository = patientMedicalRecordRepository;
            _auditLogRepository = auditLogRepository;
        }

        public async Task Handle(CreateTestOrderExistPatientCommand request, CancellationToken cancellationToken)
        {
            var patient = await _patientRepository.GetPatientByIdAsync(request.PatientId);
            if (patient == null)
            {
                throw new KeyNotFoundException("Patient not found.");
            }

            var medicalRecord = await _patientMedicalRecordRepository.GetLatestMedicalRecordByPatientIdAsync(request.PatientId);
            if (medicalRecord == null)
            {
                throw new ArgumentException("Medical record not found for the patient.");
            }

            var nowUtc = DateTime.UtcNow;

            TestOrder testOrder = new TestOrder
            {
                TestOrderId = Guid.NewGuid(),
                PatientId = request.PatientId,
                Status = "Pending",
                CreatedBy = request.CreatedBy,
                CreatedAt = nowUtc,
                Address = patient.Address,
                DateOfBirth = patient.DateOfBirth,
                PatientName = patient.FullName,
                Age = (int)((nowUtc - patient.DateOfBirth).TotalDays / 365.25),
                Comments = null,
                Gender = patient.Gender,
                IsDeleted = false,
                PhoneNumber = patient.PhoneNumber,
                RecordId = medicalRecord.RecordId
            };

            await _testOrderRepository.AddTestOrderAsync(testOrder);

            // === ADD AUDIT LOG ===
            var auditLog = new TestOrderAuditLog
            {
                TestOrderId = testOrder.TestOrderId,
                UserId = request.CreatedBy.ToString(),
                ActionType = "CREATE_TEST_ORDER",
                Timestamp = DateTime.UtcNow,
                ChangedFields =
                    $"Created TestOrder. " +
                    $"| PatientName: {testOrder.PatientName} " +
                    $"| Status: {testOrder.Status} " +
                    $"| RecordId: {testOrder.RecordId}"
            };

            await _auditLogRepository.AddLogAsync(auditLog);

            await _testOrderRepository.SaveChangeAsync();  // Save order + audit log
        }
    }
}
