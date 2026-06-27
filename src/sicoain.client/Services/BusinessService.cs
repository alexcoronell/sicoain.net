using System.Net;
using System.Net.Http.Json;
using sicoain.client.Abstractions;
using sicoain.client.Exceptions;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Business;

namespace sicoain.client.Services
{
    public class BusinessService
        : BaseService<BusinessDto, CreateBusinessRequest, UpdateBusinessEmailRequest>,
          IBusinessService
    {
        public BusinessService(HttpClient httpClient)
            : base(httpClient, "businesses")
        {
        }

        // Shadow base methods to satisfy IBusinessService with explicit interface types
        public new Task<PagedResponse<BusinessDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
            => base.GetAllAsync(pageNumber, pageSize);

        public new Task<BusinessDto?> GetByIdAsync(int id)
            => base.GetByIdAsync(id);

        public new Task<BusinessDto> CreateAsync(CreateBusinessRequest request)
            => base.CreateAsync(request);

        /// <summary>
        /// Updates a business. Uses <see cref="UpdateBusinessRequest"/> directly
        /// (different from the generic <c>UpdateBusinessEmailRequest</c>) via a
        /// dedicated PATCH call.
        /// </summary>
        public async Task<BusinessDto?> UpdateAsync(int id, UpdateBusinessRequest request)
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
                .ReadFromJsonAsync<BusinessDto>()
                .ConfigureAwait(false);
        }

        public new Task<bool> DeleteAsync(int id)
            => base.DeleteAsync(id);
    }
}
