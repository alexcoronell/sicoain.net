namespace sicoain.shared.DTOs
{
    public record AuthResponse(
        bool Success,
        string? Message,
        string? Email,
        string? FullName,
        DateTime? ExpiresAt
    );
}
