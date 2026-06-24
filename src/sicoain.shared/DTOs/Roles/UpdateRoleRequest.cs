using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs.Roles
{
    public record UpdateRoleRequest
    {
        public string Description { get; init; } = string.Empty;

        public bool IsActive { get; init; }
    }
}
