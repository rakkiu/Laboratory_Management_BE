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
    /// <seealso cref="PatientService.Domain.Interfaces.IPatientRepository" />
    public class PatientRepository : IPatientRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly PatientDbContext _context;
        /// <summary>
        /// Initializes a new instance of the <see cref="PatientRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public PatientRepository(PatientDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Adds the patient asynchronous.
        /// </summary>
        /// <param name="patient">The patient.</param>
        public async Task AddPatientAsync(Patient patient)
        {
            patient.FullName = EncryptionHelper.Encrypt(patient.FullName);
            patient.PhoneNumber = EncryptionHelper.Encrypt(patient.PhoneNumber);
            patient.Email = EncryptionHelper.EncryptDeterministic(patient.Email);
            patient.Address = EncryptionHelper.Encrypt(patient.Address);
            patient.IdentifyNumber = EncryptionHelper.Encrypt(patient.IdentifyNumber);
            await _context.Patients.AddAsync(patient);
        }

        /// <summary>
        /// Deletes the patient asynchronous.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public async Task<bool> DeletePatientAsync(Guid patientId)
        {
            var loadedPatient = await _context.Patients.FindAsync(patientId);
            if (loadedPatient is null)
            {
                return false;
            }
            else
            {
                loadedPatient.IsDeleted = true;
                return true;
            }
        }

        /// <summary>
        /// Gets all patients asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Patient>> GetAllPatientsAsync()
        {
            var patients = await _context.Patients
                .AsNoTracking()
                .Include(i => i.MedicalRecords)
                .ToListAsync();
            foreach (var patient in patients)
            {
                DecryptUserSensitiveData(patient);
            }
            return patients;
        }

        /// <summary>
        /// Gets the patient by identifier asynchronous.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns></returns>
        public async Task<Patient?> GetPatientByIdAsync(Guid patientId)
        {
            var patient = await _context.Patients
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PatientId == patientId);
            
            if (patient != null)
            {
                DecryptUserSensitiveData(patient);
            }
            
            return patient;
        }

        /// <summary>
        /// Gets a patient entity by ID for update purposes, without decrypting data.
        /// </summary>
        /// <param name="patientId">The patient identifier.</param>
        /// <returns>The patient entity or null if not found.</returns>
        public async Task<Patient?> GetPatientForUpdateAsync(Guid patientId)
        {
            return await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == patientId);
        }

        public async Task<IEnumerable<Patient>> GetAllPatientsAsyncUpdate()
        {
            var patients = await _context.Patients.ToListAsync();
            return patients;
        }

        /// <summary>
        /// Updates the patient.
        /// </summary>
        /// <param name="patient">The patient.</param>
        /// <returns></returns>
        public Task<bool> UpdatePatient(Patient patient)
        {
            patient.FullName = EncryptionHelper.Encrypt(patient.FullName);
            patient.PhoneNumber = EncryptionHelper.Encrypt(patient.PhoneNumber);
            patient.Email = EncryptionHelper.EncryptDeterministic(patient.Email);
            patient.Address = EncryptionHelper.Encrypt(patient.Address);
            patient.IdentifyNumber = EncryptionHelper.Encrypt(patient.IdentifyNumber);
            _context.Patients.Update(patient);
            return Task.FromResult(true);
        }

        /// <summary>
        /// Decrypts the user sensitive data.
        /// </summary>
        /// <param name="patient">The patient.</param>
        private static void DecryptUserSensitiveData(Patient patient)
        {
            if (!string.IsNullOrEmpty(patient.Email))
                patient.Email = EncryptionHelper.DecryptDeterministic(patient.Email);
            
            if (!string.IsNullOrEmpty(patient.PhoneNumber))
                patient.PhoneNumber = EncryptionHelper.Decrypt(patient.PhoneNumber);
            
            if (!string.IsNullOrEmpty(patient.IdentifyNumber))
                patient.IdentifyNumber = EncryptionHelper.Decrypt(patient.IdentifyNumber);
            
            if (!string.IsNullOrEmpty(patient.FullName))
                patient.FullName = EncryptionHelper.Decrypt(patient.FullName);
            
            if (!string.IsNullOrEmpty(patient.Address))
                patient.Address = EncryptionHelper.Decrypt(patient.Address);
        }

        /// <summary>
        /// Saves the change asynchronous.
        /// </summary>
        /// <returns></returns>
        public Task SaveChangeAsync()
        {
            return _context.SaveChangesAsync();
        }

        public bool EmailIsExist(string? email)
        {
            var encryptedEmail = EncryptionHelper.EncryptDeterministic(email);
            return _context.Patients.Any(p => p.Email == encryptedEmail && !p.IsDeleted);
        }

        public bool IdentifyNumberIsExist(string? identifyNumber)
        {
            var patients = _context.Patients
                .AsNoTracking()
                .Include(i => i.MedicalRecords)
                .ToList();
            foreach (var patient in patients)
            {
                DecryptUserSensitiveData(patient);
            }
            return patients.Any(p => p.IdentifyNumber == identifyNumber);
        }

        public bool PhoneNumberIsExist(string phoneNumber)
        {
            var patients = _context.Patients
                .AsNoTracking()
                .Include(i => i.MedicalRecords)
                .ToList();
            foreach (var patient in patients)
            {
                DecryptUserSensitiveData(patient);
            }
            return patients.Any(p => p.PhoneNumber == phoneNumber);
        }
    }
}
