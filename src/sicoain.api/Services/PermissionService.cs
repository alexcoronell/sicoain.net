using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Abstractions;
using sicoain.api.Data;
using sicoain.shared.Entities;

namespace sicoain.api.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly ApplicationDbContext _context;

        public PermissionService(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<List<string>> GetUserPermissionNameAsync(User user)
        {
            var userRoles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            var identityRoleNames = userRoles.ToList();

            if (!identityRoleNames.Any()) return new List<string>();

            var customRoles = await _context.CustomRoles
                .Where(cr => identityRoleNames.Contains(cr.Name))
                .Select(cr => new { cr.Id, cr.IdentityRoleId })
                .ToListAsync().ConfigureAwait(false);

            var customRoleIds = customRoles.Select(cr => cr.Id).ToList();

            if (!customRoleIds.Any()) return new List<string>();

            var permissionNames = await _context.RolePermissions
                .Where(rp => customRoleIds.Contains(rp.RoleId))
                .Include(rp => rp.Permission)
                .Select(rp => rp.Permission!.Name)
                .Distinct()
                .ToListAsync().ConfigureAwait(false);

            return permissionNames;
        }
    }
}
