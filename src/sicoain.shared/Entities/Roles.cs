using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Entities
{
    public class Roles : BaseEntity
    {
        public int IdentityRoleId { get; set; }

        [Required]
        public required string Name { get; set; }

        public string? NormalizedName { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

    }
}
