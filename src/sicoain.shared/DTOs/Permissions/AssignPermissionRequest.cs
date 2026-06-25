using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs.Permissions;

public class AssignPermissionRequest
{
    [Required]
    public string PermissionName { get; set; } = string.Empty;
}
