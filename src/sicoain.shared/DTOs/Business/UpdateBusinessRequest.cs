using sicoain.shared.DTOs;

namespace sicoain.shared.DTOs.Business
{
    public record UpdateBusinessRequest
    {
        public string? Name { get; init; }
        public string? AddressStreet { get; init; }
        public List<UpdateEntityEmailRequest>? Emails { get; init; }
        public List<UpdateEntityPhoneRequest>? Phones { get; init; }
    }
}
