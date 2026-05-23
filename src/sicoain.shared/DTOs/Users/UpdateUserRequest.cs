using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs.Users
{
    public record UpdateUserRequest
    {
        [EmailAddress]
        public string? Email { get; init; }

        [MinLength(2)]
        [MaxLength(100)]
        public string? FullName { get; init; }

        public bool? IsActive { get; init; }
    }
}
