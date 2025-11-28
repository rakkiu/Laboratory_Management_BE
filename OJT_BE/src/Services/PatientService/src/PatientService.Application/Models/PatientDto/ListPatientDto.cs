
namespace PatientService.Application.Models.PatientDto
{
    /// <summary>
    /// Show patient in list
    /// </summary>
    public class ListPatientDto
    {
        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        public string FullName { get; set; } = null!;
        /// <summary>
        /// Gets or sets the patient identifier.
        /// </summary>
        /// <value>
        /// The patient identifier.
        /// </value>
        public Guid PatientId { get; set; }
        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        /// <value>
        /// The date of birth.
        /// </value>
        public DateTime DateOfBirth { get; set; }
        /// <summary>
        /// Gets or sets the last test date.
        /// </summary>
        /// <value>
        /// The last test date.
        /// </value>
        public DateTime? LastTestDate { get; set; }
        public string PhoneNumber { get; set; }
    }
}
