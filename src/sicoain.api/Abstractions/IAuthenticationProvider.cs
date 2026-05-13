using sicoain.shared.DTOs;

namespace sicoain.api.Abstractions
{
    public interface IAuthenticationProvider
    {
        Task<AuthResponse> AuthenticateAsync(LoginRequest request);
    }
}
