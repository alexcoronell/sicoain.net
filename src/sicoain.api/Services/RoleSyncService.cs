using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Abstractions;
using sicoain.api.Data;
using sicoain.shared.Entities;
using System.Linq;
using System.Threading.Tasks;

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

            // 2. Get all custom roles from our custom Roles table (DbSet CustomRoles)
            var customRoles = await _context.CustomRoles.ToListAsync().ConfigureAwait(false);  // <-- Usar CustomRoles

            // 3. For each identity role, ensure there's a corresponding custom role with the same Id
            foreach (var identityRole in identityRoles)
            {
                var existingCustomRole = customRoles.FirstOrDefault(r => r.Id == identityRole.Id);
                if (existingCustomRole == null)
                {
                    // Create missing custom role
                    var newCustomRole = new Roles
                    {
                        Id = identityRole.Id,          // Use same Id
                        Name = identityRole.Name ?? string.Empty,
                        NormalizedName = identityRole.NormalizedName,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };
                    await _context.CustomRoles.AddAsync(newCustomRole).ConfigureAwait(false);  // <-- Usar CustomRoles
                }
                else
                {
                    // Update name if changed (optional)
                    if (existingCustomRole.Name != identityRole.Name)
                    {
                        existingCustomRole.Name = identityRole.Name ?? string.Empty;
                        existingCustomRole.NormalizedName = identityRole.NormalizedName;
                        existingCustomRole.UpdatedAt = DateTime.UtcNow;
                        _context.CustomRoles.Update(existingCustomRole); // <-- Usar CustomRoles
                    }
                }
            }

            // 4. (Optional) Remove custom roles that no longer exist in Identity
            var customRoleIds = customRoles.Select(r => r.Id).ToList();
            var identityRoleIds = identityRoles.Select(r => r.Id).ToList();
            var orphans = customRoleIds.Where(id => !identityRoleIds.Contains(id)).ToList();
            if (orphans.Any())
            {
                var rolesToRemove = customRoles.Where(r => orphans.Contains(r.Id)).ToList();
                _context.CustomRoles.RemoveRange(rolesToRemove); // <-- Usar CustomRoles
            }

            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
