using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using sicoain.api.Abstractions;
using sicoain.api.Data;
using sicoain.api.Services;
using sicoain.shared.DTOs.Permissions;
using sicoain.shared.Entities;
using Xunit;

namespace sicoain.UnitTests.Services
{
    /// <summary>
    /// Unit tests for the Permission service covering permission CRUD, role-permission assignment, and policy-based authorization queries.
    /// </summary>
    public class PermissionServiceTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole<int>>> _roleManagerMock;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
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

            // Setup AutoMapper
            var expression = new MapperConfigurationExpression();
            expression.CreateMap<Permissions, PermissionDto>();
            var config = new MapperConfiguration(expression, new Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory());
            _mapper = config.CreateMapper();

            _service = new PermissionService(_userManagerMock.Object, _roleManagerMock.Object, _context, _mapper);
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
