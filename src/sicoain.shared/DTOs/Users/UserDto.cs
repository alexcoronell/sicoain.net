namespace sicoain.shared.DTOs.Users
{
    public class UserDto : BaseDto
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
