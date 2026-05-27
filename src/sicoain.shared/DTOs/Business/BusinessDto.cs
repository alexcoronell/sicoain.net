namespace sicoain.shared.DTOs.Business
{
    public record BusinessDto
    {
        public required string Name { get; init; }
        public required string Description { get; init; }
        public required string AddressStreet { get; init; }
    }
}
