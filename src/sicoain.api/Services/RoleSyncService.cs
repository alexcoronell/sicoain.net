using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Abstractions;
using sicoain.api.Data;
using sicoain.shared.Entities;

namespace sicoain.api.Services
{
    /// <summary>
    /// Synchronizes Identity roles with the custom Roles table.
    /// </summary>
    public class RoleSyncService : IRoleSyncService
    {
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly ApplicationDbContext _context;

        public RoleSyncService(RoleManager<IdentityRole<int>> roleManager, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _context = context;
        }

        public async Task SynchronizeRoleAsync()
        {
            // 1. Get all roles from Identity
            var identityRoles = await _roleManager.Roles.ToListAsync().ConfigureAwait(false);

            // 2. Get all custom roles from our custom Roles table
            var customRoles = await _context.CustomRoles.ToListAsync().ConfigureAwait(false);

            // 3. For each identity role, ensure there's a corresponding custom role
            foreach (var identityRole in identityRoles)
            {
                var existingCustomRole = customRoles.FirstOrDefault(r => r.IdentityRoleId == identityRole.Id);
                if (existingCustomRole == null)
                {
                    // Create missing custom role
                    var newCustomRole = new Roles
                    {
                        Name = identityRole.Name ?? string.Empty,
                        NormalizedName = identityRole.NormalizedName,
                        IdentityRoleId = identityRole.Id,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };
                    await _context.CustomRoles.AddAsync(newCustomRole).ConfigureAwait(false);
                }
                else
                {
                    // Update name if changed (optional)
                    if (existingCustomRole.Name != identityRole.Name)
                    {
                        existingCustomRole.Name = identityRole.Name ?? string.Empty;
                        existingCustomRole.NormalizedName = identityRole.NormalizedName;
                        existingCustomRole.UpdatedAt = DateTime.UtcNow;
                        _context.CustomRoles.Update(existingCustomRole);
                    }
                }
            }

            // 4. Remove custom roles that no longer exist in Identity (orphans)
            var identityRoleIds = identityRoles.Select(r => r.Id).ToHashSet();
            var orphans = customRoles.Where(r => !identityRoleIds.Contains(r.IdentityRoleId)).ToList();
            if (orphans.Any())
            {
                _context.CustomRoles.RemoveRange(orphans);
            }

            // 5. Save all changes
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
