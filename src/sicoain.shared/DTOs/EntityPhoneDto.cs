using sicoain.shared.Enums;

namespace sicoain.shared.DTOs
{
    public abstract record EntityPhoneDto
    {
        public int Id { get; init; }
        public required string PhoneNumber { get; init; }
        public bool IsMain { get; init; }
        public PhoneType PhoneType { get; init; } = PhoneType.Mobile;
    }
}
