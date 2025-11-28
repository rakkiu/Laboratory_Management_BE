
namespace PatientService.Application.Models.PatientDto
{
    /// <summary>
    /// dto for creating a patient
    /// </summary>
    public class CreatePatient
    {
        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        public string FullName { get; set; } = null!;
        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        /// <value>
        /// The date of birth.
        /// </value>
        public string DateOfBirth { get; set; }
        /// <summary>
        /// Gets or sets the gender.
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        public string Gender { get; set; } = null!;
        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        /// <value>
        /// The phone number.
        /// </value>
        public string PhoneNumber { get; set; } = null!;
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string? Email { get; set; }
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string? Address { get; set; }
        /// <summary>
        /// Gets or sets the identify number.
        /// </summary>
        /// <value>
        /// The identify number.
        /// </value>
        public string? IdentifyNumber { get; set; }
        /// <summary>
        /// Gets or sets the last test date.
        /// </summary>
        /// <value>
        /// The last test date.
        /// </value>
        public string? LastTestDate { get; set; }
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public Guid? UserId { get; set; }
    }
}
