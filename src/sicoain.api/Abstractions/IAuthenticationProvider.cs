using sicoain.shared.DTOs;

namespace sicoain.api.Abstractions
{
    internal interface IAuthenticationProvider
    {
        Task<AuthResponse> AuthenticateAsync(LoginRequest request);
    }
}
