namespace sicoain.shared.DTOs.Business
{
    public record UpdateBusinessRequest
    {
        public string? Name { get; init; }
        public string? AddressStreet { get; init; }
    }
}
