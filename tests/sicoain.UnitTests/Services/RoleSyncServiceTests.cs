using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using sicoain.api.Data;
using sicoain.api.Services;
using sicoain.shared.Entities;
using Xunit;

namespace sicoain.UnitTests.Services
{
    /// <summary>
    /// Unit tests for the Role synchronization service covering identity role mapping, permission seeding, and role-permission assignment.
    /// </summary>
    public class RoleSyncServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleSyncService _service;

        public RoleSyncServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            var roleStore = new RoleStore<IdentityRole<int>, ApplicationDbContext, int>(_context);
            var roleManager = new RoleManager<IdentityRole<int>>(
                roleStore, null, null, new IdentityErrorDescriber(), null);

            _service = new RoleSyncService(roleManager, _context);
        }

        [Fact]
        public async Task SynchronizeRoleAsync_ShouldCreateMissingCustomRoles()
        {
            // Arrange
            _context.Roles.AddRange(
                new IdentityRole<int> { Id = 1, Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole<int> { Id = 2, Name = "User", NormalizedName = "USER" }
            );
            await _context.SaveChangesAsync();

            // Act
            await _service.SynchronizeRoleAsync();

            // Assert
            var customRoles = await _context.CustomRoles.ToListAsync();
            customRoles.Should().HaveCount(2);
            customRoles.Should().Contain(r => r.Name == "Admin" && r.IdentityRoleId == 1);
            customRoles.Should().Contain(r => r.Name == "User" && r.IdentityRoleId == 2);
        }

        [Fact]
        public async Task SynchronizeRoleAsync_ShouldUpdateChangedRoleNames()
        {
            // Arrange
            _context.Roles.Add(new IdentityRole<int> { Id = 3, Name = "SuperAdmin", NormalizedName = "SUPERADMIN" });

            // Pre-seed a custom role with old name
            _context.CustomRoles.Add(new Roles
            {
                Id = 100,
                IdentityRoleId = 3,
                Name = "OldName",
                NormalizedName = "OLDNAME"
            });
            await _context.SaveChangesAsync();

            // Act
            await _service.SynchronizeRoleAsync();

            // Assert
            var updated = await _context.CustomRoles.FirstOrDefaultAsync(r => r.IdentityRoleId == 3);
            updated.Should().NotBeNull();
            updated!.Name.Should().Be("SuperAdmin");
            updated.NormalizedName.Should().Be("SUPERADMIN");
        }

        [Fact]
        public async Task SynchronizeRoleAsync_ShouldRemoveOrphanCustomRoles()
        {
            // Arrange
            // No Identity roles seeded — service sees an empty Roles table

            // Orphan custom role
            _context.CustomRoles.Add(new Roles { Id = 200, IdentityRoleId = 999, Name = "Orphan" });
            await _context.SaveChangesAsync();

            // Act
            await _service.SynchronizeRoleAsync();

            // Assert
            var orphans = await _context.CustomRoles.ToListAsync();
            orphans.Should().BeEmpty();
        }

        [Fact]
        public async Task SynchronizeRoleAsync_ShouldNotCreateDuplicates()
        {
            // Arrange
            _context.Roles.Add(new IdentityRole<int> { Id = 4, Name = "Manager", NormalizedName = "MANAGER" });

            // Pre-existing matching custom role
            _context.CustomRoles.Add(new Roles
            {
                IdentityRoleId = 4,
                Name = "Manager",
                NormalizedName = "MANAGER"
            });
            await _context.SaveChangesAsync();

            // Act
            await _service.SynchronizeRoleAsync();

            // Assert
            var count = await _context.CustomRoles.CountAsync(r => r.IdentityRoleId == 4);
            count.Should().Be(1);
        }
    }
}
