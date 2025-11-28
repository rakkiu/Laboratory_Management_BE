using System.ComponentModel.DataAnnotations;

namespace IAMService.Application.Models.ForgotPassword
{
    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "Email is missing.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
    }
}
