using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using sicoain.IntegrationTests.Fixtures;
using sicoain.IntegrationTests.Utilities;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.RiskClasses;
using sicoain.shared.Entities;

namespace sicoain.IntegrationTests.Controllers;

public class RiskClassesControllerTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly CookieHandler _cookieHandler;
    private readonly IntegrationTestWebAppFactory _factory;

    public RiskClassesControllerTests(IntegrationTestWebAppFactory factory)
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

    private CreateRiskClassRequest CreateValidRiskClassRequest()
    {
        return new CreateRiskClassRequest
        {
            Name = $"TestRiskClass_{Guid.NewGuid():N}",
            Code = $"RC{Guid.NewGuid():N}"[..5],
            ContributionRate = 0.025m,
            IsActive = true
        };
    }

    private async Task<int> CreateTestRiskClassAsync()
    {
        var request = CreateValidRiskClassRequest();
        var response = await _client.PostAsJsonAsync("/api/v1/RiskClasses", request);
        response.EnsureSuccessStatusCode();
        var riskClass = await response.Content.ReadFromJsonAsync<RiskClassDto>();
        return riskClass!.Id;
    }

    #endregion

    // 94. CreateRiskClass_WithValidData_ReturnsCreated
    [Fact]
    public async Task CreateRiskClass_WithValidData_ReturnsCreated()
    {
        var request = CreateValidRiskClassRequest();

        var response = await _client.PostAsJsonAsync("/api/v1/RiskClasses", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var riskClass = await response.Content.ReadFromJsonAsync<RiskClassDto>();
        riskClass.Should().NotBeNull();
        riskClass!.Name.Should().Be(request.Name);
        riskClass.Code.Should().Be(request.Code);
        riskClass.ContributionRate.Should().Be(request.ContributionRate);
        riskClass.IsActive.Should().Be(request.IsActive);
    }

    // 95. CreateRiskClass_WithDuplicateCode_No unique constraint -> both succeed
    [Fact]
    public async Task CreateRiskClass_WithDuplicateCode_ReturnsCreated()
    {
        var request = CreateValidRiskClassRequest();
        var firstResponse = await _client.PostAsJsonAsync("/api/v1/RiskClasses", request);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var first = await firstResponse.Content.ReadFromJsonAsync<RiskClassDto>();

        var duplicateRequest = request with { Name = "Another Name" };
        var response = await _client.PostAsJsonAsync("/api/v1/RiskClasses", duplicateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    // 96. CreateRiskClass_WithInvalidContributionRate_ReturnsBadRequest
    [Fact]
    public async Task CreateRiskClass_WithInvalidContributionRate_ReturnsBadRequest()
    {
        var request = CreateValidRiskClassRequest() with { ContributionRate = 0.00001m }; // too low (<0.0001)
        var response = await _client.PostAsJsonAsync("/api/v1/RiskClasses", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 97. GetRiskClassById_WithExisting_ReturnsDto
    [Fact]
    public async Task GetRiskClassById_WithExisting_ReturnsDto()
    {
        var riskClassId = await CreateTestRiskClassAsync();

        var response = await _client.GetAsync($"/api/v1/RiskClasses/{riskClassId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var riskClass = await response.Content.ReadFromJsonAsync<RiskClassDto>();
        riskClass.Should().NotBeNull();
        riskClass!.Id.Should().Be(riskClassId);
        riskClass.Name.Should().NotBeNullOrEmpty();
        riskClass.Code.Should().NotBeNullOrEmpty();
    }

    // 98. GetAllRiskClasses_ReturnsPaginatedList
    [Fact]
    public async Task GetAllRiskClasses_ReturnsPaginatedList()
    {
        await CreateTestRiskClassAsync();

        var response = await _client.GetAsync("/api/v1/RiskClasses?pageNumber=1&pageSize=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<RiskClassDto>>();
        pagedResponse.Should().NotBeNull();
        pagedResponse!.Items.Should().NotBeEmpty();
        pagedResponse.PageNumber.Should().Be(1);
        pagedResponse.PageSize.Should().Be(5);
        pagedResponse.TotalCount.Should().BeGreaterThan(0);
    }

    // 99. UpdateRiskClass_WithValidData_Updates
    [Fact]
    public async Task UpdateRiskClass_WithValidData_Updates()
    {
        var riskClassId = await CreateTestRiskClassAsync();

        var updateRequest = new UpdateRiskClassRequest
        {
            Name = "Updated Risk Class Name",
            Code = "UPD",
            ContributionRate = 0.045m,
            IsActive = false
        };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/v1/RiskClasses/{riskClassId}", updateRequest);

        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<RiskClassDto>();
        updated!.Name.Should().Be("Updated Risk Class Name");
        updated.Code.Should().Be("UPD");
        updated.ContributionRate.Should().Be(0.045m);
        updated.IsActive.Should().BeFalse();
    }

    // 100. DeleteRiskClass_SoftDeletes
    [Fact]
    public async Task DeleteRiskClass_SoftDeletes()
    {
        var riskClassId = await CreateTestRiskClassAsync();

        var deleteResponse = await _client.DeleteAsync($"/api/v1/RiskClasses/{riskClassId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/v1/RiskClasses/{riskClassId}");
        // Patrón consistente: GetById devuelve OK incluso después de soft delete
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // 101. Unauthorized_CreateRiskClass_WithoutSettingsEdit
    [Fact]
    public async Task Unauthorized_CreateRiskClass_WithoutPermission()
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

        var request = CreateValidRiskClassRequest();
        var response = await unauthClient.PostAsJsonAsync("/api/v1/RiskClasses", request);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // 102. Unauthorized_ViewRiskClass_WithoutSettingsView
    [Fact]
    public async Task Unauthorized_ViewRiskClass_WithoutPermission()
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

        var riskClassId = await CreateTestRiskClassAsync();
        var response = await unauthClient.GetAsync($"/api/v1/RiskClasses/{riskClassId}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ========== VALIDATION ERRORS ==========

    // 103. CreateRiskClass_WithNameTooShort_ReturnsBadRequest
    [Fact]
    public async Task CreateRiskClass_WithNameTooShort_ReturnsBadRequest()
    {
        var request = new CreateRiskClassRequest
        {
            Name = "AB", // MinLength(3) + FluentValidation Length(3, 100)
            Code = "RC01",
            ContributionRate = 0.025m,
            IsActive = true
        };
        var response = await _client.PostAsJsonAsync("/api/v1/RiskClasses", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 104. CreateRiskClass_WithCodeTooLong_ReturnsBadRequest
    [Fact]
    public async Task CreateRiskClass_WithCodeTooLong_ReturnsBadRequest()
    {
        var request = new CreateRiskClassRequest
        {
            Name = "Valid Risk Class",
            Code = "TOOLONG", // MaxLength(5) + FluentValidation Length(1, 5)
            ContributionRate = 0.025m,
            IsActive = true
        };
        var response = await _client.PostAsJsonAsync("/api/v1/RiskClasses", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 105. CreateRiskClass_WithCodeEmpty_ReturnsBadRequest
    [Fact]
    public async Task CreateRiskClass_WithCodeEmpty_ReturnsBadRequest()
    {
        var request = new CreateRiskClassRequest
        {
            Name = "Valid Risk Class",
            Code = string.Empty, // FluentValidation NotEmpty()
            ContributionRate = 0.025m,
            IsActive = true
        };
        var response = await _client.PostAsJsonAsync("/api/v1/RiskClasses", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ========== NOT FOUND EDGE CASES ==========

    // 106. GetRiskClassById_WithNonExisting_ReturnsNotFound
    [Fact]
    public async Task GetRiskClassById_WithNonExisting_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v1/RiskClasses/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // 107. UpdateRiskClass_WithNonExisting_ReturnsNotFound
    [Fact]
    public async Task UpdateRiskClass_WithNonExisting_ReturnsNotFound()
    {
        var updateRequest = new UpdateRiskClassRequest
        {
            Name = "Non Existing",
            Code = "NONEX",
            ContributionRate = 0.025m,
            IsActive = true
        };
        var response = await _client.PatchAsJsonAsync("/api/v1/RiskClasses/99999", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // 108. DeleteRiskClass_WithNonExisting_ReturnsNotFound
    [Fact]
    public async Task DeleteRiskClass_WithNonExisting_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/v1/RiskClasses/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ========== PARTIAL UPDATES ==========

    // 109. UpdateRiskClass_PartialUpdateOnlyName_OtherFieldsUnchanged
    [Fact]
    public async Task UpdateRiskClass_PartialUpdateOnlyName_OtherFieldsUnchanged()
    {
        var createRequest = CreateValidRiskClassRequest();
        var createResponse = await _client.PostAsJsonAsync("/api/v1/RiskClasses", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<RiskClassDto>();

        var updateRequest = new UpdateRiskClassRequest { Name = "Partially Updated Risk Class" };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/v1/RiskClasses/{created!.Id}", updateRequest);

        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<RiskClassDto>();
        updated!.Name.Should().Be("Partially Updated Risk Class");
        updated.Code.Should().Be(createRequest.Code);
        updated.ContributionRate.Should().Be(createRequest.ContributionRate);
        updated.IsActive.Should().Be(createRequest.IsActive);
    }

    // 110. UpdateRiskClass_PartialUpdateOnlyIsActive
    [Fact]
    public async Task UpdateRiskClass_PartialUpdateOnlyIsActive()
    {
        var createRequest = CreateValidRiskClassRequest();
        var createResponse = await _client.PostAsJsonAsync("/api/v1/RiskClasses", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<RiskClassDto>();

        var updateRequest = new UpdateRiskClassRequest { IsActive = false };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/v1/RiskClasses/{created!.Id}", updateRequest);

        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<RiskClassDto>();
        updated!.IsActive.Should().BeFalse();
        updated.Name.Should().Be(createRequest.Name);
        updated.Code.Should().Be(createRequest.Code);
        updated.ContributionRate.Should().Be(createRequest.ContributionRate);
    }

    // ========== PAGINATION EDGE CASE ==========

    // 111. GetAllRiskClasses_WithPageBeyondData_ReturnsEmptyItems
    [Fact]
    public async Task GetAllRiskClasses_WithPageBeyondData_ReturnsEmptyItems()
    {
        var response = await _client.GetAsync("/api/v1/RiskClasses?pageNumber=9999&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<RiskClassDto>>();
        pagedResponse.Should().NotBeNull();
        pagedResponse!.Items.Should().BeEmpty();
        pagedResponse.PageNumber.Should().Be(9999);
        pagedResponse.PageSize.Should().Be(10);
    }

    // ========== MISSING AUTHORIZATION TESTS ==========

    // 112. Unauthorized_UpdateRiskClass_WithoutPermission
    [Fact]
    public async Task Unauthorized_UpdateRiskClass_WithoutPermission()
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

        var riskClassId = await CreateTestRiskClassAsync();
        var updateRequest = new UpdateRiskClassRequest { Name = "Hacked Name" };
        var response = await unauthClient.PatchAsJsonAsync($"/api/v1/RiskClasses/{riskClassId}", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // 113. Unauthorized_DeleteRiskClass_WithoutPermission
    [Fact]
    public async Task Unauthorized_DeleteRiskClass_WithoutPermission()
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

        var riskClassId = await CreateTestRiskClassAsync();
        var response = await unauthClient.DeleteAsync($"/api/v1/RiskClasses/{riskClassId}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
