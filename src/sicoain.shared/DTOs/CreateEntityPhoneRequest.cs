using System.ComponentModel.DataAnnotations;
using sicoain.shared.Enums;

namespace sicoain.shared.DTOs
{
    public record CreateEntityPhoneRequest
    {
        [Required, Range(1, int.MaxValue)]
        public required string Phone { get; init; }
        public bool IsMain { get; init; }
        public PhoneType PhoneType { get; init; } = PhoneType.Mobile;
    }
}
