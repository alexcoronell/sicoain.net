using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Abstractions;
using sicoain.api.Data;
using sicoain.shared.DTOs.Permissions;
using sicoain.shared.Entities;

namespace sicoain.api.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PermissionService(
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            ApplicationDbContext context,
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _mapper = mapper;
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

        public async Task<List<PermissionDto>> GetAllPermissionsAsync()
        {
            var permissions = await _context.Permissions
                .Where(p => !p.IsDeleted)
                .ToListAsync()
                .ConfigureAwait(false);

            return _mapper.Map<List<PermissionDto>>(permissions);
        }

        public async Task<List<string>> GetRolePermissionsAsync(int roleId)
        {
            var role = await _context.CustomRoles.FindAsync(roleId);
            if (role == null || role.IsDeleted)
                return new List<string>();

            return await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId && !rp.IsDeleted)
                .Include(rp => rp.Permission)
                .Where(rp => rp.Permission != null && !rp.Permission.IsDeleted)
                .Select(rp => rp.Permission!.Name)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<bool> AssignPermissionAsync(int roleId, string permissionName)
        {
            var permission = await _context.Permissions
                .FirstOrDefaultAsync(p => p.Name == permissionName && !p.IsDeleted)
                .ConfigureAwait(false);

            if (permission == null) return false;

            var exists = await _context.RolePermissions
                .AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permission.Id && !rp.IsDeleted)
                .ConfigureAwait(false);

            if (exists) return false;

            var rolePermission = new RolePermissions
            {
                RoleId = roleId,
                PermissionId = permission.Id
            };

            _context.RolePermissions.Add(rolePermission);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

        public async Task<bool> RemovePermissionAsync(int roleId, string permissionName)
        {
            var permission = await _context.Permissions
                .FirstOrDefaultAsync(p => p.Name == permissionName && !p.IsDeleted)
                .ConfigureAwait(false);

            if (permission == null) return false;

            var rolePermission = await _context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permission.Id && !rp.IsDeleted)
                .ConfigureAwait(false);

            if (rolePermission == null) return false;

            rolePermission.IsDeleted = true;
            rolePermission.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }
    }
}
