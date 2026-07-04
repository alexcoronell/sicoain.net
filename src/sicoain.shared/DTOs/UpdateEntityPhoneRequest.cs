using System.ComponentModel.DataAnnotations;
using sicoain.shared.Enums;

namespace sicoain.shared.DTOs
{
    public record UpdateEntityPhoneRequest
    {
        public int? Id { get; init; }
        [Required, MaxLength(20)]
        public required string Phone { get; init; }
        public bool IsMain { get; init; }
        public PhoneType PhoneType { get; init; } = PhoneType.Mobile;
    }
}
