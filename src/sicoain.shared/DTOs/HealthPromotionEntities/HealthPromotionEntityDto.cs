namespace sicoain.shared.DTOs.HealthPromotionEntities
{
    public record HealthPromotionEntityDto : BaseDto
    {
        public string? Name { get; init; }
        public string? AddressStreet { get; init; }

        public string? Notes { get; init; }

        public List<HealthPromotionEntityEmailDto>? Emails { get; init; }
        public List<HealthPromotionEntityPhoneDto>? Phones { get; init; }
    }
}
