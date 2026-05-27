using Microsoft.EntityFrameworkCore;
using sicoain.shared.Entities;

namespace sicoain.api.Data.Seeders
{
    public static class RolePermissionSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            var customRoles = await context.CustomRoles.ToListAsync().ConfigureAwait(false);
            var permissions = await context.Permissions.ToDictionaryAsync(p => p.Name, p => p.Id).ConfigureAwait(false);

            var rolePermissionsMapping = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["Admin"] = new List<string>(), // Se llenará con todos los permisos existentes
                ["Investigator"] = new List<string>
                {
                    "Accidents.View",
                    "Accidents.Create",
                    "Accidents.Edit",
                    "Reports.View",
                    "Employees.View",
                },
                ["Supervisor"] = new List<string>
                {
                    "Accidents.View",
                    "Employees.View",
                },
                ["Consultant"] = new List<string>
                {
                    "Reports.View",
                    "Accidents.View",
                },
            };

            // Asignar todos los permisos al rol Admin
            if (rolePermissionsMapping.ContainsKey("Admin") && permissions.Any())
            {
                rolePermissionsMapping["Admin"] = permissions.Keys.ToList();
            }

            // Lista para acumular nuevas relaciones
            var newRolePermissions = new List<RolePermissions>();

            foreach (var mapping in rolePermissionsMapping)
            {
                var roleName = mapping.Key;
                var permissionNames = mapping.Value;

                var customRole = customRoles.FirstOrDefault(r => string.Equals(r.Name, roleName, StringComparison.OrdinalIgnoreCase));
                if (customRole == null)
                {
                    Console.WriteLine($"Role '{roleName}' not found in CustomRoles. Skipping permission assignment.");
                    continue;
                }

                foreach (var permName in permissionNames)
                {
                    if (!permissions.TryGetValue(permName, out var permissionId))
                    {
                        Console.WriteLine($"Permission '{permName}' not found. Skipping.");
                        continue;
                    }

                    var exists = await context.RolePermissions
                        .AnyAsync(rp => rp.RoleId == customRole.Id && rp.PermissionId == permissionId).ConfigureAwait(false);

                    if (!exists)
                    {
                        newRolePermissions.Add(new RolePermissions
                        {
                            RoleId = customRole.Id,
                            PermissionId = permissionId,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = 0
                        });
                    }
                }
            }

            // Insertar todas las nuevas relaciones de una sola vez
            if (newRolePermissions.Any())
            {
                await context.RolePermissions.AddRangeAsync(newRolePermissions).ConfigureAwait(false);
                await context.SaveChangesAsync().ConfigureAwait(false);
                Console.WriteLine($"Added {newRolePermissions.Count} role-permission assignments.");
            }
            else
            {
                Console.WriteLine("No new role-permission assignments needed.");
            }
        }
    }
}
