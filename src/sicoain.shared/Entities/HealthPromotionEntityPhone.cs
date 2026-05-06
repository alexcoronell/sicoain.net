

using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Entities
{
    public class HealthPromotionEntityPhone : BaseEntity
    {
        [Required]
        public required string Phone { get; set; }

        public required int HealthPromotionEntityId { get; set; }

        public required HealthPromotionEntity HealthPromotionEntity { get; set; }
    }
}
