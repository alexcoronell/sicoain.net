using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sicoain.shared.Entities
{
    public class RolePermissions : BaseEntity
    {
        [Required]
        public required int RoleId { get; set; }

        [Required]
        public required int PermissionId { get; set; }

        [ForeignKey(nameof(PermissionId))]
        public virtual Permissions? Permission { get; set; }

        [ForeignKey(nameof(RoleId))]
        public virtual Roles? Role { get; set; }
    }
}
