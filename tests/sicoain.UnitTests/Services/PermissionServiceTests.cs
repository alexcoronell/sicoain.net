using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using sicoain.api.Abstractions;
using sicoain.api.Data;
using sicoain.api.Services;
using sicoain.shared.Entities;
using Xunit;

namespace sicoain.UnitTests.Services
{
    public class PermissionServiceTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole<int>>> _roleManagerMock;
        private readonly ApplicationDbContext _context;
        private readonly PermissionService _service;

        public PermissionServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            // Setup UserManager
            var userStoreMock = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            // Setup RoleManager
            var roleStoreMock = new Mock<IRoleStore<IdentityRole<int>>>();
            _roleManagerMock = new Mock<RoleManager<IdentityRole<int>>>(
                roleStoreMock.Object, null, null, null, null);

            _service = new PermissionService(_userManagerMock.Object, _roleManagerMock.Object, _context);
        }

        [Fact]
        public async Task GetUserPermissionNameAsync_WhenUserHasNoRoles_ReturnsEmptyList()
        {
            // Arrange
            var user = new User { Id = 1, FullName = "Test User" };
            _userManagerMock.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string>());

            // Act
            var result = await _service.GetUserPermissionNameAsync(user);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetUserPermissionNameAsync_WhenUserHasRoles_ReturnsDistinctPermissionNames()
        {
            // Arrange
            var user = new User { Id = 2, FullName = "Test User 2" };
            _userManagerMock.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "Admin", "Manager" });

            // Seed custom roles and permissions
            var customRoleAdmin = new Roles { Id = 1, Name = "Admin", IdentityRoleId = 1 };
            var customRoleManager = new Roles { Id = 2, Name = "Manager", IdentityRoleId = 2 };
            _context.CustomRoles.AddRange(customRoleAdmin, customRoleManager);

            var permission1 = new Permissions { Id = 10, Name = "Accidents.View", Module = "Accidents", Action = "View" };
            var permission2 = new Permissions { Id = 11, Name = "Employees.View", Module = "Employees", Action = "View" };
            _context.Permissions.AddRange(permission1, permission2);

            _context.RolePermissions.Add(new RolePermissions { RoleId = 1, PermissionId = 10 });
            _context.RolePermissions.Add(new RolePermissions { RoleId = 1, PermissionId = 11 });
            _context.RolePermissions.Add(new RolePermissions { RoleId = 2, PermissionId = 10 });

            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetUserPermissionNameAsync(user);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain("Accidents.View");
            result.Should().Contain("Employees.View");
        }
    }
}
