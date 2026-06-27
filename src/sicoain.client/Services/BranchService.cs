using System.Net;
using System.Net.Http.Json;
using sicoain.client.Abstractions;
using sicoain.client.Exceptions;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Branches;

namespace sicoain.client.Services
{
    public class BranchService : BaseService<BranchDto, CreateBranchRequest, UpdateBranchRequest>, IBranchService
    {
        public BranchService(HttpClient httpClient) : base(httpClient, "branches")
        {

        }

        public new Task<PagedResponse<BranchDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10) => base.GetAllAsync(pageNumber, pageSize);

        public new Task<BranchDto?> GetByIdAsync(int id) => base.GetByIdAsync(id);

        public new Task<BranchDto> CreateAsync(CreateBranchRequest request) => base.CreateAsync(request);

        public new async Task<BranchDto?> UpdateAsync(int id, UpdateBranchRequest request)
        {
            var response = await _httpClient
                .PatchAsJsonAsync($"{_endpointPath}/{id}", request)
                .ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NotFound) return null;

            if (!response.IsSuccessStatusCode)
            {
                var error = await ReadErrorAsync(response).ConfigureAwait(false);
                throw new ApiException(error);
            }

            return await response.Content.ReadFromJsonAsync<BranchDto>().ConfigureAwait(false);
        }

        public new Task<bool> DeleteAsync(int id) => base.DeleteAsync(id);
    }
}
