using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Entities
{
    public class RolePermissions : BaseEntity
    {
        [Required]
        public required int RoleId { get; set; }

        [Required]
        public required int PermissionId { get; set; }
    }
}
