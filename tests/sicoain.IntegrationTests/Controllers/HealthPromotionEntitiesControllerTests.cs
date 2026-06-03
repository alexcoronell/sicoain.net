using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using sicoain.IntegrationTests.Fixtures;
using sicoain.IntegrationTests.Utilities;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.HealthPromotionEntities;
using sicoain.shared.Entities;
using sicoain.shared.Enums;

namespace sicoain.IntegrationTests.Controllers;

public class HealthPromotionEntitiesControllerTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly CookieHandler _cookieHandler;
    private readonly IntegrationTestWebAppFactory _factory;

    public HealthPromotionEntitiesControllerTests(IntegrationTestWebAppFactory factory)
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

    private CreateHealthPromotionEntityRequest CreateValidRequest()
    {
        return new CreateHealthPromotionEntityRequest
        {
            Name = $"TestEPS_{Guid.NewGuid():N}",
            AddressStreet = "Calle 123",
            Notes = "Nota de prueba"
        };
    }

    private async Task<int> CreateTestHealthPromotionEntityAsync()
    {
        var request = CreateValidRequest();
        var response = await _client.PostAsJsonAsync("/api/v1/HealthPromotionEntities", request);
        response.EnsureSuccessStatusCode();
        var entity = await response.Content.ReadFromJsonAsync<HealthPromotionEntityDto>();
        return entity!.Id;
    }

    #endregion

    // ========== CREATE ==========

    // 142. CreateHealthPromotionEntity_Valid_ReturnsCreated
    [Fact]
    public async Task CreateHealthPromotionEntity_WithValidData_ReturnsCreated()
    {
        var request = CreateValidRequest();

        var response = await _client.PostAsJsonAsync("/api/v1/HealthPromotionEntities", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var entity = await response.Content.ReadFromJsonAsync<HealthPromotionEntityDto>();
        entity.Should().NotBeNull();
        entity!.Name.Should().Be(request.Name);
        entity.AddressStreet.Should().Be(request.AddressStreet);
        entity.Notes.Should().Be(request.Notes);
    }

    // 147. CreateHealthPromotionEntity_WithNameTooShort_ReturnsBadRequest
    [Fact]
    public async Task CreateHealthPromotionEntity_WithNameTooShort_ReturnsBadRequest()
    {
        var request = new CreateHealthPromotionEntityRequest
        {
            Name = "AB", // menos de 3 caracteres
            AddressStreet = "Calle 123",
            Notes = "Nota"
        };
        var response = await _client.PostAsJsonAsync("/api/v1/HealthPromotionEntities", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 148. CreateHealthPromotionEntity_WithNameTooLong_ReturnsBadRequest
    [Fact]
    public async Task CreateHealthPromotionEntity_WithNameTooLong_ReturnsBadRequest()
    {
        var request = new CreateHealthPromotionEntityRequest
        {
            Name = new string('A', 101), // más de 100 caracteres
            AddressStreet = "Calle 123",
            Notes = "Nota"
        };
        var response = await _client.PostAsJsonAsync("/api/v1/HealthPromotionEntities", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 149. CreateHealthPromotionEntity_WithEmptyName_ReturnsBadRequest
    [Fact]
    public async Task CreateHealthPromotionEntity_WithEmptyName_ReturnsBadRequest()
    {
        var request = new CreateHealthPromotionEntityRequest
        {
            Name = string.Empty,
            AddressStreet = "Calle 123",
            Notes = "Nota"
        };
        var response = await _client.PostAsJsonAsync("/api/v1/HealthPromotionEntities", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 150. CreateHealthPromotionEntity_WithAddressStreetTooLong_ReturnsBadRequest
    [Fact]
    public async Task CreateHealthPromotionEntity_WithAddressStreetTooLong_ReturnsBadRequest()
    {
        var request = new CreateHealthPromotionEntityRequest
        {
            Name = $"TestEPS_{Guid.NewGuid():N}",
            AddressStreet = new string('X', 201), // más de 200 caracteres
            Notes = "Nota"
        };
        var response = await _client.PostAsJsonAsync("/api/v1/HealthPromotionEntities", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 151. CreateHealthPromotionEntity_WithNotesTooLong_ReturnsBadRequest
    [Fact]
    public async Task CreateHealthPromotionEntity_WithNotesTooLong_ReturnsBadRequest()
    {
        var request = new CreateHealthPromotionEntityRequest
        {
            Name = $"TestEPS_{Guid.NewGuid():N}",
            AddressStreet = "Calle 123",
            Notes = new string('Y', 501) // más de 500 caracteres
        };
        var response = await _client.PostAsJsonAsync("/api/v1/HealthPromotionEntities", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ========== GET BY ID ==========

    // 143. GetHealthPromotionEntityById_ReturnsDto
    [Fact]
    public async Task GetHealthPromotionEntityById_WithExisting_ReturnsDto()
    {
        var entityId = await CreateTestHealthPromotionEntityAsync();

        var response = await _client.GetAsync($"/api/v1/HealthPromotionEntities/{entityId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var entity = await response.Content.ReadFromJsonAsync<HealthPromotionEntityDto>();
        entity.Should().NotBeNull();
        entity!.Id.Should().Be(entityId);
        entity.Name.Should().NotBeNullOrEmpty();
    }

    // 152. GetHealthPromotionEntityById_WithNonExisting_ReturnsNotFound
    [Fact]
    public async Task GetHealthPromotionEntityById_WithNonExisting_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v1/HealthPromotionEntities/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ========== GET ALL ==========

    // 144. GetAllHealthPromotionEntities_Paginated
    [Fact]
    public async Task GetAllHealthPromotionEntities_ReturnsPaginatedList()
    {
        await CreateTestHealthPromotionEntityAsync();

        var response = await _client.GetAsync("/api/v1/HealthPromotionEntities?pageNumber=1&pageSize=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<HealthPromotionEntityDto>>();
        pagedResponse.Should().NotBeNull();
        pagedResponse!.Items.Should().NotBeEmpty();
        pagedResponse.PageNumber.Should().Be(1);
        pagedResponse.PageSize.Should().Be(5);
        pagedResponse.TotalCount.Should().BeGreaterThan(0);
    }

    // 153. GetAllHealthPromotionEntities_WithPageBeyondData_ReturnsEmptyItems
    [Fact]
    public async Task GetAllHealthPromotionEntities_WithPageBeyondData_ReturnsEmptyItems()
    {
        var response = await _client.GetAsync("/api/v1/HealthPromotionEntities?pageNumber=9999&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<HealthPromotionEntityDto>>();
        pagedResponse.Should().NotBeNull();
        pagedResponse!.Items.Should().BeEmpty();
        pagedResponse.PageNumber.Should().Be(9999);
        pagedResponse.PageSize.Should().Be(10);
    }

    // ========== UPDATE (PATCH) ==========

    // 145. UpdateHealthPromotionEntity_Valid
    [Fact]
    public async Task UpdateHealthPromotionEntity_WithValidData_Updates()
    {
        var entityId = await CreateTestHealthPromotionEntityAsync();

        var updateRequest = new UpdateHealthPromotionEntityRequest
        {
            Name = "Updated EPS Name",
            AddressStreet = "Nueva Calle 456",
            Notes = "Nota actualizada"
        };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/v1/HealthPromotionEntities/{entityId}", updateRequest);

        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<HealthPromotionEntityDto>();
        updated!.Name.Should().Be("Updated EPS Name");
        updated.AddressStreet.Should().Be("Nueva Calle 456");
        updated.Notes.Should().Be("Nota actualizada");
    }

    // 154. UpdateHealthPromotionEntity_WithNonExisting_ReturnsNotFound
    [Fact]
    public async Task UpdateHealthPromotionEntity_WithNonExisting_ReturnsNotFound()
    {
        var updateRequest = new UpdateHealthPromotionEntityRequest
        {
            Name = "No existe"
        };
        var response = await _client.PatchAsJsonAsync("/api/v1/HealthPromotionEntities/99999", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // 155. UpdateHealthPromotionEntity_PartialUpdateName_OnlyChangesName
    /// <summary>
    /// Todas las propiedades del Update DTO son string? (nullable), por lo que
    /// AutoMapper Condition funciona correctamente: null → no se mapea.
    /// Solo Name cambia; AddressStreet y Notes se preservan.
    /// </summary>
    [Fact]
    public async Task UpdateHealthPromotionEntity_PartialUpdateName_OnlyChangesName()
    {
        var entityId = await CreateTestHealthPromotionEntityAsync();

        // Get original to verify preservation
        var getResponse = await _client.GetAsync($"/api/v1/HealthPromotionEntities/{entityId}");
        var original = await getResponse.Content.ReadFromJsonAsync<HealthPromotionEntityDto>();
        var originalAddress = original!.AddressStreet;
        var originalNotes = original.Notes;

        // PATCH with only Name
        var updateRequest = new UpdateHealthPromotionEntityRequest
        {
            Name = "Partially Updated EPS"
            // AddressStreet = null → Condition lo ignora ✓
            // Notes = null → Condition lo ignora ✓
        };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/v1/HealthPromotionEntities/{entityId}", updateRequest);
        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<HealthPromotionEntityDto>();

        updated!.Name.Should().Be("Partially Updated EPS");
        updated.AddressStreet.Should().Be(originalAddress);
        updated.Notes.Should().Be(originalNotes);
    }

    // 156. UpdateHealthPromotionEntity_PartialUpdateAddress_OnlyChangesAddress
    [Fact]
    public async Task UpdateHealthPromotionEntity_PartialUpdateAddress_OnlyChangesAddress()
    {
        var entityId = await CreateTestHealthPromotionEntityAsync();

        var getResponse = await _client.GetAsync($"/api/v1/HealthPromotionEntities/{entityId}");
        var original = await getResponse.Content.ReadFromJsonAsync<HealthPromotionEntityDto>();
        var originalName = original!.Name;
        var originalNotes = original.Notes;

        var updateRequest = new UpdateHealthPromotionEntityRequest
        {
            AddressStreet = "Nueva Dirección 789"
        };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/v1/HealthPromotionEntities/{entityId}", updateRequest);
        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<HealthPromotionEntityDto>();

        updated!.Name.Should().Be(originalName);
        updated.AddressStreet.Should().Be("Nueva Dirección 789");
        updated.Notes.Should().Be(originalNotes);
    }

    // 157. UpdateHealthPromotionEntity_WithNameTooShort_ReturnsBadRequest
    [Fact]
    public async Task UpdateHealthPromotionEntity_WithNameTooShort_ReturnsBadRequest()
    {
        var entityId = await CreateTestHealthPromotionEntityAsync();

        var updateRequest = new UpdateHealthPromotionEntityRequest
        {
            Name = "AB" // menos de 3 caracteres
        };
        var response = await _client.PatchAsJsonAsync($"/api/v1/HealthPromotionEntities/{entityId}", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 158. UpdateHealthPromotionEntity_WithNameTooLong_ReturnsBadRequest
    [Fact]
    public async Task UpdateHealthPromotionEntity_WithNameTooLong_ReturnsBadRequest()
    {
        var entityId = await CreateTestHealthPromotionEntityAsync();

        var updateRequest = new UpdateHealthPromotionEntityRequest
        {
            Name = new string('A', 101) // más de 100 caracteres
        };
        var response = await _client.PatchAsJsonAsync($"/api/v1/HealthPromotionEntities/{entityId}", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 159. UpdateHealthPromotionEntity_WithAddressStreetTooLong_ReturnsBadRequest
    [Fact]
    public async Task UpdateHealthPromotionEntity_WithAddressStreetTooLong_ReturnsBadRequest()
    {
        var entityId = await CreateTestHealthPromotionEntityAsync();

        var updateRequest = new UpdateHealthPromotionEntityRequest
        {
            Name = "Valid Name",
            AddressStreet = new string('X', 201) // más de 200 caracteres
        };
        var response = await _client.PatchAsJsonAsync($"/api/v1/HealthPromotionEntities/{entityId}", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 160. UpdateHealthPromotionEntity_WithNotesTooLong_ReturnsBadRequest
    [Fact]
    public async Task UpdateHealthPromotionEntity_WithNotesTooLong_ReturnsBadRequest()
    {
        var entityId = await CreateTestHealthPromotionEntityAsync();

        var updateRequest = new UpdateHealthPromotionEntityRequest
        {
            Name = "Valid Name",
            Notes = new string('Y', 501) // más de 500 caracteres
        };
        var response = await _client.PatchAsJsonAsync($"/api/v1/HealthPromotionEntities/{entityId}", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ========== DELETE ==========

    // 146. DeleteHealthPromotionEntity_SoftDelete
    [Fact]
    public async Task DeleteHealthPromotionEntity_SoftDeletes()
    {
        var entityId = await CreateTestHealthPromotionEntityAsync();

        var deleteResponse = await _client.DeleteAsync($"/api/v1/HealthPromotionEntities/{entityId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/v1/HealthPromotionEntities/{entityId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // 161. DeleteHealthPromotionEntity_WithNonExisting_ReturnsNotFound
    [Fact]
    public async Task DeleteHealthPromotionEntity_WithNonExisting_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/v1/HealthPromotionEntities/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // 162. DeleteHealthPromotionEntity_AlreadySoftDeleted_ReturnsNoContent
    [Fact]
    public async Task DeleteHealthPromotionEntity_AlreadySoftDeleted_ReturnsNoContent()
    {
        var entityId = await CreateTestHealthPromotionEntityAsync();

        var firstDelete = await _client.DeleteAsync($"/api/v1/HealthPromotionEntities/{entityId}");
        firstDelete.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Second delete — BaseService.DeleteAsync no detecta que ya está soft-deleted
        var secondDelete = await _client.DeleteAsync($"/api/v1/HealthPromotionEntities/{entityId}");
        secondDelete.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    // ========== AUTHORIZATION ==========

    // 163. Unauthorized_CreateHealthPromotionEntity_WithoutSettingsEdit
    [Fact]
    public async Task Unauthorized_CreateHealthPromotionEntity_WithoutSettingsEdit()
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
        var response = await unauthClient.PostAsJsonAsync("/api/v1/HealthPromotionEntities", request);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // 164. Unauthorized_ViewHealthPromotionEntity_WithoutSettingsView
    [Fact]
    public async Task Unauthorized_ViewHealthPromotionEntity_WithoutSettingsView()
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

        var entityId = await CreateTestHealthPromotionEntityAsync();
        var response = await unauthClient.GetAsync($"/api/v1/HealthPromotionEntities/{entityId}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // 165. Unauthorized_UpdateHealthPromotionEntity_WithoutSettingsEdit
    [Fact]
    public async Task Unauthorized_UpdateHealthPromotionEntity_WithoutSettingsEdit()
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

        var entityId = await CreateTestHealthPromotionEntityAsync();
        var updateRequest = new UpdateHealthPromotionEntityRequest
        {
            Name = "Hacked Name"
        };
        var response = await unauthClient.PatchAsJsonAsync($"/api/v1/HealthPromotionEntities/{entityId}", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // 166. Unauthorized_DeleteHealthPromotionEntity_WithoutSettingsEdit
    [Fact]
    public async Task Unauthorized_DeleteHealthPromotionEntity_WithoutSettingsEdit()
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

        var entityId = await CreateTestHealthPromotionEntityAsync();
        var response = await unauthClient.DeleteAsync($"/api/v1/HealthPromotionEntities/{entityId}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ========== SUB-RECURSOS: EMAIL Y PHONE ==========
    // NOTA: Los endpoints de email/phone no están implementados en el controller.
    // Los tests existen como placeholder para cuando se implementen.

    [Fact(Skip = "HealthPromotionEntity email endpoints not yet implemented")]
    public async Task CreateHealthPromotionEntityEmail_WithValidData_ReturnsCreated()
    {
        var entityId = await CreateTestHealthPromotionEntityAsync();
        var request = new CreateHealthPromotionEntityEmailRequest
        {
            HealthPromotionEntityId = entityId,
            Email = $"test{Guid.NewGuid():N}@example.com"
        };
        var response = await _client.PostAsJsonAsync("/api/v1/HealthPromotionEntities/emails", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var emailDto = await response.Content.ReadFromJsonAsync<HealthPromotionEntityEmailDto>();
        emailDto.Should().NotBeNull();
        emailDto!.Email.Should().Be(request.Email);
        emailDto.HealthPromotionEntityId.Should().Be(entityId);
    }

    [Fact(Skip = "HealthPromotionEntity phone endpoints not yet implemented")]
    public async Task CreateHealthPromotionEntityPhone_WithValidData_ReturnsCreated()
    {
        var entityId = await CreateTestHealthPromotionEntityAsync();
        var request = new CreateHealthPromotionEntityPhoneRequest
        {
            HealthPromotionEntityId = entityId,
            Phone = "3001234567"
        };
        var response = await _client.PostAsJsonAsync("/api/v1/HealthPromotionEntities/phones", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var phoneDto = await response.Content.ReadFromJsonAsync<HealthPromotionEntityPhoneDto>();
        phoneDto.Should().NotBeNull();
        phoneDto!.PhoneNumber.Should().Be(request.Phone);
        phoneDto.HealthPromotionEntityId.Should().Be(entityId);
    }

    [Fact(Skip = "HealthPromotionEntity email endpoints not yet implemented")]
    public async Task GetHealthPromotionEntityEmails_ReturnsList()
    {
        var entityId = await CreateTestHealthPromotionEntityAsync();
        var emailRequest = new CreateHealthPromotionEntityEmailRequest { HealthPromotionEntityId = entityId, Email = "list@test.com" };
        await _client.PostAsJsonAsync("/api/v1/HealthPromotionEntities/emails", emailRequest);

        var response = await _client.GetAsync($"/api/v1/HealthPromotionEntities/{entityId}/emails");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var emails = await response.Content.ReadFromJsonAsync<List<HealthPromotionEntityEmailDto>>();
        emails.Should().Contain(e => e.Email == "list@test.com");
    }

    [Fact(Skip = "HealthPromotionEntity phone endpoints not yet implemented")]
    public async Task GetHealthPromotionEntityPhones_ReturnsList()
    {
        var entityId = await CreateTestHealthPromotionEntityAsync();
        var phoneRequest = new CreateHealthPromotionEntityPhoneRequest { HealthPromotionEntityId = entityId, Phone = "3112223333" };
        await _client.PostAsJsonAsync("/api/v1/HealthPromotionEntities/phones", phoneRequest);

        var response = await _client.GetAsync($"/api/v1/HealthPromotionEntities/{entityId}/phones");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var phones = await response.Content.ReadFromJsonAsync<List<HealthPromotionEntityPhoneDto>>();
        phones.Should().Contain(p => p.PhoneNumber == "3112223333");
    }

    [Fact(Skip = "HealthPromotionEntity email endpoints not yet implemented")]
    public async Task UpdateHealthPromotionEntityEmail_WithValidData_Updates()
    {
        var entityId = await CreateTestHealthPromotionEntityAsync();
        var emailRequest = new CreateHealthPromotionEntityEmailRequest { HealthPromotionEntityId = entityId, Email = "old@test.com" };
        var emailCreateResponse = await _client.PostAsJsonAsync("/api/v1/HealthPromotionEntities/emails", emailRequest);
        var emailDto = await emailCreateResponse.Content.ReadFromJsonAsync<HealthPromotionEntityEmailDto>();

        var updateRequest = new UpdateHealthPromotionEntityEmailRequest { Email = "new@test.com" };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/v1/HealthPromotionEntities/emails/{emailDto!.Id}", updateRequest);
        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<HealthPromotionEntityEmailDto>();
        updated!.Email.Should().Be("new@test.com");
    }

    [Fact(Skip = "HealthPromotionEntity phone endpoints not yet implemented")]
    public async Task UpdateHealthPromotionEntityPhone_WithValidData_Updates()
    {
        var entityId = await CreateTestHealthPromotionEntityAsync();
        var phoneRequest = new CreateHealthPromotionEntityPhoneRequest { HealthPromotionEntityId = entityId, Phone = "3001112222" };
        var phoneCreateResponse = await _client.PostAsJsonAsync("/api/v1/HealthPromotionEntities/phones", phoneRequest);
        var phoneDto = await phoneCreateResponse.Content.ReadFromJsonAsync<HealthPromotionEntityPhoneDto>();

        var updateRequest = new UpdateHealthPromotionEntityPhoneRequest { Phone = "3112223333" };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/v1/HealthPromotionEntities/phones/{phoneDto!.Id}", updateRequest);
        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<HealthPromotionEntityPhoneDto>();
        updated!.PhoneNumber.Should().Be("3112223333");
    }

    [Fact(Skip = "HealthPromotionEntity email endpoints not yet implemented")]
    public async Task DeleteHealthPromotionEntityEmail_Removes()
    {
        var entityId = await CreateTestHealthPromotionEntityAsync();
        var emailRequest = new CreateHealthPromotionEntityEmailRequest { HealthPromotionEntityId = entityId, Email = "delete@test.com" };
        var emailCreateResponse = await _client.PostAsJsonAsync("/api/v1/HealthPromotionEntities/emails", emailRequest);
        var emailDto = await emailCreateResponse.Content.ReadFromJsonAsync<HealthPromotionEntityEmailDto>();

        var deleteResponse = await _client.DeleteAsync($"/api/v1/HealthPromotionEntities/emails/{emailDto!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/v1/HealthPromotionEntities/{entityId}/emails");
        var emails = await getResponse.Content.ReadFromJsonAsync<List<HealthPromotionEntityEmailDto>>();
        emails.Should().NotContain(e => e.Id == emailDto.Id);
    }

    [Fact(Skip = "HealthPromotionEntity phone endpoints not yet implemented")]
    public async Task DeleteHealthPromotionEntityPhone_Removes()
    {
        var entityId = await CreateTestHealthPromotionEntityAsync();
        var phoneRequest = new CreateHealthPromotionEntityPhoneRequest { HealthPromotionEntityId = entityId, Phone = "3004445555" };
        var phoneCreateResponse = await _client.PostAsJsonAsync("/api/v1/HealthPromotionEntities/phones", phoneRequest);
        var phoneDto = await phoneCreateResponse.Content.ReadFromJsonAsync<HealthPromotionEntityPhoneDto>();

        var deleteResponse = await _client.DeleteAsync($"/api/v1/HealthPromotionEntities/phones/{phoneDto!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/v1/HealthPromotionEntities/{entityId}/phones");
        var phones = await getResponse.Content.ReadFromJsonAsync<List<HealthPromotionEntityPhoneDto>>();
        phones.Should().NotContain(p => p.Id == phoneDto.Id);
    }
}
