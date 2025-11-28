using MediatR;
using PatientService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Application.UseCases.MedicalRecord.Commands.DeletePatient
{
    public class DeletePatientHandler : IRequestHandler<DeletePatientCommand, bool>
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IPatientMedicalRecordRepository _medicalRecordRepository;

        public DeletePatientHandler(IPatientRepository patientRepository, IPatientMedicalRecordRepository medicalRecordRepository)
        {
            _patientRepository = patientRepository;
            _medicalRecordRepository = medicalRecordRepository;
        }

        public async Task<bool> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
        {
            // Find the patient to be deleted
            var patient = await _patientRepository.GetPatientByIdAsync(request.PatientId);
            if (patient == null || patient.IsDeleted)
            {
                return false; // Or throw KeyNotFoundException
            }

            // Soft delete the patient
            patient.IsDeleted = true;
            _patientRepository.UpdatePatient(patient);

            // Find and soft delete all associated medical records
            var medicalRecords = await _medicalRecordRepository.GetMedicalRecordsByPatientIdAsync(request.PatientId);
            foreach (var record in medicalRecords)
            {
                if (!record.IsDeleted)
                {
                    record.IsDeleted = true;
                    _medicalRecordRepository.UpdateMedicalRecord(record);
                }
            }

            // AuditBehavior will detect changes to PatientMedicalRecord and log them.
            // It will also save all changes in a single transaction.

            return true;
        }
    }
}
