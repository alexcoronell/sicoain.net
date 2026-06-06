using System.Net;
using System.Net.Http.Json;
using sicoain.client.Abstractions;
using sicoain.client.Constants;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Users;

namespace sicoain.client.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string path = $"{ApiPath.Path}/auth";

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{path}/login", request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("Correo o contraseña incorrectos.");
            }

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            return result ?? throw new InvalidOperationException("La respuesta del servidor fue nula o inválida");
        }

        public async Task<UserDto> GetCurrentUserAsync()
        {
            var response = await _httpClient.GetAsync($"{path}/me");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("usuario no autenticado.");
            }

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<UserDto>();
            return result ?? throw new InvalidOperationException("No se pudo obtener la información del usuario actual.");
        }

        public async Task<bool> RefreshTokenAsync()
        {
            var response = await _httpClient.PostAsync($"{path}/refresh", null);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("No se pudo renovar la sesión. Vuelve a iniciar sesión");
            }

            response.EnsureSuccessStatusCode();
            return true;
        }

        public async Task LogoutAsync()
        {
            var response = await _httpClient.PostAsync($"{path}/logout", null);
            response.EnsureSuccessStatusCode();
        }

    }
}
