using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs.Users
{
    public record ChangePasswordRequest
    {
        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; init; } = string.Empty;

        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string NewPassword { get; init; } = string.Empty;

        [Required]
        [Compare("NewPassword")]
        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; init; } = string.Empty;
    }
}
