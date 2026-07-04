using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs.Business
{
    public record CreateBusinessRequest
    {
        [Required, MinLength(3), MaxLength(100)]
        public required string Name { get; init; }
        public string? AddressStreet { get; init; }
        public List<CreateEntityEmailRequest>? Emails { get; init; }
        public List<CreateEntityPhoneRequest>? Phones { get; init; }
    }
}
