using PatientService.Application.Models.PatientDto;
using PatientService.Application.Models.PatientMedicalRecordDto;
using PatientService.Domain.Interfaces;
using System.Globalization;

namespace PatientService.Application.UseCases.MedicalRecord.Commands.UpdateMedicalRecord
{
    public class UpdateMedicalRecordHandler : IRequestHandler<UpdatePatientMedicalRecordCommand, MedicalRecordDto>
    {
        private readonly IPatientMedicalRecordRepository _medicalRecordRepository;
        private readonly IPatientRepository _patientRepository;

        private static readonly string[] AcceptableDobFormats = { "MM/dd/yyyy" };

        public UpdateMedicalRecordHandler(
            IPatientMedicalRecordRepository medicalRecordRepository,
            IPatientRepository patientRepository)
        {
            _medicalRecordRepository = medicalRecordRepository;
            _patientRepository = patientRepository;
        }

        public async Task<MedicalRecordDto> Handle(UpdatePatientMedicalRecordCommand request, CancellationToken cancellationToken)
        {
            // 1. Get existing patient
            var patient = await _patientRepository.GetPatientForUpdateAsync(request.PatientId);
            if (patient == null)
            {
                throw new KeyNotFoundException($"Patient with ID {request.PatientId} not found");
            }

            // 2. Get the latest medical record for this patient
            var medicalRecord = await _medicalRecordRepository.GetLatestMedicalRecordByPatientIdAsync(request.PatientId);
            if (medicalRecord == null)
            {
                throw new KeyNotFoundException($"No medical record found for patient with ID {request.PatientId}");
            }

            

            // 4. Parse and validate dates from request
            if (!DateTime.TryParseExact(request.Patient.DateOfBirth, AcceptableDobFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dob))
                throw new ArgumentException("Invalid date of birth. Accepted: MM/dd/yyyy.");

            DateTime? lastTestDate = null;
            if (!string.IsNullOrWhiteSpace(request.Patient.LastTestDate))
            {
                if (!DateTime.TryParseExact(request.Patient.LastTestDate, AcceptableDobFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var ltd))
                    throw new ArgumentException("Invalid last test date. Accepted: MM/dd/yyyy.");
                lastTestDate = ltd.ToUniversalTime();
            }

            // 5. Update patient entity properties
            patient.FullName = request.Patient.FullName;
            patient.DateOfBirth = dob.ToUniversalTime();
            patient.IdentifyNumber = request.Patient.IdentifyNumber;
            patient.Address = request.Patient.Address;
            patient.Email = request.Patient.Email;
            patient.Gender = request.Patient.Gender;
            patient.PhoneNumber = request.Patient.PhoneNumber;
            patient.UserId = request.Patient.UserId;
            patient.LastTestDate = lastTestDate;
            patient.UpdatedAt = DateTime.UtcNow;
            _patientRepository.UpdatePatient(patient);

            // 6. Update medical record entity properties
            medicalRecord.UpdatedAt = DateTime.UtcNow;
            medicalRecord.UpdatedBy = request.UpdatedBy;
            medicalRecord.Version++;
            _medicalRecordRepository.UpdateMedicalRecord(medicalRecord);

            // 7. Return DTO (AuditBehavior will handle SaveChangesAsync)
            return new MedicalRecordDto
            {
                RecordId = medicalRecord.RecordId,
                PatientId = medicalRecord.PatientId,
                Version = medicalRecord.Version,
                CreatedAt = medicalRecord.CreatedAt,
                UpdatedAt = medicalRecord.UpdatedAt,
                UpdatedBy = medicalRecord.UpdatedBy,
                IsDeleted = medicalRecord.IsDeleted,
                Patient = request.Patient
            };
        }
    }
}
