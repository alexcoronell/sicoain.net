using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs.Users
{
    public class UpdateUserRequest
    {
        [EmailAddress]
        public string? Email { get; set; }

        [MinLength(2)]
        [MaxLength(100)]
        public string? FullName { get; set; }

        public bool? IsActive { get; set; }
    }
}
