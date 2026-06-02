namespace sicoain.shared.DTOs.Business
{
    public record BusinessDto : BaseDto
    {
        public required string Name { get; init; }
        public string? AddressStreet { get; init; }
    }
}
