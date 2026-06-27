namespace sicoain.shared.DTOs.Branches
{
    public record UpdateBranchRequest
    {
        public string? Name { get; init; }
        public string? AddressStreet { get; init; }
        public int? BusinessId { get; init; }
        public List<string>? Emails { get; init; }
        public List<string>? Phones { get; init; }
    }
}
