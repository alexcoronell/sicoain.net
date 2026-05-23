using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs.Users
{
    public record CreateUserRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; init; } = string.Empty;

        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; init; } = string.Empty;

        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        public string FullName { get; init; } = string.Empty;

        public List<string>? Roles { get; init; }
    }
}
