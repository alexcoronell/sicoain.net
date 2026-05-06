using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Entities
{
    public class BusinessEmail: BaseEntity
    {
        [Required]
        public required string Email { get; set; }

        public required int BusinessId { get; set; }

        public required Business Business { get; set; }
    }
}
