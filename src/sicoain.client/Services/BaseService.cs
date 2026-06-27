using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using sicoain.client.Abstractions;
using sicoain.client.Constants;
using sicoain.client.Exceptions;
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

            if (!response.IsSuccessStatusCode)
            {
                var error = await ReadErrorAsync(response).ConfigureAwait(false);
                throw new ApiException(error);
            }

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

            if (!response.IsSuccessStatusCode)
            {
                var error = await ReadErrorAsync(response).ConfigureAwait(false);
                throw new ApiException(error);
            }

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

        /// <summary>
        /// Reads a non-success HTTP response and extracts a user-facing error message
        /// from the body. Supports both <c>ValidationProblemDetails</c> (FluentValidation)
        /// and our custom <c>{ message: "..." }</c> format.
        /// </summary>
        protected static async Task<string> ReadErrorAsync(HttpResponseMessage response)
        {
            try
            {
                var body = await response.Content
                    .ReadAsStringAsync()
                    .ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(body))
                    return $"Error del servidor: {(int)response.StatusCode}";

                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;

                // Try our custom { message: "..." } format first (ConflictException, BadRequest)
                if (root.TryGetProperty("message", out var msgProp) &&
                    msgProp.ValueKind == JsonValueKind.String)
                {
                    var msg = msgProp.GetString();
                    if (!string.IsNullOrWhiteSpace(msg))
                        return msg;
                }

                // Try ValidationProblemDetails { errors: { field: [msg, ...] } } format
                if (root.TryGetProperty("errors", out var errorsProp) &&
                    errorsProp.ValueKind == JsonValueKind.Object)
                {
                    var messages = new List<string>();
                    foreach (var field in errorsProp.EnumerateObject())
                    {
                        if (field.Value.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var err in field.Value.EnumerateArray())
                            {
                                if (err.ValueKind == JsonValueKind.String)
                                {
                                    var text = err.GetString();
                                    if (!string.IsNullOrWhiteSpace(text))
                                        messages.Add(text);
                                }
                            }
                        }
                    }

                    if (messages.Count > 0)
                        return string.Join(Environment.NewLine, messages.Distinct());
                }

                // Try ProblemDetails { title: "..." } format (fallback)
                if (root.TryGetProperty("title", out var titleProp) &&
                    titleProp.ValueKind == JsonValueKind.String)
                {
                    return titleProp.GetString() ?? $"Error del servidor: {(int)response.StatusCode}";
                }
            }
            catch
            {
                // If we can't parse the error body, fall through to generic message
            }

            return $"Error del servidor: {(int)response.StatusCode}";
        }
    }
}
