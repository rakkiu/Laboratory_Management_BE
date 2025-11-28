using System.ComponentModel.DataAnnotations;

namespace IAMService.Application.Models.ChangePassword

{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Current password is missing.")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is missing.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}