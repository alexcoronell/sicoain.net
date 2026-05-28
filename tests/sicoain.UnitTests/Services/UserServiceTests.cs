using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using sicoain.api.Abstractions;
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
    }
}
