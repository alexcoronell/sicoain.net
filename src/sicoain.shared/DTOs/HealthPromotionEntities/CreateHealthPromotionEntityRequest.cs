using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs.HealthPromotionEntities
{
    public class CreateHealthPromotionEntityRequest
    {
        [Required, MinLength(3), MaxLength(100)]
        public string? Name { get; init; }
        public string? AddressStreet { get; init; }

        public string? Notes { get; init; }
    }
}
