namespace sicoain.shared.DTOs.RiskClasses
{
    public record UpdateRiskClassRequest
    {
        public string? Name { get; init; }
        public string? Code { get; init; }
        public decimal? ContributionRate { get; init; }
        public bool? IsActive { get; init; }
    }
}
