using System.Net;
using System.Net.Http.Json;
using sicoain.client.Abstractions;
using sicoain.client.Constants;
using sicoain.shared.DTOs;

namespace sicoain.client.Services
{
    /// <summary>
    /// Generic base service implementing CRUD operations via HTTP calls to the API.
    /// </summary>
    /// <typeparam name="TDto">DTO type for responses</typeparam>
    /// <typeparam name="TCreateRequest">DTO type for create requests</typeparam>
    /// <typeparam name="TUpdateRequest">DTO type for update requests</typeparam>
    public abstract class BaseService<TDto, TCreateRequest, TUpdateRequest>
        : IBaseService<TDto, TCreateRequest, TUpdateRequest>
        where TDto : class
        where TCreateRequest : class
        where TUpdateRequest : class
    {
        protected readonly HttpClient _httpClient;
        protected readonly string _endpointPath;

        protected BaseService(HttpClient httpClient, string endpointPath)
        {
            _httpClient = httpClient;
            _endpointPath = $"{ApiPath.Path}/{endpointPath}";
        }

        /// <inheritdoc />
        public virtual async Task<PagedResponse<TDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            var response = await _httpClient
                .GetAsync($"{_endpointPath}?pageNumber={pageNumber}&pageSize={pageSize}")
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var result = await response.Content
                .ReadFromJsonAsync<PagedResponse<TDto>>()
                .ConfigureAwait(false);

            return result ?? throw new InvalidOperationException("La respuesta del servidor fue nula o inválida.");
        }

        /// <inheritdoc />
        public virtual async Task<TDto?> GetByIdAsync(int id)
        {
            var response = await _httpClient
                .GetAsync($"{_endpointPath}/{id}")
                .ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content
                .ReadFromJsonAsync<TDto>()
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public virtual async Task<TDto> CreateAsync(TCreateRequest request)
        {
            var response = await _httpClient
                .PostAsJsonAsync(_endpointPath, request)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var result = await response.Content
                .ReadFromJsonAsync<TDto>()
                .ConfigureAwait(false);

            return result ?? throw new InvalidOperationException("No se pudo crear el recurso.");
        }

        /// <inheritdoc />
        public virtual async Task<TDto?> UpdateAsync(int id, TUpdateRequest request)
        {
            var response = await _httpClient
                .PatchAsJsonAsync($"{_endpointPath}/{id}", request)
                .ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content
                .ReadFromJsonAsync<TDto>()
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public virtual async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient
                .DeleteAsync($"{_endpointPath}/{id}")
                .ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return false;

            response.EnsureSuccessStatusCode();
            return true;
        }
    }
}
