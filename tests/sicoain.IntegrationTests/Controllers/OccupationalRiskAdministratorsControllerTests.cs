using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using sicoain.IntegrationTests.Fixtures;
using sicoain.IntegrationTests.Utilities;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.OccupationalRiskAdministrators;
using sicoain.shared.Entities;

namespace sicoain.IntegrationTests.Controllers;

public class OccupationalRiskAdministratorsControllerTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly CookieHandler _cookieHandler;
    private readonly IntegrationTestWebAppFactory _factory;

    public OccupationalRiskAdministratorsControllerTests(IntegrationTestWebAppFactory factory)
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

    private CreateOccupationalRiskAdministratorRequest CreateValidRequest()
    {
        return new CreateOccupationalRiskAdministratorRequest
        {
            Name = $"TestARL_{Guid.NewGuid():N}",
            AddressStreet = "Avenida Principal 123"
        };
    }

    private async Task<int> CreateTestOccupationalRiskAdministratorAsync()
    {
        var request = CreateValidRequest();
        var response = await _client.PostAsJsonAsync("/api/v1/OccupationalRiskAdministrators", request);
        response.EnsureSuccessStatusCode();

        // DTO doesn't include Id, so query the DB
        await using var db = CreateDbContext();
        var entity = await db.Set<OccupationalRiskAdministrator>()
            .OrderByDescending(x => x.Id)
            .FirstAsync();
        return entity.Id;
    }

    #endregion

    // ========== CREATE ==========

    [Fact]
    public async Task CreateOccupationalRiskAdministrator_WithValidData_ReturnsCreated()
    {
        var request = CreateValidRequest();

        var response = await _client.PostAsJsonAsync("/api/v1/OccupationalRiskAdministrators", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var entity = await response.Content.ReadFromJsonAsync<OccupationalRiskAdministratorDto>();
        entity.Should().NotBeNull();
        entity!.Name.Should().Be(request.Name);
        entity.AddressStreet.Should().Be(request.AddressStreet);
    }

    [Fact]
    public async Task CreateOccupationalRiskAdministrator_WithNameTooShort_ReturnsBadRequest()
    {
        var request = CreateValidRequest();
        request = request with { Name = "AB" };

        var response = await _client.PostAsJsonAsync("/api/v1/OccupationalRiskAdministrators", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOccupationalRiskAdministrator_WithNameTooLong_ReturnsBadRequest()
    {
        var request = CreateValidRequest();
        request = request with { Name = new string('X', 101) };

        var response = await _client.PostAsJsonAsync("/api/v1/OccupationalRiskAdministrators", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOccupationalRiskAdministrator_WithEmptyName_ReturnsBadRequest()
    {
        var request = CreateValidRequest();
        request = request with { Name = string.Empty };

        var response = await _client.PostAsJsonAsync("/api/v1/OccupationalRiskAdministrators", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOccupationalRiskAdministrator_WithAddressStreetTooLong_ReturnsBadRequest()
    {
        var request = CreateValidRequest();
        request = request with { AddressStreet = new string('A', 201) };

        var response = await _client.PostAsJsonAsync("/api/v1/OccupationalRiskAdministrators", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ========== GET BY ID ==========

    [Fact]
    public async Task GetOccupationalRiskAdministratorById_WithExisting_ReturnsDto()
    {
        var entityId = await CreateTestOccupationalRiskAdministratorAsync();

        var response = await _client.GetAsync($"/api/v1/OccupationalRiskAdministrators/{entityId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var entity = await response.Content.ReadFromJsonAsync<OccupationalRiskAdministratorDto>();
        entity.Should().NotBeNull();
        entity!.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetOccupationalRiskAdministratorById_WithNonExisting_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v1/OccupationalRiskAdministrators/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ========== GET ALL ==========

    [Fact]
    public async Task GetAllOccupationalRiskAdministrators_ReturnsPaginatedList()
    {
        await CreateTestOccupationalRiskAdministratorAsync();

        var response = await _client.GetAsync("/api/v1/OccupationalRiskAdministrators?pageNumber=1&pageSize=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<OccupationalRiskAdministratorDto>>();
        pagedResponse.Should().NotBeNull();
        pagedResponse!.Items.Should().NotBeEmpty();
        pagedResponse.PageNumber.Should().Be(1);
        pagedResponse.PageSize.Should().Be(5);
        pagedResponse.TotalCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetAllOccupationalRiskAdministrators_WithPageBeyondData_ReturnsEmpty()
    {
        var response = await _client.GetAsync("/api/v1/OccupationalRiskAdministrators?pageNumber=9999&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<OccupationalRiskAdministratorDto>>();
        pagedResponse.Should().NotBeNull();
        pagedResponse!.Items.Should().BeEmpty();
        pagedResponse.PageNumber.Should().Be(9999);
        pagedResponse.TotalCount.Should().BeGreaterThanOrEqualTo(0);
    }

    // ========== UPDATE ==========

    [Fact]
    public async Task UpdateOccupationalRiskAdministrator_WithValidData_Updates()
    {
        var entityId = await CreateTestOccupationalRiskAdministratorAsync();

        var updateRequest = new UpdateOccupationalRiskAdministratorRequest
        {
            Name = "Updated ARL Name",
            AddressStreet = "Nueva Dirección 789"
        };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/v1/OccupationalRiskAdministrators/{entityId}", updateRequest);

        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<OccupationalRiskAdministratorDto>();
        updated!.Name.Should().Be("Updated ARL Name");
        updated.AddressStreet.Should().Be("Nueva Dirección 789");
    }

    [Fact]
    public async Task UpdateOccupationalRiskAdministrator_WithNonExisting_ReturnsNotFound()
    {
        var updateRequest = new UpdateOccupationalRiskAdministratorRequest
        {
            Name = "Non-existent"
        };

        var response = await _client.PatchAsJsonAsync("/api/v1/OccupationalRiskAdministrators/999999", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateOccupationalRiskAdministrator_WithPartialNameOnly_UpdatesOnlyName()
    {
        var entityId = await CreateTestOccupationalRiskAdministratorAsync();
        var originalAddress = "Avenida Principal 123";

        var updateRequest = new UpdateOccupationalRiskAdministratorRequest
        {
            Name = "Only Name Changed"
        };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/v1/OccupationalRiskAdministrators/{entityId}", updateRequest);

        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<OccupationalRiskAdministratorDto>();
        updated!.Name.Should().Be("Only Name Changed");
        updated.AddressStreet.Should().Be(originalAddress);
    }

    [Fact]
    public async Task UpdateOccupationalRiskAdministrator_WithPartialAddressOnly_UpdatesOnlyAddress()
    {
        var entityId = await CreateTestOccupationalRiskAdministratorAsync();
        var originalName = (await _client.GetFromJsonAsync<OccupationalRiskAdministratorDto>($"/api/v1/OccupationalRiskAdministrators/{entityId}"))!.Name;

        var updateRequest = new UpdateOccupationalRiskAdministratorRequest
        {
            AddressStreet = "Solo Dirección Cambió"
        };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/v1/OccupationalRiskAdministrators/{entityId}", updateRequest);

        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<OccupationalRiskAdministratorDto>();
        updated!.AddressStreet.Should().Be("Solo Dirección Cambió");
        updated.Name.Should().Be(originalName);
    }

    [Fact]
    public async Task UpdateOccupationalRiskAdministrator_WithNameTooShort_ReturnsBadRequest()
    {
        var entityId = await CreateTestOccupationalRiskAdministratorAsync();

        var updateRequest = new UpdateOccupationalRiskAdministratorRequest
        {
            Name = "AB"
        };
        var response = await _client.PatchAsJsonAsync($"/api/v1/OccupationalRiskAdministrators/{entityId}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateOccupationalRiskAdministrator_WithNameTooLong_ReturnsBadRequest()
    {
        var entityId = await CreateTestOccupationalRiskAdministratorAsync();

        var updateRequest = new UpdateOccupationalRiskAdministratorRequest
        {
            Name = new string('X', 101)
        };
        var response = await _client.PatchAsJsonAsync($"/api/v1/OccupationalRiskAdministrators/{entityId}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateOccupationalRiskAdministrator_WithAddressStreetTooLong_ReturnsBadRequest()
    {
        var entityId = await CreateTestOccupationalRiskAdministratorAsync();

        var updateRequest = new UpdateOccupationalRiskAdministratorRequest
        {
            AddressStreet = new string('A', 201)
        };
        var response = await _client.PatchAsJsonAsync($"/api/v1/OccupationalRiskAdministrators/{entityId}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ========== DELETE ==========

    [Fact]
    public async Task DeleteOccupationalRiskAdministrator_SoftDeletes()
    {
        var entityId = await CreateTestOccupationalRiskAdministratorAsync();

        var deleteResponse = await _client.DeleteAsync($"/api/v1/OccupationalRiskAdministrators/{entityId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/v1/OccupationalRiskAdministrators/{entityId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteOccupationalRiskAdministrator_WithNonExisting_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/v1/OccupationalRiskAdministrators/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteOccupationalRiskAdministrator_WithAlreadyDeleted_ReturnsNoContent()
    {
        var entityId = await CreateTestOccupationalRiskAdministratorAsync();

        var firstDelete = await _client.DeleteAsync($"/api/v1/OccupationalRiskAdministrators/{entityId}");
        firstDelete.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var secondDelete = await _client.DeleteAsync($"/api/v1/OccupationalRiskAdministrators/{entityId}");
        secondDelete.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    // ========== AUTHORIZATION ==========

    [Fact]
    public async Task Unauthorized_CreateOccupationalRiskAdministrator_WithoutSettingsEdit()
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

        var request = CreateValidRequest();
        var response = await unauthClient.PostAsJsonAsync("/api/v1/OccupationalRiskAdministrators", request);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Unauthorized_ViewOccupationalRiskAdministrator_WithoutSettingsView()
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

        var entityId = await CreateTestOccupationalRiskAdministratorAsync();
        var response = await unauthClient.GetAsync($"/api/v1/OccupationalRiskAdministrators/{entityId}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Unauthorized_UpdateOccupationalRiskAdministrator_WithoutSettingsEdit()
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

        var entityId = await CreateTestOccupationalRiskAdministratorAsync();
        var updateRequest = new UpdateOccupationalRiskAdministratorRequest { Name = "Hacked Name" };
        var response = await unauthClient.PatchAsJsonAsync($"/api/v1/OccupationalRiskAdministrators/{entityId}", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Unauthorized_DeleteOccupationalRiskAdministrator_WithoutSettingsEdit()
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

        var entityId = await CreateTestOccupationalRiskAdministratorAsync();
        var response = await unauthClient.DeleteAsync($"/api/v1/OccupationalRiskAdministrators/{entityId}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ========== SUB-RESOURCES: EMAILS ==========
    // NOTE: Email and phone endpoints may not be implemented yet.
    // Tests are skipped but kept for reference.

    [Fact(Skip = "OccupationalRiskAdministrator email endpoints not yet implemented")]
    public async Task CreateOccupationalRiskAdministratorEmail_WithValidData_ReturnsCreated()
    {
        var entityId = await CreateTestOccupationalRiskAdministratorAsync();
        var request = new CreateOccupationalRiskAdministratorEmailRequest
        {
            OccupationalRiskAdministratorId = entityId,
            Email = $"test{Guid.NewGuid():N}@example.com"
        };
        var response = await _client.PostAsJsonAsync("/api/v1/OccupationalRiskAdministrators/emails", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var emailDto = await response.Content.ReadFromJsonAsync<OccupationalRiskAdministratorEmailDto>();
        emailDto.Should().NotBeNull();
        emailDto!.Email.Should().Be(request.Email);
        emailDto.OccupationalRiskAdministratorId.Should().Be(entityId);
    }

    [Fact(Skip = "OccupationalRiskAdministrator email endpoints not yet implemented")]
    public async Task GetOccupationalRiskAdministratorEmails_ReturnsList()
    {
        var entityId = await CreateTestOccupationalRiskAdministratorAsync();
        var emailRequest = new CreateOccupationalRiskAdministratorEmailRequest { OccupationalRiskAdministratorId = entityId, Email = "list@test.com" };
        await _client.PostAsJsonAsync("/api/v1/OccupationalRiskAdministrators/emails", emailRequest);

        var response = await _client.GetAsync($"/api/v1/OccupationalRiskAdministrators/{entityId}/emails");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var emails = await response.Content.ReadFromJsonAsync<List<OccupationalRiskAdministratorEmailDto>>();
        emails.Should().Contain(e => e.Email == "list@test.com");
    }

    [Fact(Skip = "OccupationalRiskAdministrator email endpoints not yet implemented")]
    public async Task UpdateOccupationalRiskAdministratorEmail_WithValidData_Updates()
    {
        var entityId = await CreateTestOccupationalRiskAdministratorAsync();
        var emailRequest = new CreateOccupationalRiskAdministratorEmailRequest { OccupationalRiskAdministratorId = entityId, Email = "old@test.com" };
        var emailCreateResponse = await _client.PostAsJsonAsync("/api/v1/OccupationalRiskAdministrators/emails", emailRequest);
        var emailDto = await emailCreateResponse.Content.ReadFromJsonAsync<OccupationalRiskAdministratorEmailDto>();

        var updateRequest = new UpdateOccupationalRiskAdministratorEmailRequest { Email = "new@test.com" };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/v1/OccupationalRiskAdministrators/emails/{emailDto!.Id}", updateRequest);
        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<OccupationalRiskAdministratorEmailDto>();
        updated!.Email.Should().Be("new@test.com");
    }

    [Fact(Skip = "OccupationalRiskAdministrator email endpoints not yet implemented")]
    public async Task DeleteOccupationalRiskAdministratorEmail_Removes()
    {
        var entityId = await CreateTestOccupationalRiskAdministratorAsync();
        var emailRequest = new CreateOccupationalRiskAdministratorEmailRequest { OccupationalRiskAdministratorId = entityId, Email = "delete@test.com" };
        var emailCreateResponse = await _client.PostAsJsonAsync("/api/v1/OccupationalRiskAdministrators/emails", emailRequest);
        var emailDto = await emailCreateResponse.Content.ReadFromJsonAsync<OccupationalRiskAdministratorEmailDto>();

        var deleteResponse = await _client.DeleteAsync($"/api/v1/OccupationalRiskAdministrators/emails/{emailDto!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/v1/OccupationalRiskAdministrators/{entityId}/emails");
        var emails = await getResponse.Content.ReadFromJsonAsync<List<OccupationalRiskAdministratorEmailDto>>();
        emails.Should().NotContain(e => e.Id == emailDto.Id);
    }

    // ========== SUB-RESOURCES: PHONES ==========

    [Fact(Skip = "OccupationalRiskAdministrator phone endpoints not yet implemented")]
    public async Task CreateOccupationalRiskAdministratorPhone_WithValidData_ReturnsCreated()
    {
        var entityId = await CreateTestOccupationalRiskAdministratorAsync();
        var request = new CreateOccupationalRiskAdministratorPhoneRequest
        {
            OccupationalRiskAdministratorId = entityId,
            Phone = "3001234567"
        };
        var response = await _client.PostAsJsonAsync("/api/v1/OccupationalRiskAdministrators/phones", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var phoneDto = await response.Content.ReadFromJsonAsync<OccupationalRiskAdministratorPhoneDto>();
        phoneDto.Should().NotBeNull();
        phoneDto!.PhoneNumber.Should().Be(request.Phone);
        phoneDto.OccupationalRiskAdministratorId.Should().Be(entityId);
    }

    [Fact(Skip = "OccupationalRiskAdministrator phone endpoints not yet implemented")]
    public async Task GetOccupationalRiskAdministratorPhones_ReturnsList()
    {
        var entityId = await CreateTestOccupationalRiskAdministratorAsync();
        var phoneRequest = new CreateOccupationalRiskAdministratorPhoneRequest { OccupationalRiskAdministratorId = entityId, Phone = "3112223333" };
        await _client.PostAsJsonAsync("/api/v1/OccupationalRiskAdministrators/phones", phoneRequest);

        var response = await _client.GetAsync($"/api/v1/OccupationalRiskAdministrators/{entityId}/phones");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var phones = await response.Content.ReadFromJsonAsync<List<OccupationalRiskAdministratorPhoneDto>>();
        phones.Should().Contain(p => p.PhoneNumber == "3112223333");
    }

    [Fact(Skip = "OccupationalRiskAdministrator phone endpoints not yet implemented")]
    public async Task UpdateOccupationalRiskAdministratorPhone_WithValidData_Updates()
    {
        var entityId = await CreateTestOccupationalRiskAdministratorAsync();
        var phoneRequest = new CreateOccupationalRiskAdministratorPhoneRequest { OccupationalRiskAdministratorId = entityId, Phone = "3001112222" };
        var phoneCreateResponse = await _client.PostAsJsonAsync("/api/v1/OccupationalRiskAdministrators/phones", phoneRequest);
        var phoneDto = await phoneCreateResponse.Content.ReadFromJsonAsync<OccupationalRiskAdministratorPhoneDto>();

        var updateRequest = new UpdateOccupationalRiskAdministratorPhoneRequest { Phone = "3112223333" };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/v1/OccupationalRiskAdministrators/phones/{phoneDto!.Id}", updateRequest);
        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<OccupationalRiskAdministratorPhoneDto>();
        updated!.PhoneNumber.Should().Be("3112223333");
    }

    [Fact(Skip = "OccupationalRiskAdministrator phone endpoints not yet implemented")]
    public async Task DeleteOccupationalRiskAdministratorPhone_Removes()
    {
        var entityId = await CreateTestOccupationalRiskAdministratorAsync();
        var phoneRequest = new CreateOccupationalRiskAdministratorPhoneRequest { OccupationalRiskAdministratorId = entityId, Phone = "3004445555" };
        var phoneCreateResponse = await _client.PostAsJsonAsync("/api/v1/OccupationalRiskAdministrators/phones", phoneRequest);
        var phoneDto = await phoneCreateResponse.Content.ReadFromJsonAsync<OccupationalRiskAdministratorPhoneDto>();

        var deleteResponse = await _client.DeleteAsync($"/api/v1/OccupationalRiskAdministrators/phones/{phoneDto!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/v1/OccupationalRiskAdministrators/{entityId}/phones");
        var phones = await getResponse.Content.ReadFromJsonAsync<List<OccupationalRiskAdministratorPhoneDto>>();
        phones.Should().NotContain(p => p.Id == phoneDto.Id);
    }
}
