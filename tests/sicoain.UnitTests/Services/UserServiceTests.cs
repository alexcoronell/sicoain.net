using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Moq;
using sicoain.api.Abstractions;
using sicoain.api.Data;
using sicoain.api.Services;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Users;
using sicoain.shared.Entities;
using Xunit;

namespace sicoain.UnitTests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole<int>>> _roleManagerMock;
        private readonly IMapper _mapper;
        private readonly UserService _service;

        public UserServiceTests()
        {
            // Setup UserManager
            var userStoreMock = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            // Setup RoleManager
            var roleStoreMock = new Mock<IRoleStore<IdentityRole<int>>>();
            _roleManagerMock = new Mock<RoleManager<IdentityRole<int>>>(
                roleStoreMock.Object, null, null, null, null);

            // Setup AutoMapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDto>()
                    .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => !src.IsDeleted));
                cfg.CreateMap<CreateUserRequest, User>();
                cfg.CreateMap<UpdateUserRequest, User>();
            });
            _mapper = config.CreateMapper();

            _service = new UserService(_userManagerMock.Object, _roleManagerMock.Object, _mapper);
        }

        [Fact]
        public async Task GetByIdAsync_WhenUserExistsAndNotDeleted_ReturnsUserDto()
        {
            // Arrange
            var user = new User { Id = 5, Email = "test@test.com", FullName = "Test User", IsDeleted = false };
            _userManagerMock.Setup(x => x.FindByIdAsync("5"))
                .ReturnsAsync(user);

            // Act
            var result = await _service.GetByIdAsync(5);

            // Assert
            result.Should().NotBeNull();
            result!.Email.Should().Be("test@test.com");
            result.FullName.Should().Be("Test User");
            result.IsActive.Should().BeTrue();
        }

        [Fact]
        public async Task GetByIdAsync_WhenUserDeleted_ReturnsNull()
        {
            // Arrange
            var user = new User { Id = 6, FullName = "User Six", IsDeleted = true };
            _userManagerMock.Setup(x => x.FindByIdAsync("6"))
                .ReturnsAsync(user);

            // Act
            var result = await _service.GetByIdAsync(6);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_WithValidRequest_CreatesUserAndAssignsRoles()
        {
            // Arrange
            var request = new CreateUserRequest
            {
                Email = "new@test.com",
                Password = "Password123!",
                FullName = "New User",
                Roles = new List<string> { "Admin" }
            };
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), request.Password))
                .ReturnsAsync(IdentityResult.Success);
            _roleManagerMock.Setup(x => x.RoleExistsAsync("Admin"))
                .ReturnsAsync(true);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), "Admin"))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _service.CreateAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(request.Email);
            result.FullName.Should().Be(request.FullName);
            _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<User>(), request.Password), Times.Once);
            _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<User>(), "Admin"), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenCreationFails_ThrowsException()
        {
            // Arrange
            var request = new CreateUserRequest
            {
                Email = "fail@test.com",
                Password = "Pass123!",
                FullName = "Fail User"
            };
            var errors = new List<IdentityError> { new IdentityError { Description = "Error" } };
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), request.Password))
                .ReturnsAsync(IdentityResult.Failed(errors.ToArray()));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.CreateAsync(request));
        }

        [Fact]
        public async Task UpdateAsync_WhenUserExistsAndValid_UpdatesFields()
        {
            // Arrange
            var user = new User { Id = 7, Email = "old@test.com", FullName = "Old Name", IsDeleted = false };
            _userManagerMock.Setup(x => x.FindByIdAsync("7"))
                .ReturnsAsync(user);
            var updateRequest = new UpdateUserRequest { Email = "new@test.com", FullName = "New Name", IsActive = true };
            _userManagerMock.Setup(x => x.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _service.UpdateAsync(7, updateRequest);

            // Assert
            result.Should().NotBeNull();
            result!.Email.Should().Be("new@test.com");
            result.FullName.Should().Be("New Name");
            result.IsActive.Should().BeTrue();
            _userManagerMock.Verify(x => x.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenUserExists_SoftDeletesUser()
        {
            // Arrange
            var user = new User { Id = 8, FullName = "User Eight", IsDeleted = false };
            _userManagerMock.Setup(x => x.FindByIdAsync("8"))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _service.DeleteAsync(8);

            // Assert
            result.Should().BeTrue();
            user.IsDeleted.Should().BeTrue();
            user.DeletedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task AssignRoleAsync_WhenUserAndRoleExist_ReturnsTrue()
        {
            // Arrange
            var user = new User { Id = 9, FullName = "User Nine", IsDeleted = false };
            _userManagerMock.Setup(x => x.FindByIdAsync("9"))
                .ReturnsAsync(user);
            _roleManagerMock.Setup(x => x.RoleExistsAsync("Admin"))
                .ReturnsAsync(true);
            _userManagerMock.Setup(x => x.AddToRoleAsync(user, "Admin"))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _service.AssignRoleAsync(9, "Admin");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPagedUsers()
        {
            // Arrange
            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new ApplicationDbContext(contextOptions);
            context.Users.AddRange(
                new User { Id = 1, UserName = "a@b.com", Email = "a@b.com", FullName = "User A", IsDeleted = false },
                new User { Id = 2, UserName = "c@d.com", Email = "c@d.com", FullName = "User C", IsDeleted = false },
                new User { Id = 3, UserName = "e@f.com", Email = "e@f.com", FullName = "User E", IsDeleted = false }
            );
            await context.SaveChangesAsync();

            var userStore = new UserStore<User, IdentityRole<int>, ApplicationDbContext, int>(context);
            var userManager = new UserManager<User>(userStore, null, null, null, null, null, null, null, null);
            var service = new UserService(userManager, _roleManagerMock.Object, _mapper);

            // Act
            var result = await service.GetAllAsync(1, 10);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(3);
            result.TotalCount.Should().Be(3);
        }

        [Fact]
        public async Task GetAllAsync_ShouldExcludeDeletedUsers()
        {
            // Arrange
            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new ApplicationDbContext(contextOptions);
            context.Users.AddRange(
                new User { Id = 1, UserName = "active@b.com", Email = "active@b.com", FullName = "Active", IsDeleted = false },
                new User { Id = 2, UserName = "deleted@d.com", Email = "deleted@d.com", FullName = "Deleted", IsDeleted = true }
            );
            await context.SaveChangesAsync();

            var userStore = new UserStore<User, IdentityRole<int>, ApplicationDbContext, int>(context);
            var userManager = new UserManager<User>(userStore, null, null, null, null, null, null, null, null);
            var service = new UserService(userManager, _roleManagerMock.Object, _mapper);

            // Act
            var result = await service.GetAllAsync(1, 10);

            // Assert
            result.Items.Should().HaveCount(1);
            result.Items.Should().ContainSingle(u => u.Email == "active@b.com");
        }

        [Fact]
        public async Task GetByEmailAsync_WhenExists_ReturnsUser()
        {
            // Arrange
            var user = new User { Id = 10, Email = "find@me.com", FullName = "Find Me", IsDeleted = false };
            _userManagerMock.Setup(x => x.FindByEmailAsync("find@me.com"))
                .ReturnsAsync(user);

            // Act
            var result = await _service.GetByEmailAsync("find@me.com");

            // Assert
            result.Should().NotBeNull();
            result!.Email.Should().Be("find@me.com");
        }

        [Fact]
        public async Task GetByEmailAsync_WhenNotFound_ReturnsNull()
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _service.GetByEmailAsync("nonexistent@test.com");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task EmailExistsAsync_WhenExists_ReturnsTrue()
        {
            // Arrange
            var user = new User { Id = 1, Email = "exists@test.com", FullName = "Exists" };
            _userManagerMock.Setup(x => x.FindByEmailAsync("exists@test.com"))
                .ReturnsAsync(user);

            // Act
            var result = await _service.EmailExistsAsync("exists@test.com");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task EmailExistsAsync_WhenNotExists_ReturnsFalse()
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _service.EmailExistsAsync("nobody@test.com");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task RemoveRoleAsync_WhenUserAndRoleExist_ReturnsTrue()
        {
            // Arrange
            var user = new User { Id = 11, FullName = "User Eleven", IsDeleted = false };
            _userManagerMock.Setup(x => x.FindByIdAsync("11"))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.RemoveFromRoleAsync(user, "Editor"))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _service.RemoveRoleAsync(11, "Editor");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task RemoveRoleAsync_WhenUserDeleted_ReturnsFalse()
        {
            // Arrange
            var user = new User { Id = 12, FullName = "User Twelve", IsDeleted = true };
            _userManagerMock.Setup(x => x.FindByIdAsync("12"))
                .ReturnsAsync(user);

            // Act
            var result = await _service.RemoveRoleAsync(12, "Editor");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetUserRolesAsync_WhenUserExists_ReturnsRoles()
        {
            // Arrange
            var user = new User { Id = 13, FullName = "User Thirteen", IsDeleted = false };
            _userManagerMock.Setup(x => x.FindByIdAsync("13"))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "Admin", "User" });

            // Act
            var result = await _service.GetUserRolesAsync(13);

            // Assert
            result.Should().BeEquivalentTo(new[] { "Admin", "User" });
        }

        [Fact]
        public async Task GetUserRolesAsync_WhenUserDeleted_ReturnsEmpty()
        {
            // Arrange
            var user = new User { Id = 14, FullName = "User Fourteen", IsDeleted = true };
            _userManagerMock.Setup(x => x.FindByIdAsync("14"))
                .ReturnsAsync(user);

            // Act
            var result = await _service.GetUserRolesAsync(14);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task ChangePasswordAsync_WhenUserNotFoundOrDeleted_ReturnsFalse()
        {
            // Arrange - user not found
            _userManagerMock.Setup(x => x.FindByIdAsync("999"))
                .ReturnsAsync((User?)null);
            var request = new ChangePasswordRequest
            {
                CurrentPassword = "old",
                NewPassword = "new",
                ConfirmNewPassword = "new"
            };

            // Act
            var result = await _service.ChangePasswordAsync(999, request);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ChangePasswordAsync_WhenValid_ReturnsTrue()
        {
            // Arrange
            var user = new User { Id = 15, FullName = "User Fifteen", IsDeleted = false };
            _userManagerMock.Setup(x => x.FindByIdAsync("15"))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.ChangePasswordAsync(user, "old", "new"))
                .ReturnsAsync(IdentityResult.Success);
            var request = new ChangePasswordRequest
            {
                CurrentPassword = "old",
                NewPassword = "new",
                ConfirmNewPassword = "new"
            };

            // Act
            var result = await _service.ChangePasswordAsync(15, request);

            // Assert
            result.Should().BeTrue();
        }
    }
}
