using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Users;

namespace sicoain.client.Abstractions
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<UserDto> GetCurrentUserAsync();
        Task<bool> RefreshTokenAsync();
        Task LogoutAsync();
    }
}
