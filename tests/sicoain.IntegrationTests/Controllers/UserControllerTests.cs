using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using sicoain.IntegrationTests.Fixtures;
using sicoain.IntegrationTests.Utilities;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Users;

namespace sicoain.IntegrationTests.Controllers;

public class UserControllerTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly CookieHandler _cookieHandler;

    public UserControllerTests(IntegrationTestWebAppFactory factory)
    {
        _cookieHandler = new CookieHandler();
        _client = factory.CreateDefaultClient(_cookieHandler);
        // Autenticarse como admin antes de cada prueba
        AuthenticateAsync().GetAwaiter().GetResult();
    }

    private async Task AuthenticateAsync()
    {
        var loginRequest = new { Email = "admin@test.com", Password = "Admin123!" };
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/login", loginRequest);
        response.EnsureSuccessStatusCode();
    }

    // 14. CreateUser_WithValidData_ReturnsCreated
    [Fact]
    public async Task CreateUser_WithValidData_ReturnsCreated()
    {
        var request = new CreateUserRequest
        {
            Email = "newuser@test.com",
            Password = "ValidPass123!",
            FullName = "New User",
            Roles = new List<string> { "Consultant" }
        };

        var response = await _client.PostAsJsonAsync("/api/v1/User", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var userDto = await response.Content.ReadFromJsonAsync<UserDto>();
        userDto.Should().NotBeNull();
        userDto!.Email.Should().Be("newuser@test.com");
        userDto.FullName.Should().Be("New User");
        userDto.IsActive.Should().BeTrue();
    }

    // 15. CreateUser_WithDuplicateEmail_ReturnsConflict
    [Fact]
    public async Task CreateUser_WithDuplicateEmail_ReturnsConflict()
    {
        // First create a user
        var request = new CreateUserRequest
        {
            Email = "duplicate@test.com",
            Password = "ValidPass123!",
            FullName = "Duplicate User",
            Roles = new List<string>()
        };
        await _client.PostAsJsonAsync("/api/v1/User", request);

        // Try to create another with same email
        var duplicateRequest = new CreateUserRequest
        {
            Email = "duplicate@test.com",
            Password = "AnotherPass123!",
            FullName = "Another User",
            Roles = new List<string>()
        };
        var response = await _client.PostAsJsonAsync("/api/v1/User", duplicateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    // 16. CreateUser_WithWeakPassword_ReturnsBadRequest
    [Fact]
    public async Task CreateUser_WithWeakPassword_ReturnsBadRequest()
    {
        var request = new CreateUserRequest
        {
            Email = "weak@test.com",
            Password = "weak", // too short, no digits/uppercase
            FullName = "Weak Password User",
            Roles = new List<string>()
        };

        var response = await _client.PostAsJsonAsync("/api/v1/User", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 17. CreateUser_WithoutRequiredFields_ReturnsBadRequest
    [Fact]
    public async Task CreateUser_WithoutRequiredFields_ReturnsBadRequest()
    {
        var request = new { Email = "missingfields@test.com", Password = "ValidPass123!" };
        var response = await _client.PostAsJsonAsync("/api/v1/User", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 18. CreateUser_WithRoleAssignment_AssignsRole
    [Fact]
    public async Task CreateUser_WithRoleAssignment_AssignsRole()
    {
        var request = new CreateUserRequest
        {
            Email = "roleuser@test.com",
            Password = "ValidPass123!",
            FullName = "Role User",
            Roles = new List<string> { "Investigator" }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/User", request);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var userDto = await createResponse.Content.ReadFromJsonAsync<UserDto>();

        // Verify roles
        var rolesResponse = await _client.GetAsync($"/api/v1/User/roles/{userDto!.Id}");
        rolesResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var roles = await rolesResponse.Content.ReadFromJsonAsync<List<string>>();
        roles.Should().Contain("Investigator");
    }

    // 19. GetUserById_WithExistingUser_ReturnsUser
    [Fact]
    public async Task GetUserById_WithExistingUser_ReturnsUser()
    {
        // Create a user first
        var request = new CreateUserRequest
        {
            Email = "getbyid@test.com",
            Password = "ValidPass123!",
            FullName = "Get By Id",
            Roles = new List<string>()
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/User", request);
        var userDto = await createResponse.Content.ReadFromJsonAsync<UserDto>();

        var response = await _client.GetAsync($"/api/v1/User/{userDto!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetched = await response.Content.ReadFromJsonAsync<UserDto>();
        fetched.Should().NotBeNull();
        fetched!.Id.Should().Be(userDto.Id);
        fetched.Email.Should().Be("getbyid@test.com");
    }

    // 20. GetUserById_WithNonExistentId_ReturnsNotFound
    [Fact]
    public async Task GetUserById_WithNonExistentId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v1/User/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // 21. GetUserById_WithSoftDeletedUser_ReturnsNotFound
    [Fact]
    public async Task GetUserById_WithSoftDeletedUser_ReturnsNotFound()
    {
        // Create user
        var request = new CreateUserRequest
        {
            Email = "softdelete@test.com",
            Password = "ValidPass123!",
            FullName = "To Be Deleted",
            Roles = new List<string>()
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/User", request);
        var userDto = await createResponse.Content.ReadFromJsonAsync<UserDto>();

        // Soft delete
        await _client.DeleteAsync($"/api/v1/User/{userDto!.Id}");

        // Try to get by id
        var getResponse = await _client.GetAsync($"/api/v1/User/{userDto.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // 22. GetAllUsers_ReturnsPaginatedList
    [Fact]
    public async Task GetAllUsers_ReturnsPaginatedList()
    {
        var response = await _client.GetAsync("/api/v1/User?pageNumber=1&pageSize=5");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<UserDto>>();
        pagedResponse.Should().NotBeNull();
        pagedResponse!.Items.Should().NotBeNull();
        pagedResponse.PageNumber.Should().Be(1);
        pagedResponse.PageSize.Should().Be(5);
        pagedResponse.TotalCount.Should().BeGreaterThanOrEqualTo(1); // at least admin user
    }

    // 23. GetAllUsers_ExcludesSoftDeletedUsers
    [Fact]
    public async Task GetAllUsers_ExcludesSoftDeletedUsers()
    {
        // Create a user
        var request = new CreateUserRequest
        {
            Email = "exclude@test.com",
            Password = "ValidPass123!",
            FullName = "Exclude Me",
            Roles = new List<string>()
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/User", request);
        var userDto = await createResponse.Content.ReadFromJsonAsync<UserDto>();

        // Delete it
        await _client.DeleteAsync($"/api/v1/User/{userDto!.Id}");

        // Get all and verify it's not there
        var getAllResponse = await _client.GetAsync("/api/v1/User?pageNumber=1&pageSize=100");
        var pagedResponse = await getAllResponse.Content.ReadFromJsonAsync<PagedResponse<UserDto>>();
        pagedResponse!.Items.Should().NotContain(u => u.Id == userDto.Id);
    }

    // 24. GetUserByEmail_WithExistingEmail_ReturnsUser
    [Fact]
    public async Task GetUserByEmail_WithExistingEmail_ReturnsUser()
    {
        var response = await _client.GetAsync("/api/v1/User/email/admin@test.com");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var userDto = await response.Content.ReadFromJsonAsync<UserDto>();
        userDto.Should().NotBeNull();
        userDto!.Email.Should().Be("admin@test.com");
    }

    // 25. GetUserByEmail_WithNonExistentEmail_ReturnsNotFound
    [Fact]
    public async Task GetUserByEmail_WithNonExistentEmail_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v1/User/email/nonexistent@test.com");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // 26. EmailExists_WithExistingEmail_ReturnsTrue
    [Fact]
    public async Task EmailExists_WithExistingEmail_ReturnsTrue()
    {
        var response = await _client.GetAsync("/api/v1/User/email-exists/admin@test.com");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, bool>>();
        result!["exists"].Should().BeTrue();
    }

    // 27. EmailExists_WithNonExistentEmail_ReturnsFalse
    [Fact]
    public async Task EmailExists_WithNonExistentEmail_ReturnsFalse()
    {
        var response = await _client.GetAsync("/api/v1/User/email-exists/notexist@test.com");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, bool>>();
        result!["exists"].Should().BeFalse();
    }

    // 28. UpdateUser_WithValidData_UpdatesFields
    [Fact]
    public async Task UpdateUser_WithValidData_UpdatesFields()
    {
        // Create user
        var request = new CreateUserRequest
        {
            Email = "update@test.com",
            Password = "ValidPass123!",
            FullName = "Original Name",
            Roles = new List<string>()
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/User", request);
        var userDto = await createResponse.Content.ReadFromJsonAsync<UserDto>();

        // Update
        var updateRequest = new UpdateUserRequest
        {
            FullName = "Updated Name",
            IsActive = false
        };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/v1/User/{userDto!.Id}", updateRequest);
        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<UserDto>();
        updated!.FullName.Should().Be("Updated Name");
        updated.IsActive.Should().BeFalse();
    }

    // 29. UpdateUser_WithNonExistentId_ReturnsNotFound
    [Fact]
    public async Task UpdateUser_WithNonExistentId_ReturnsNotFound()
    {
        var updateRequest = new UpdateUserRequest { FullName = "Doesn't Matter" };
        var response = await _client.PatchAsJsonAsync("/api/v1/User/99999", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // 30. UpdateUser_ToDuplicateEmail_ReturnsConflict
    [Fact]
    public async Task UpdateUser_ToDuplicateEmail_ReturnsConflict()
    {
        // Create two users
        var user1 = new CreateUserRequest { Email = "user1@test.com", Password = "Pass123!", FullName = "User One", Roles = new List<string>() };
        var user2 = new CreateUserRequest { Email = "user2@test.com", Password = "Pass123!", FullName = "User Two", Roles = new List<string>() };
        var resp1 = await _client.PostAsJsonAsync("/api/v1/User", user1);
        var resp2 = await _client.PostAsJsonAsync("/api/v1/User", user2);
        var user2Dto = await resp2.Content.ReadFromJsonAsync<UserDto>();

        // Update user2 with email of user1
        var updateRequest = new UpdateUserRequest { Email = "user1@test.com" };
        var response = await _client.PatchAsJsonAsync($"/api/v1/User/{user2Dto!.Id}", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    // 31. AssignRole_ToExistingUser_ReturnsOk
    [Fact]
    public async Task AssignRole_ToExistingUser_ReturnsOk()
    {
        // Create user
        var request = new CreateUserRequest
        {
            Email = "assignrole@test.com",
            Password = "ValidPass123!",
            FullName = "Assign Role",
            Roles = new List<string>()
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/User", request);
        var userDto = await createResponse.Content.ReadFromJsonAsync<UserDto>();

        var assignRequest = new AssignOrRemoveRoleRequest { RoleName = "Supervisor" };
        var response = await _client.PatchAsJsonAsync($"/api/v1/User/assign-role/{userDto!.Id}", assignRequest);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // 32. AssignRole_WithInvalidRole_ReturnsBadRequest
    [Fact]
    public async Task AssignRole_WithInvalidRole_ReturnsBadRequest()
    {
        // Create user
        var request = new CreateUserRequest
        {
            Email = "invalidrole@test.com",
            Password = "ValidPass123!",
            FullName = "Invalid Role User",
            Roles = new List<string>()
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/User", request);
        var userDto = await createResponse.Content.ReadFromJsonAsync<UserDto>();

        var assignRequest = new AssignOrRemoveRoleRequest { RoleName = "NonExistentRole" };
        var response = await _client.PatchAsJsonAsync($"/api/v1/User/assign-role/{userDto!.Id}", assignRequest);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 33. RemoveRole_FromUser_RemovesAssignment
    [Fact]
    public async Task RemoveRole_FromUser_RemovesAssignment()
    {
        // Create user with role
        var request = new CreateUserRequest
        {
            Email = "removerole@test.com",
            Password = "ValidPass123!",
            FullName = "Remove Role",
            Roles = new List<string> { "Investigator" }
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/User", request);
        var userDto = await createResponse.Content.ReadFromJsonAsync<UserDto>();

        // Remove role
        var removeRequest = new AssignOrRemoveRoleRequest { RoleName = "Investigator" };
        var response = await _client.PatchAsJsonAsync($"/api/v1/User/remove-role/{userDto!.Id}", removeRequest);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify role removed
        var rolesResponse = await _client.GetAsync($"/api/v1/User/roles/{userDto.Id}");
        var roles = await rolesResponse.Content.ReadFromJsonAsync<List<string>>();
        roles.Should().NotContain("Investigator");
    }

    // 34. ChangePassword_WithValidCurrentPassword_ReturnsOk
    [Fact]
    public async Task ChangePassword_WithValidCurrentPassword_ReturnsOk()
    {
        // Create user with known password
        var email = $"changepwd{Guid.NewGuid():N}@test.com";
        var password = "OldPass123!";
        var request = new CreateUserRequest
        {
            Email = email,
            Password = password,
            FullName = "Change Password",
            Roles = new List<string>()
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/User", request);
        var userDto = await createResponse.Content.ReadFromJsonAsync<UserDto>();

        var changeRequest = new ChangePasswordRequest
        {
            CurrentPassword = password,
            NewPassword = "NewPass123!",
            ConfirmNewPassword = "NewPass123!"
        };
        var response = await _client.PatchAsJsonAsync($"/api/v1/User/change-password/{userDto!.Id}", changeRequest);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // 35. ChangePassword_WithWrongCurrentPassword_ReturnsBadRequest
    [Fact]
    public async Task ChangePassword_WithWrongCurrentPassword_ReturnsBadRequest()
    {
        var email = $"wrongpwd{Guid.NewGuid():N}@test.com";
        var request = new CreateUserRequest
        {
            Email = email,
            Password = "ValidPass123!",
            FullName = "Wrong Current",
            Roles = new List<string>()
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/User", request);
        var userDto = await createResponse.Content.ReadFromJsonAsync<UserDto>();

        var changeRequest = new ChangePasswordRequest
        {
            CurrentPassword = "WrongPassword",
            NewPassword = "NewPass123!",
            ConfirmNewPassword = "NewPass123!"
        };
        var response = await _client.PatchAsJsonAsync($"/api/v1/User/change-password/{userDto!.Id}", changeRequest);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 36. ChangePassword_WithWeakNewPassword_ReturnsBadRequest
    [Fact]
    public async Task ChangePassword_WithWeakNewPassword_ReturnsBadRequest()
    {
        var email = $"weaknew{Guid.NewGuid():N}@test.com";
        var password = "ValidPass123!";
        var request = new CreateUserRequest
        {
            Email = email,
            Password = password,
            FullName = "Weak New",
            Roles = new List<string>()
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/User", request);
        var userDto = await createResponse.Content.ReadFromJsonAsync<UserDto>();

        var changeRequest = new ChangePasswordRequest
        {
            CurrentPassword = password,
            NewPassword = "weak",
            ConfirmNewPassword = "weak"
        };
        var response = await _client.PatchAsJsonAsync($"/api/v1/User/change-password/{userDto!.Id}", changeRequest);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 37. DeleteUser_SoftDeletesUser
    [Fact]
    public async Task DeleteUser_SoftDeletesUser()
    {
        var request = new CreateUserRequest
        {
            Email = $"softdelete{Guid.NewGuid():N}@test.com",
            Password = "ValidPass123!",
            FullName = "Soft Delete",
            Roles = new List<string>()
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/User", request);
        var userDto = await createResponse.Content.ReadFromJsonAsync<UserDto>();

        var deleteResponse = await _client.DeleteAsync($"/api/v1/User/{userDto!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify not found
        var getResponse = await _client.GetAsync($"/api/v1/User/{userDto.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // 38. DeleteUser_AlreadyDeleted_ReturnsNotFound
    [Fact]
    public async Task DeleteUser_AlreadyDeleted_ReturnsNotFound()
    {
        var request = new CreateUserRequest
        {
            Email = $"alreadydel{Guid.NewGuid():N}@test.com",
            Password = "ValidPass123!",
            FullName = "Already Deleted",
            Roles = new List<string>()
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/User", request);
        var userDto = await createResponse.Content.ReadFromJsonAsync<UserDto>();

        await _client.DeleteAsync($"/api/v1/User/{userDto!.Id}");
        var secondDelete = await _client.DeleteAsync($"/api/v1/User/{userDto.Id}");
        secondDelete.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // 39. GetUserRoles_ReturnsListOfRoles
    [Fact]
    public async Task GetUserRoles_ReturnsListOfRoles()
    {
        // Create user with multiple roles
        var request = new CreateUserRequest
        {
            Email = $"roles{Guid.NewGuid():N}@test.com",
            Password = "ValidPass123!",
            FullName = "Roles Test",
            Roles = new List<string> { "Admin", "Investigator" }
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/User", request);
        var userDto = await createResponse.Content.ReadFromJsonAsync<UserDto>();

        var response = await _client.GetAsync($"/api/v1/User/roles/{userDto!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var roles = await response.Content.ReadFromJsonAsync<List<string>>();
        roles.Should().Contain("Admin");
        roles.Should().Contain("Investigator");
    }
}
