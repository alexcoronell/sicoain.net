using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs.Users
{
    public class AssignOrRemoveRoleRequest
    {
        [Required]
        public string RoleName { get; set; } = string.Empty;
    }
}
