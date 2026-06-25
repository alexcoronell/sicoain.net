using System.Net.Http.Json;
using sicoain.client.Abstractions;
using sicoain.client.Constants;
using sicoain.shared.DTOs.Permissions;

namespace sicoain.client.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _basePath = $"{ApiPath.Path}/Roles";

        public PermissionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<PermissionDto>> GetAllAsync()
        {
            var response = await _httpClient
                .GetAsync($"{_basePath}/permissions/catalog")
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var result = await response.Content
                .ReadFromJsonAsync<List<PermissionDto>>()
                .ConfigureAwait(false);

            return result ?? [];
        }

        public async Task<List<string>> GetRolePermissionsAsync(int roleId)
        {
            var response = await _httpClient
                .GetAsync($"{_basePath}/{roleId}/permissions")
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var result = await response.Content
                .ReadFromJsonAsync<List<string>>()
                .ConfigureAwait(false);

            return result ?? [];
        }

        public async Task<bool> AssignPermissionAsync(int roleId, string permissionName)
        {
            var request = new AssignPermissionRequest { PermissionName = permissionName };
            var response = await _httpClient
                .PostAsJsonAsync($"{_basePath}/{roleId}/permissions", request)
                .ConfigureAwait(false);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemovePermissionAsync(int roleId, string permissionName)
        {
            var response = await _httpClient
                .DeleteAsync($"{_basePath}/{roleId}/permissions/{permissionName}")
                .ConfigureAwait(false);

            return response.IsSuccessStatusCode;
        }
    }
}
