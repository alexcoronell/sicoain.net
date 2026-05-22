using System.Security.Claims;
using sicoain.shared.Entities;

namespace sicoain.api.Abstractions
{
    public interface IPermissionService
    {
        /// <summary>
        /// Gets the list of permission names (e.g., "Accidents.View") for a given user.
        /// </summary>
        Task<List<string>> GetUserPermissionNameAsync(User user);
    }
}
