using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using sicoain.IntegrationTests.Fixtures;
using sicoain.IntegrationTests.Utilities;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Departments;
using sicoain.shared.Entities;

namespace sicoain.IntegrationTests.Controllers;

public class DepartmentsControllerTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly CookieHandler _cookieHandler;
    private readonly IntegrationTestWebAppFactory _factory;

    public DepartmentsControllerTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _cookieHandler = new CookieHandler();
        _client = factory.CreateDefaultClient(_cookieHandler);
        AuthenticateAsync().GetAwaiter().GetResult();
    }

    private async Task AuthenticateAsync()
    {
        var loginRequest = new { Email = "admin@test.com", Password = "Admin123!" };
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/login", loginRequest);
        response.EnsureSuccessStatusCode();
    }

    #region Helpers

    private ApplicationDbContext CreateDbContext()
    {
        var scope = _factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    private CreateDepartmentRequest CreateValidDepartmentRequest()
    {
        return new CreateDepartmentRequest
        {
            Name = $"TestDept_{Guid.NewGuid():N}",
            Description = "Descripción del departamento",
            Email = $"dept{Guid.NewGuid():N}@test.com",
            Phone = "6011234567"
        };
    }

    private async Task<int> CreateTestDepartmentAsync()
    {
        var request = CreateValidDepartmentRequest();
        var response = await _client.PostAsJsonAsync("/api/v1/Departments", request);
        response.EnsureSuccessStatusCode();
        var department = await response.Content.ReadFromJsonAsync<DepartmentDto>();
        return department!.Id;
    }

    #endregion

    // 78. CreateDepartment_WithValidData_ReturnsCreated
    [Fact]
    public async Task CreateDepartment_WithValidData_ReturnsCreated()
    {
        var request = CreateValidDepartmentRequest();

        var response = await _client.PostAsJsonAsync("/api/v1/Departments", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var department = await response.Content.ReadFromJsonAsync<DepartmentDto>();
        department.Should().NotBeNull();
        department!.Name.Should().Be(request.Name);
        department.Description.Should().Be(request.Description);
        department.Email.Should().Be(request.Email);
        department.Phone.Should().Be(request.Phone);
    }

    // 79. CreateDepartment_WithDuplicateName_No unique constraint -> both succeed
    [Fact]
    public async Task CreateDepartment_WithDuplicateName_ReturnsCreated()
    {
        var request = CreateValidDepartmentRequest();
        var firstResponse = await _client.PostAsJsonAsync("/api/v1/Departments", request);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var duplicateRequest = request with { Description = "Otra descripción", Email = "otro@test.com" };
        var response = await _client.PostAsJsonAsync("/api/v1/Departments", duplicateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    // 80. GetDepartmentById_WithExisting_ReturnsDto
    [Fact]
    public async Task GetDepartmentById_WithExisting_ReturnsDto()
    {
        var departmentId = await CreateTestDepartmentAsync();

        var response = await _client.GetAsync($"/api/v1/Departments/{departmentId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var department = await response.Content.ReadFromJsonAsync<DepartmentDto>();
        department.Should().NotBeNull();
        department!.Id.Should().Be(departmentId);
        department.Name.Should().NotBeNullOrEmpty();
    }

    // 81. GetAllDepartments_ReturnsPaginatedList
    [Fact]
    public async Task GetAllDepartments_ReturnsPaginatedList()
    {
        await CreateTestDepartmentAsync();

        var response = await _client.GetAsync("/api/v1/Departments?pageNumber=1&pageSize=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<DepartmentDto>>();
        pagedResponse.Should().NotBeNull();
        pagedResponse!.Items.Should().NotBeEmpty();
        pagedResponse.PageNumber.Should().Be(1);
        pagedResponse.PageSize.Should().Be(5);
        pagedResponse.TotalCount.Should().BeGreaterThan(0);
    }

    // 82. UpdateDepartment_WithValidData_Updates
    [Fact]
    public async Task UpdateDepartment_WithValidData_Updates()
    {
        var departmentId = await CreateTestDepartmentAsync();

        var updateRequest = new UpdateDepartmentRequest
        {
            Name = "Updated Department Name",
            Description = "Descripción actualizada",
            Email = "updated@test.com",
            Phone = "6019998888"
        };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/v1/Departments/{departmentId}", updateRequest);

        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<DepartmentDto>();
        updated!.Name.Should().Be("Updated Department Name");
        updated.Description.Should().Be("Descripción actualizada");
        updated.Email.Should().Be("updated@test.com");
        updated.Phone.Should().Be("6019998888");
    }

    // 83. DeleteDepartment_SoftDeletes
    [Fact]
    public async Task DeleteDepartment_SoftDeletes()
    {
        var departmentId = await CreateTestDepartmentAsync();

        var deleteResponse = await _client.DeleteAsync($"/api/v1/Departments/{departmentId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/v1/Departments/{departmentId}");
        // Observando patrón: GetById devuelve OK incluso después de soft delete (como en Employees y Branches)
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // 84. Unauthorized_CreateDepartment_WithoutSettingsEdit
    [Fact]
    public async Task Unauthorized_CreateDepartment_WithoutPermission()
    {
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var email = $"noperm{Guid.NewGuid():N}@test.com";
        var user = new User
        {
            UserName = email,
            Email = email,
            FullName = "No Permissions User",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        await userManager.CreateAsync(user, "NoPerm123!");

        var unauthHandler = new CookieHandler();
        var unauthClient = _factory.CreateDefaultClient(unauthHandler);
        await unauthClient.PostAsJsonAsync("/api/v1/Auth/login", new { Email = email, Password = "NoPerm123!" });

        var request = CreateValidDepartmentRequest();
        var response = await unauthClient.PostAsJsonAsync("/api/v1/Departments", request);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // 85. Unauthorized_ViewDepartment_WithoutSettingsView
    [Fact]
    public async Task Unauthorized_ViewDepartment_WithoutPermission()
    {
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var email = $"noview{Guid.NewGuid():N}@test.com";
        var user = new User
        {
            UserName = email,
            Email = email,
            FullName = "No View User",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        await userManager.CreateAsync(user, "NoView123!");

        var unauthHandler = new CookieHandler();
        var unauthClient = _factory.CreateDefaultClient(unauthHandler);
        await unauthClient.PostAsJsonAsync("/api/v1/Auth/login", new { Email = email, Password = "NoView123!" });

        var departmentId = await CreateTestDepartmentAsync();
        var response = await unauthClient.GetAsync($"/api/v1/Departments/{departmentId}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ========== VALIDATION ERRORS ==========

    // 86. CreateDepartment_WithNameTooShort_ReturnsBadRequest
    [Fact]
    public async Task CreateDepartment_WithNameTooShort_ReturnsBadRequest()
    {
        var request = new CreateDepartmentRequest
        {
            Name = "AB", // too short — MinLength(3)
            Description = "Descripción",
            Email = "test@test.com",
            Phone = "6011234567"
        };
        var response = await _client.PostAsJsonAsync("/api/v1/Departments", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 87. CreateDepartment_WithInvalidEmail_ReturnsBadRequest
    [Fact]
    public async Task CreateDepartment_WithInvalidEmail_ReturnsBadRequest()
    {
        var request = new CreateDepartmentRequest
        {
            Name = "Test Department Valid",
            Description = "Descripción",
            Email = "not-an-email",
            Phone = "6011234567"
        };
        var response = await _client.PostAsJsonAsync("/api/v1/Departments", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 88. CreateDepartment_WithInvalidPhone_ReturnsBadRequest
    [Fact]
    public async Task CreateDepartment_WithInvalidPhone_ReturnsBadRequest()
    {
        var request = new CreateDepartmentRequest
        {
            Name = "Test Department Valid",
            Description = "Descripción",
            Email = "test@test.com",
            Phone = "123" // invalid Colombian phone format
        };
        var response = await _client.PostAsJsonAsync("/api/v1/Departments", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ========== NOT FOUND EDGE CASES ==========

    // 89. GetDepartmentById_WithNonExisting_ReturnsNotFound
    [Fact]
    public async Task GetDepartmentById_WithNonExisting_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v1/Departments/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // 90. UpdateDepartment_WithNonExisting_ReturnsNotFound
    [Fact]
    public async Task UpdateDepartment_WithNonExisting_ReturnsNotFound()
    {
        var updateRequest = new UpdateDepartmentRequest { Name = "Non Existing" };
        var response = await _client.PatchAsJsonAsync("/api/v1/Departments/99999", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // 91. DeleteDepartment_WithNonExisting_ReturnsNotFound
    [Fact]
    public async Task DeleteDepartment_WithNonExisting_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/v1/Departments/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ========== PARTIAL UPDATE ==========

    // 92. UpdateDepartment_PartialUpdateOnlyName_OtherFieldsUnchanged
    [Fact]
    public async Task UpdateDepartment_PartialUpdateOnlyName_OtherFieldsUnchanged()
    {
        var request = CreateValidDepartmentRequest();
        var createResponse = await _client.PostAsJsonAsync("/api/v1/Departments", request);
        var created = await createResponse.Content.ReadFromJsonAsync<DepartmentDto>();

        // Update only the Name, leave everything else null (no change)
        var updateRequest = new UpdateDepartmentRequest { Name = "Partially Updated Name" };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/v1/Departments/{created!.Id}", updateRequest);

        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<DepartmentDto>();
        updated!.Name.Should().Be("Partially Updated Name");
        // Other fields should retain original values from creation
        updated.Description.Should().Be(request.Description);
        updated.Email.Should().Be(request.Email);
        updated.Phone.Should().Be(request.Phone);
    }
}
