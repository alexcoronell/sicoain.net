using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs.Users
{
    public record AssignOrRemoveRoleRequest
    {
        [Required]
        public string RoleName { get; init; } = string.Empty;
    }
}
