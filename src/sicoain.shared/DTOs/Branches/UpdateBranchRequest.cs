using sicoain.shared.DTOs;

namespace sicoain.shared.DTOs.Branches
{
    public record UpdateBranchRequest
    {
        public string? Name { get; init; }
        public string? AddressStreet { get; init; }
        public int? BusinessId { get; init; }
        public List<UpdateEntityEmailRequest>? Emails { get; init; }
        public List<UpdateEntityPhoneRequest>? Phones { get; init; }
    }
}
