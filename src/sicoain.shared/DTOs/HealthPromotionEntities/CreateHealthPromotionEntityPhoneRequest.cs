using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs.HealthPromotionEntities
{
    public record CreateHealthPromotionEntityPhoneRequest : CreateEntityPhoneRequest
    {
        [Required, Range(1, int.MaxValue)]
        public int HealthPromotionEntityId { get; init; }
    }
}
