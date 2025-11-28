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
    public interface IPatientRepository
    {
        /// <summary>
        /// Adds the patient asynchronous.
        /// </summary>
        /// <param name="patient">The patient.</param>
        /// <returns></returns>
        Task AddPatientAsync(Patient patient);
        /// <summary>
        /// Gets the patient by identifier asynchronous.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        Task<Patient?> GetPatientByIdAsync(Guid patientId);
        /// <summary>
        /// Gets the patient for update by identifier asynchronous.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        Task<Patient?> GetPatientForUpdateAsync(Guid patientId); // Add this new method
        /// <summary>
        /// Gets all patients asynchronous.
        /// </summary>
        /// <returns></returns>
        /// 
        Task<List<Patient>> GetAllPatientsAsync();
        Task<IEnumerable<Patient>> GetAllPatientsAsyncUpdate();
        /// <summary>
        /// Updates the patient.
        /// </summary>
        /// <param name="patient">The patient.</param>
        /// <returns></returns>
        Task<bool> UpdatePatient(Patient patient);
        /// <summary>
        /// Deletes the patient asynchronous.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        Task<bool> DeletePatientAsync(Guid patientId);
        /// <summary>
        /// Saves the change asynchronous.
        /// </summary>
        /// <returns></returns>
        Task SaveChangeAsync();
        bool EmailIsExist(string? email);
        bool IdentifyNumberIsExist(string? identifyNumber);
        bool PhoneNumberIsExist(string phoneNumber);
    }
}
