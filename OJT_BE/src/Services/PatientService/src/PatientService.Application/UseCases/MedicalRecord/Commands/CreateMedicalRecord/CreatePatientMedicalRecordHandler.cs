using PatientService.Application.Models.PatientDto;
using PatientService.Application.Models.PatientMedicalRecordDto;
using PatientService.Domain.Entities.Patient;
using PatientService.Domain.Interfaces;
using System.Globalization;

namespace PatientService.Application.UseCases.MedicalRecord.Commands.CreateMedicalRecord
{
    /// <summary>
    /// create patient medical record handler
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;PatientService.Application.UseCases.PatientMedicalRecord.Commands.CreateMedicalRecord.CreatePatientMedicalRecordCommad, PatientService.Application.Models.PatientMedicalRecordDto.CreatePatientMedicalRecord&gt;" />
    public class CreatePatientMedicalRecordHandler : IRequestHandler<CreatePatientMedicalRecordCommad, CreatePatientMedicalRecord>
    {
        /// <summary>
        /// The patient medical record repository
        /// </summary>
        private readonly IPatientMedicalRecordRepository _patientMedicalRecordRepository;
        /// <summary>
        /// The patient repository
        /// </summary>
        private readonly IPatientRepository _patientRepository;

        private static readonly string[] AcceptableDobFormats =
        {
            "MM/dd/yyyy"
        };
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePatientMedicalRecordHandler"/> class.
        /// </summary>
        /// <param name="patientMedicalRecordRepository">The patient medical record repository.</param>
        /// <param name="patientRepository">The patient repository.</param>
        public CreatePatientMedicalRecordHandler(
            IPatientMedicalRecordRepository patientMedicalRecordRepository,
            IPatientRepository patientRepository)
        {
            _patientMedicalRecordRepository = patientMedicalRecordRepository;
            _patientRepository = patientRepository;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<CreatePatientMedicalRecord> Handle(CreatePatientMedicalRecordCommad request, CancellationToken cancellationToken)
        {
            if (!DateTime.TryParseExact(request.Patient.DateOfBirth, AcceptableDobFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dob))
                throw new ArgumentException("Invalid date of birth. Accepted: MM/dd/yyyy.");

            DateTime? lastTestDate = null;

            if (!string.IsNullOrWhiteSpace(request.Patient.LastTestDate))
            {
                if (!DateTime.TryParseExact(request.Patient.LastTestDate, AcceptableDobFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var ltd))
                    throw new ArgumentException("Invalid last test date. Accepted: MM/dd/yyyy.");

                lastTestDate = ltd.ToUniversalTime();
            }
            if (_patientRepository.EmailIsExist(request.Patient.Email))
            {
                throw new ArgumentException("Email already exists.");
            }
            if (_patientRepository.IdentifyNumberIsExist(request.Patient.IdentifyNumber))
            {
                throw new ArgumentException("Identify number already exists.");
            }
            if (_patientRepository.PhoneNumberIsExist(request.Patient.PhoneNumber))
            {
                throw new ArgumentException("Phone number already exists.");
            }

            // Create patient entity
            var patient = new Patient
                {
                    PatientId = Guid.NewGuid(),
                    FullName = request.Patient.FullName,
                    DateOfBirth = dob.ToUniversalTime(),
                    IdentifyNumber = request.Patient.IdentifyNumber,
                    Address = request.Patient.Address,
                    Email = request.Patient.Email,
                    Gender = request.Patient.Gender,
                    PhoneNumber = request.Patient.PhoneNumber,
                    UserId = request.Patient.UserId,
                    LastTestDate = lastTestDate,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

            // Add patient (DO NOT call SaveChangesAsync here - AuditBehavior will handle it)
            await _patientRepository.AddPatientAsync(patient); 

            // Create medical record entity
            var newRecord = new PatientMedicalRecord
            {
                RecordId = Guid.NewGuid(),
                PatientId = patient.PatientId,
                CreatedAt = DateTime.UtcNow,
                UpdatedBy = null,
                UpdatedAt = null,
                Version = 1,
                IsDeleted = false
            };

            // Add medical record (DO NOT call SaveChangesAsync here - AuditBehavior will handle it)
            await _patientMedicalRecordRepository.AddMedicalRecordAsync(newRecord);

            // 🎯 AuditBehavior will handle SaveChangesAsync() + audit logging in a single transaction

            // Return response with plain text data
            return new CreatePatientMedicalRecord
            {
                Patient = new CreatePatient
                {
                    FullName = request.Patient.FullName,
                    DateOfBirth = request.Patient.DateOfBirth,
                    IdentifyNumber = request.Patient.IdentifyNumber,
                    Address = request.Patient.Address,
                    Email = request.Patient.Email,
                    Gender = request.Patient.Gender,
                    PhoneNumber = request.Patient.PhoneNumber,
                    UserId = request.Patient.UserId,
                    LastTestDate = patient.LastTestDate.ToString()
                },
                RecordId = newRecord.RecordId,
                PatientId = newRecord.PatientId,
                CreatedAt = newRecord.CreatedAt,
                UpdatedBy = newRecord.UpdatedBy,
                UpdatedAt = newRecord.UpdatedAt,
                Version = newRecord.Version,
                IsDeleted = newRecord.IsDeleted
            };
        }
    }
}
