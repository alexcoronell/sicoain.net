namespace sicoain.shared.DTOs.Business
{
    public record UpdateBusinessPhoneRequest : UpdateEntityPhoneRequest
    {
        public int BusinessId { get; init; }
    }
}
