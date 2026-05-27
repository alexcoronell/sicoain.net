using Microsoft.EntityFrameworkCore;
using sicoain.shared.Constants;
using sicoain.shared.Entities;

namespace sicoain.api.Data.Seeders
{
    public static class PermissionSeeder
    {
        /// <summary>
        /// Ensures that all permissions defined in AppPermissions exist in the database.
        /// </summary>
        /// <param name="context">Application database context</param>
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Get all permission names from constants using reflection
            var permissionNames = typeof(AppPermissions)
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(f => f.FieldType == typeof(string))
                .Select(f => f.GetValue(null)?.ToString())
                .Where(v => !string.IsNullOrEmpty(v))
                .ToList();

            if (!permissionNames.Any())
                return;

            // Get existing permission names from database (only the Name property)
            var existingNames = await context.Permissions
                .Select(p => p.Name)
                .ToListAsync().ConfigureAwait(false);

            // Determine which permissions are missing
            var missingPermissions = permissionNames
                .Where(name => name != null && !existingNames.Contains(name))
                .ToList();

            if (!missingPermissions.Any())
                return;

            // Prepare new Permission entities
            var newPermissions = new List<Permissions>();
            foreach (var permName in missingPermissions)
            {
                // Optionally derive Module and Action from name (e.g., "Accidents.View")
                var parts = permName!.Split('.');
                var module = parts.Length > 0 ? parts[0] : string.Empty;
                var action = parts.Length > 1 ? parts[1] : string.Empty;

                newPermissions.Add(new Permissions
                {
                    Name = permName,
                    Module = module,
                    Action = action,
                    Description = $"Permission to {action} {module}" // Puedes mejorar la descripción
                });
            }
            await context.Permissions.AddRangeAsync(newPermissions).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
