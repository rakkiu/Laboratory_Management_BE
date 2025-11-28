using PatientService.Domain.Entities.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Domain.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPatientMedicalRecordRepository
    {
        /// <summary>
        /// Adds the medical record asynchronous.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns></returns>
        Task AddMedicalRecordAsync(PatientMedicalRecord record);
        /// <summary>
        /// Gets the medical record by identifier asynchronous.
        /// </summary>
        /// <param name="recordId">The record identifier.</param>
        /// <returns></returns>
        Task<PatientMedicalRecord?> GetMedicalRecordByIdAsync(Guid recordId);
        /// <summary>
        /// Gets the medical record for update by identifier asynchronous.
        /// </summary>
        /// <param name="recordId">The record identifier.</param>
        /// <returns></returns>
        Task<PatientMedicalRecord?> GetMedicalRecordForUpdateAsync(Guid recordId);
        /// <summary>
        /// Gets the latest medical record by patient identifier asynchronous.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        Task<PatientMedicalRecord?> GetLatestMedicalRecordByPatientIdAsync(Guid patientId);
        /// <summary>
        /// Gets the medical records by patient identifier asynchronous.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        Task<List<PatientMedicalRecord>> GetMedicalRecordsByPatientIdAsync(Guid patientId);
        /// <summary>
        /// Updates the medical record.
        /// </summary>
        /// <param name="record">The record.</param>
        void UpdateMedicalRecord(PatientMedicalRecord record);
        /// <summary>
        /// Deletes the medical record asynchronous.
        /// </summary>
        /// <param name="recordId">The record identifier.</param>
        /// <returns></returns>
        Task<bool> DeleteMedicalRecordAsync(Guid recordId);
        /// <summary>
        /// Saves the change asynchronous.
        /// </summary>
        /// <returns></returns>
        Task SaveChangeAsync();
        /// <summary>
        /// Gets all medical records asynchronous.
        /// </summary>
        /// <returns></returns>
        Task<List<PatientMedicalRecord>> GetAllMedicalRecordsAsync();
    }
}
