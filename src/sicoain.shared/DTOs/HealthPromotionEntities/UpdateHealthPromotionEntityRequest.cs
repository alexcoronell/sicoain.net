using sicoain.shared.DTOs;

namespace sicoain.shared.DTOs.HealthPromotionEntities
{
    public record UpdateHealthPromotionEntityRequest
    {
        public string? Name { get; init; }
        public string? AddressStreet { get; init; }
        public string? Notes { get; init; }

        public List<UpdateEntityEmailRequest>? Emails { get; init; }
        public List<UpdateEntityPhoneRequest>? Phones { get; init; }
    }
}
