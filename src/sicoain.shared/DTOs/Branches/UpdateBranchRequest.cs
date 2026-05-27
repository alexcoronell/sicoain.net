namespace sicoain.shared.DTOs.Branches
{
    public record UpdateBranchRequest
    {
        public string? Name { get; init; }
        public string? AddressStreet { get; init; }
        public int? BusinessId { get; init; }
    }
}
