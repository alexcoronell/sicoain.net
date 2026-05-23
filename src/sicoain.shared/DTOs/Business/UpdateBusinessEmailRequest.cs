namespace sicoain.shared.DTOs.Business
{
    public record UpdateBusinessEmailRequest : UpdateEntityEmailRequest
    {
        public int BusinessId { get; init; }
    }
}
