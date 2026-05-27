namespace sicoain.shared.DTOs.Users
{
    public record UserDto : BaseDto
    {
        public string Email { get; init; } = string.Empty;
        public string FullName { get; init; } = string.Empty;
        public bool IsActive { get; init; }
    }
}
