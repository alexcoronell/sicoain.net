using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Entities
{
    public class BusinessPhone : BaseEntity
    {
        [Required]
        public required string Phone { get; set; }

        public required int BusinessId { get; set; }

        public required Business Business { get; set; }
    }
}
