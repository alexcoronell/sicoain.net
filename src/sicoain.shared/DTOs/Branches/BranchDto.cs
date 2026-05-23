namespace sicoain.shared.DTOs.Branch
{
    public record BranchDto : BaseDto
    {
        public string? Name { get; init; }
        public string? AddressStreet { get; init; }
        public int? BusinessId { get; init; }
        public string? BusinessName { get; init; }
    }
}
