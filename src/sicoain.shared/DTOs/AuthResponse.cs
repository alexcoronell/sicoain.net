namespace sicoain.shared.DTOs
{
    public class AuthResponse
    {
        public bool Success { get; set; }

        public string? Message { get; set; }

        public string? Email { get; set; }

        public string? FullName { get; set; }

        public DateTime ExpiresAt { get; set; }
    }
}
