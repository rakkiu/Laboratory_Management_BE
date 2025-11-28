using Microsoft.EntityFrameworkCore;
using PatientService.Domain.Entities.Patient;
using PatientService.Domain.Interfaces;
using PatientService.Infrastructure.Data;
using PatientService.Infrastructure.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Infrastructure.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="PatientService.Domain.Interfaces.IPatientMedicalRecordRepository" />
    public class PatientMedicalRecordRepository : IPatientMedicalRecordRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly PatientDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="PatientMedicalRecordRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public PatientMedicalRecordRepository(PatientDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds the medical record asynchronous.
        /// </summary>
        /// <param name="record">The record.</param>
        public async Task AddMedicalRecordAsync(PatientMedicalRecord record)
        {
            await _context.PatientMedicalRecords.AddAsync(record);
        }

        /// <summary>
        /// Deletes the medical record asynchronous.
        /// </summary>
        /// <param name="recordId">The record identifier.</param>
        /// <returns></returns>
        public async Task<bool> DeleteMedicalRecordAsync(Guid recordId)
        {
            var record = await _context.PatientMedicalRecords.FindAsync(recordId);
            if (record == null)
            {
                return false;
            }
            
            record.IsDeleted = true;
            return true;
        }

        /// <summary>
        /// Gets all medical records asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task<List<PatientMedicalRecord>> GetAllMedicalRecordsAsync()
        {
            var records = await _context.PatientMedicalRecords
                .Include(r => r.Patient)
                .AsNoTracking()
                .ToListAsync();
            
            // Decrypt patient data for each record
            foreach (var record in records)
            {
                if (record.Patient != null)
                {
                    DecryptPatientData(record.Patient);
                }
            }
            
            return records;
        }

        /// <summary>
        /// Gets the medical record by identifier asynchronous.
        /// </summary>
        /// <param name="recordId">The record identifier.</param>
        /// <returns></returns>
        public async Task<PatientMedicalRecord?> GetMedicalRecordByIdAsync(Guid recordId)
        {
            var record = await _context.PatientMedicalRecords
                .FirstOrDefaultAsync(r => r.RecordId == recordId); 
            return record;
        }

        /// <summary>
        /// Gets the medical records by patient identifier asynchronous.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public async Task<List<PatientMedicalRecord>> GetMedicalRecordsByPatientIdAsync(Guid patientId)
        {
            // For updates, we should not use AsNoTracking
            var records = await _context.PatientMedicalRecords
                .Where(r => r.PatientId == patientId)
                .ToListAsync();
            
            return records;
        }

        public async Task<PatientMedicalRecord?> GetMedicalRecordForUpdateAsync(Guid recordId)
        {
            return await _context.PatientMedicalRecords.FirstOrDefaultAsync(r => r.RecordId == recordId);
        }

        public async Task<PatientMedicalRecord?> GetLatestMedicalRecordByPatientIdAsync(Guid patientId)
        {
            return await _context.PatientMedicalRecords
                                 .Where(r => r.PatientId == patientId)
                                 .OrderByDescending(r => r.CreatedAt)
                                 .FirstOrDefaultAsync();
        }

        public void AddMedicalRecord(PatientMedicalRecord medicalRecord)
        {
            _context.PatientMedicalRecords.Add(medicalRecord);
        }

        /// <summary>
        /// Saves the change asynchronous.
        /// </summary>
        public async Task SaveChangeAsync()
        {
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates the medical record.
        /// </summary>
        /// <param name="record">The record.</param>
        public void UpdateMedicalRecord(PatientMedicalRecord record)
        {
            _context.PatientMedicalRecords.Update(record);
        }

        /// <summary>
        /// Decrypts patient sensitive data
        /// </summary>
        /// <param name="patient">The patient.</param>
        private static void DecryptPatientData(Patient patient)
        {
            if (!string.IsNullOrEmpty(patient.FullName))
                patient.FullName = EncryptionHelper.Decrypt(patient.FullName);
            
            if (!string.IsNullOrEmpty(patient.Email))
                patient.Email = EncryptionHelper.DecryptDeterministic(patient.Email);
            
            if (!string.IsNullOrEmpty(patient.PhoneNumber))
                patient.PhoneNumber = EncryptionHelper.Decrypt(patient.PhoneNumber);
            
            if (!string.IsNullOrEmpty(patient.Address))
                patient.Address = EncryptionHelper.Decrypt(patient.Address);
            
            if (!string.IsNullOrEmpty(patient.IdentifyNumber))
                patient.IdentifyNumber = EncryptionHelper.Decrypt(patient.IdentifyNumber);
        }
    }
}
