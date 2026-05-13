using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs.Users
{
    public class AssignRoleRequest
    {
        [Required]
        public string RoleName { get; set; } = string.Empty;
    }
}
