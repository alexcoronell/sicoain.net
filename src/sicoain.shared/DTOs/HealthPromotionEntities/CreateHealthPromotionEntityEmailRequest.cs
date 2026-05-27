using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs.HealthPromotionEntities
{
    public record CreateHealthPromotionEntityEmailRequest : CreateEntityEmailRequest
    {
        [Required, Range(1, int.MaxValue)]
        public int HealthPromotionEntityId { get; init; }
    }
}
