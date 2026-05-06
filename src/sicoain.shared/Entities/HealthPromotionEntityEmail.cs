using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Entities
{
    public class HealthPromotionEntityEmail : BaseEntity
    {
        [Required]
        public required string Email { get; set; }

        public required int HealthPromotionEntityId { get; set; }

        public required HealthPromotionEntity HealthPromotionEntity { get; set; }
    }
}
