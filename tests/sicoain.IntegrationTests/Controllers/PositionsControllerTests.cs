using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using sicoain.IntegrationTests.Fixtures;
using sicoain.IntegrationTests.Utilities;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Departments;
using sicoain.shared.DTOs.Positions;
using sicoain.shared.Entities;

namespace sicoain.IntegrationTests.Controllers;

public class PositionsControllerTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly CookieHandler _cookieHandler;
    private readonly IntegrationTestWebAppFactory _factory;

    public PositionsControllerTests(IntegrationTestWebAppFactory factory)
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

    private async Task<(int departmentId, int riskClassId)> CreateRequiredEntitiesAsync()
    {
        await using var context = CreateDbContext();

        // Create Department
        var department = new Department { Name = $"TestDept_{Guid.NewGuid():N}" };
        context.Departments.Add(department);
        await context.SaveChangesAsync();

        // Get or create RiskClass (from seeder, fallback)
        var riskClass = await context.RiskClasses.FirstOrDefaultAsync();
        if (riskClass == null)
        {
            riskClass = new RiskClass { Name = "Clase I", Code = "I", ContributionRate = 0.005m, IsActive = true };
            context.RiskClasses.Add(riskClass);
            await context.SaveChangesAsync();
        }

        return (department.Id, riskClass.Id);
    }

    private CreatePositionRequest CreateValidPositionRequest(int departmentId, int riskClassId)
    {
        return new CreatePositionRequest
        {
            Name = $"TestPosition_{Guid.NewGuid():N}",
            Description = "Descripción del puesto",
            DepartmentId = departmentId,
            RiskClassId = riskClassId
        };
    }

    private async Task<int> CreateTestPositionAsync()
    {
        var (deptId, riskId) = await CreateRequiredEntitiesAsync();
        var request = CreateValidPositionRequest(deptId, riskId);
        var response = await _client.PostAsJsonAsync("/api/v1/Positions", request);
        response.EnsureSuccessStatusCode();
        var position = await response.Content.ReadFromJsonAsync<PositionDto>();
        return position!.Id;
    }

    #endregion

    // 86. CreatePosition_WithValidData_ReturnsCreated
    [Fact]
    public async Task CreatePosition_WithValidData_ReturnsCreated()
    {
        var (deptId, riskId) = await CreateRequiredEntitiesAsync();
        var request = CreateValidPositionRequest(deptId, riskId);

        var response = await _client.PostAsJsonAsync("/api/v1/Positions", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var position = await response.Content.ReadFromJsonAsync<PositionDto>();
        position.Should().NotBeNull();
        position!.Name.Should().Be(request.Name);
        position.Description.Should().Be(request.Description);
        position.DepartmentId.Should().Be(deptId);
        position.RiskClassId.Should().Be(riskId);
    }

    // 87. CreatePosition_WithInvalidDepartmentId_ThrowsDbUpdateException
    [Fact]
    public async Task CreatePosition_WithInvalidDepartmentId_ThrowsDbUpdateException()
    {
        var (_, riskId) = await CreateRequiredEntitiesAsync();
        var request = new CreatePositionRequest
        {
            Name = "Invalid Position",
            Description = "FK violada",
            DepartmentId = 99999,
            RiskClassId = riskId
        };
        Func<Task> act = () => _client.PostAsJsonAsync("/api/v1/Positions", request);
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    // 88. CreatePosition_WithInvalidRiskClassId_ThrowsDbUpdateException
    [Fact]
    public async Task CreatePosition_WithInvalidRiskClassId_ThrowsDbUpdateException()
    {
        var (deptId, _) = await CreateRequiredEntitiesAsync();
        var request = new CreatePositionRequest
        {
            Name = "Invalid Position",
            Description = "FK violada",
            DepartmentId = deptId,
            RiskClassId = 99999
        };
        Func<Task> act = () => _client.PostAsJsonAsync("/api/v1/Positions", request);
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    // 89. GetPositionById_WithExisting_ReturnsDto
    [Fact]
    public async Task GetPositionById_WithExisting_ReturnsDto()
    {
        var positionId = await CreateTestPositionAsync();

        var response = await _client.GetAsync($"/api/v1/Positions/{positionId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var position = await response.Content.ReadFromJsonAsync<PositionDto>();
        position.Should().NotBeNull();
        position!.Id.Should().Be(positionId);
        position.Name.Should().NotBeNullOrEmpty();
        position.DepartmentId.Should().BeGreaterThan(0);
        position.RiskClassId.Should().BeGreaterThan(0);
    }

    // 90. GetAllPositions_ReturnsPaginatedList
    [Fact]
    public async Task GetAllPositions_ReturnsPaginatedList()
    {
        await CreateTestPositionAsync();

        var response = await _client.GetAsync("/api/v1/Positions?pageNumber=1&pageSize=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<PositionDto>>();
        pagedResponse.Should().NotBeNull();
        pagedResponse!.Items.Should().NotBeEmpty();
        pagedResponse.PageNumber.Should().Be(1);
        pagedResponse.PageSize.Should().Be(5);
        pagedResponse.TotalCount.Should().BeGreaterThan(0);
    }

    // 91. UpdatePosition_WithValidData_Updates
    [Fact]
    public async Task UpdatePosition_WithValidData_Updates()
    {
        var positionId = await CreateTestPositionAsync();
        var (newDeptId, newRiskId) = await CreateRequiredEntitiesAsync();

        var updateRequest = new UpdatePositionRequest
        {
            Name = "Updated Position Name",
            Description = "Descripción actualizada",
            DepartmentId = newDeptId,
            RiskClassId = newRiskId
        };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/v1/Positions/{positionId}", updateRequest);

        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<PositionDto>();
        updated!.Name.Should().Be("Updated Position Name");
        updated.Description.Should().Be("Descripción actualizada");
        updated.DepartmentId.Should().Be(newDeptId);
        updated.RiskClassId.Should().Be(newRiskId);
    }

    // 92. DeletePosition_SoftDeletes
    [Fact]
    public async Task DeletePosition_SoftDeletes()
    {
        var positionId = await CreateTestPositionAsync();

        var deleteResponse = await _client.DeleteAsync($"/api/v1/Positions/{positionId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/v1/Positions/{positionId}");
        // Patrón consistente: GetById devuelve OK incluso después de soft delete
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // 93. Unauthorized_CreatePosition_WithoutSettingsEdit
    [Fact]
    public async Task Unauthorized_CreatePosition_WithoutPermission()
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

        var (deptId, riskId) = await CreateRequiredEntitiesAsync();
        var request = CreateValidPositionRequest(deptId, riskId);
        var response = await unauthClient.PostAsJsonAsync("/api/v1/Positions", request);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // 94. Unauthorized_ViewPosition_WithoutSettingsView
    [Fact]
    public async Task Unauthorized_ViewPosition_WithoutPermission()
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

        var positionId = await CreateTestPositionAsync();
        var response = await unauthClient.GetAsync($"/api/v1/Positions/{positionId}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ========== VALIDATION ERRORS ==========

    // 95. CreatePosition_WithNameTooShort_ReturnsBadRequest
    [Fact]
    public async Task CreatePosition_WithNameTooShort_ReturnsBadRequest()
    {
        var (deptId, riskId) = await CreateRequiredEntitiesAsync();
        var request = new CreatePositionRequest
        {
            Name = "AB", // FluentValidation Length(3, 100)
            DepartmentId = deptId,
            RiskClassId = riskId
        };
        var response = await _client.PostAsJsonAsync("/api/v1/Positions", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 96. CreatePosition_WithDescriptionTooLong_ReturnsBadRequest
    [Fact]
    public async Task CreatePosition_WithDescriptionTooLong_ReturnsBadRequest()
    {
        var (deptId, riskId) = await CreateRequiredEntitiesAsync();
        var request = new CreatePositionRequest
        {
            Name = "Valid Position Name",
            Description = new string('A', 501), // FluentValidation MaximumLength(500)
            DepartmentId = deptId,
            RiskClassId = riskId
        };
        var response = await _client.PostAsJsonAsync("/api/v1/Positions", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 97. CreatePosition_WithDepartmentIdZero_ReturnsBadRequest
    [Fact]
    public async Task CreatePosition_WithDepartmentIdZero_ReturnsBadRequest()
    {
        var (_, riskId) = await CreateRequiredEntitiesAsync();
        var request = new CreatePositionRequest
        {
            Name = "Valid Position Name",
            DepartmentId = 0, // Range(1, int.MaxValue) fails
            RiskClassId = riskId
        };
        var response = await _client.PostAsJsonAsync("/api/v1/Positions", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 98. CreatePosition_WithRiskClassIdZero_ReturnsBadRequest
    [Fact]
    public async Task CreatePosition_WithRiskClassIdZero_ReturnsBadRequest()
    {
        var (deptId, _) = await CreateRequiredEntitiesAsync();
        var request = new CreatePositionRequest
        {
            Name = "Valid Position Name",
            DepartmentId = deptId,
            RiskClassId = 0 // Range(1, int.MaxValue) fails
        };
        var response = await _client.PostAsJsonAsync("/api/v1/Positions", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 99. UpdatePosition_WithNameTooShort_ReturnsBadRequest
    [Fact]
    public async Task UpdatePosition_WithNameTooShort_ReturnsBadRequest()
    {
        var positionId = await CreateTestPositionAsync();
        var updateRequest = new UpdatePositionRequest { Name = "AB" }; // FluentValidation Length(3, 100)
        var response = await _client.PatchAsJsonAsync($"/api/v1/Positions/{positionId}", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 100. UpdatePosition_WithDepartmentIdZero_ReturnsBadRequest
    [Fact]
    public async Task UpdatePosition_WithDepartmentIdZero_ReturnsBadRequest()
    {
        var positionId = await CreateTestPositionAsync();
        var updateRequest = new UpdatePositionRequest { DepartmentId = 0 }; // GreaterThan(0) fails
        var response = await _client.PatchAsJsonAsync($"/api/v1/Positions/{positionId}", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ========== NOT FOUND EDGE CASES ==========

    // 101. GetPositionById_WithNonExisting_ReturnsNotFound
    [Fact]
    public async Task GetPositionById_WithNonExisting_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v1/Positions/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // 102. UpdatePosition_WithNonExisting_ReturnsNotFound
    [Fact]
    public async Task UpdatePosition_WithNonExisting_ReturnsNotFound()
    {
        var (deptId, riskId) = await CreateRequiredEntitiesAsync();
        var updateRequest = new UpdatePositionRequest
        {
            Name = "Non Existing",
            DepartmentId = deptId,
            RiskClassId = riskId
        };
        var response = await _client.PatchAsJsonAsync("/api/v1/Positions/99999", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // 103. DeletePosition_WithNonExisting_ReturnsNotFound
    [Fact]
    public async Task DeletePosition_WithNonExisting_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/v1/Positions/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ========== PARTIAL UPDATES ==========

    // 104. UpdatePosition_PartialUpdateOnlyName_OtherFieldsUnchanged
    [Fact]
    public async Task UpdatePosition_PartialUpdateOnlyName_OtherFieldsUnchanged()
    {
        var (deptId, riskId) = await CreateRequiredEntitiesAsync();
        var createRequest = CreateValidPositionRequest(deptId, riskId);
        var createResponse = await _client.PostAsJsonAsync("/api/v1/Positions", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<PositionDto>();

        var updateRequest = new UpdatePositionRequest { Name = "Partially Updated Name" };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/v1/Positions/{created!.Id}", updateRequest);

        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<PositionDto>();
        updated!.Name.Should().Be("Partially Updated Name");
        updated.Description.Should().Be(createRequest.Description);
        updated.DepartmentId.Should().Be(deptId);
        updated.RiskClassId.Should().Be(riskId);
    }

    // 105. UpdatePosition_PartialUpdateOnlyDepartmentId
    [Fact]
    public async Task UpdatePosition_PartialUpdateOnlyDepartmentId()
    {
        var (deptId, riskId) = await CreateRequiredEntitiesAsync();
        var createRequest = CreateValidPositionRequest(deptId, riskId);
        var createResponse = await _client.PostAsJsonAsync("/api/v1/Positions", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<PositionDto>();

        // Create another department to switch to
        var (newDeptId, _) = await CreateRequiredEntitiesAsync();

        var updateRequest = new UpdatePositionRequest { DepartmentId = newDeptId };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/v1/Positions/{created!.Id}", updateRequest);

        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<PositionDto>();
        updated!.DepartmentId.Should().Be(newDeptId);
        updated.Name.Should().Be(createRequest.Name);
        updated.RiskClassId.Should().Be(riskId);
    }

    // ========== PAGINATION EDGE CASE ==========

    // 106. GetAllPositions_WithPageBeyondData_ReturnsEmptyItems
    [Fact]
    public async Task GetAllPositions_WithPageBeyondData_ReturnsEmptyItems()
    {
        var response = await _client.GetAsync("/api/v1/Positions?pageNumber=9999&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<PositionDto>>();
        pagedResponse.Should().NotBeNull();
        pagedResponse!.Items.Should().BeEmpty();
        pagedResponse.PageNumber.Should().Be(9999);
        pagedResponse.PageSize.Should().Be(10);
    }

    // ========== MISSING AUTHORIZATION TESTS ==========

    // 107. Unauthorized_UpdatePosition_WithoutPermission
    [Fact]
    public async Task Unauthorized_UpdatePosition_WithoutPermission()
    {
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var email = $"noupdate{Guid.NewGuid():N}@test.com";
        var user = new User
        {
            UserName = email,
            Email = email,
            FullName = "No Update User",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        await userManager.CreateAsync(user, "NoUpdate123!");

        var unauthHandler = new CookieHandler();
        var unauthClient = _factory.CreateDefaultClient(unauthHandler);
        await unauthClient.PostAsJsonAsync("/api/v1/Auth/login", new { Email = email, Password = "NoUpdate123!" });

        var positionId = await CreateTestPositionAsync();
        var updateRequest = new UpdatePositionRequest { Name = "Hacked Name" };
        var response = await unauthClient.PatchAsJsonAsync($"/api/v1/Positions/{positionId}", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // 108. Unauthorized_DeletePosition_WithoutPermission
    [Fact]
    public async Task Unauthorized_DeletePosition_WithoutPermission()
    {
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var email = $"nodelete{Guid.NewGuid():N}@test.com";
        var user = new User
        {
            UserName = email,
            Email = email,
            FullName = "No Delete User",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        await userManager.CreateAsync(user, "NoDelete123!");

        var unauthHandler = new CookieHandler();
        var unauthClient = _factory.CreateDefaultClient(unauthHandler);
        await unauthClient.PostAsJsonAsync("/api/v1/Auth/login", new { Email = email, Password = "NoDelete123!" });

        var positionId = await CreateTestPositionAsync();
        var response = await unauthClient.DeleteAsync($"/api/v1/Positions/{positionId}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
