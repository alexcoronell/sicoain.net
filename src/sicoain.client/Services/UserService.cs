using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using sicoain.client.Abstractions;
using sicoain.shared.DTOs.Users;

namespace sicoain.client.Services
{
    public class UserService : BaseService<UserDto, CreateUserRequest, UpdateUserRequest>, IUserService
    {
        public UserService(HttpClient httpClient)
            : base(httpClient, "user")
        {
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var response = await _httpClient
                .GetAsync($"{_endpointPath}/email/{email}")
                .ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content
                .ReadFromJsonAsync<UserDto>()
                .ConfigureAwait(false);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var response = await _httpClient
                .GetAsync($"{_endpointPath}/email-exists/{email}")
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var json = await response.Content
                .ReadFromJsonAsync<JsonElement>()
                .ConfigureAwait(false);

            return json.GetProperty("exists").GetBoolean();
        }

        public async Task<bool> AssignRoleAsync(int userId, string roleName)
        {
            var request = new AssignOrRemoveRoleRequest { RoleName = roleName };
            var response = await _httpClient
                .PatchAsJsonAsync($"{_endpointPath}/assign-role/{userId}", request)
                .ConfigureAwait(false);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveRoleAsync(int userId, string roleName)
        {
            var request = new AssignOrRemoveRoleRequest { RoleName = roleName };
            var response = await _httpClient
                .PatchAsJsonAsync($"{_endpointPath}/remove-role/{userId}", request)
                .ConfigureAwait(false);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            var response = await _httpClient
                .PatchAsJsonAsync($"{_endpointPath}/change-password/{userId}", request)
                .ConfigureAwait(false);

            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(int userId)
        {
            var response = await _httpClient
                .GetAsync($"{_endpointPath}/roles/{userId}")
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var result = await response.Content
                .ReadFromJsonAsync<IEnumerable<string>>()
                .ConfigureAwait(false);

            return result ?? Enumerable.Empty<string>();
        }
    }
}
