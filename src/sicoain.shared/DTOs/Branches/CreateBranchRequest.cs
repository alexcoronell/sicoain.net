using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.DTOs.Branches
{
    public record CreateBranchRequest
    {
        [Required, MinLength(3), MaxLength(100)]
        public string? Name { get; init; }
        public string? AddressStreet { get; init; }

        [Required, Range(1, int.MaxValue)]
        public int? BusinessId { get; init; }

        public List<CreateEntityEmailRequest>? Emails { get; init; }
        public List<CreateEntityPhoneRequest>? Phones { get; init; }
    }
}
