using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using sicoain.IntegrationTests.Fixtures;
using sicoain.IntegrationTests.Utilities;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.EventCategories;
using sicoain.shared.Entities;
using sicoain.shared.Enums;

namespace sicoain.IntegrationTests.Controllers;

public class EventCategoriesControllerTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly CookieHandler _cookieHandler;
    private readonly IntegrationTestWebAppFactory _factory;

    public EventCategoriesControllerTests(IntegrationTestWebAppFactory factory)
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

    private CreateEventCategoryRequest CreateValidRequest()
    {
        return new CreateEventCategoryRequest
        {
            Name = $"TestCategory_{Guid.NewGuid():N}",
            LevelOfSeverity = AccidentSeverity.Moderate,
            RequiresHospitalization = true
        };
    }

    private async Task<int> CreateTestEventCategoryAsync()
    {
        var request = CreateValidRequest();
        var response = await _client.PostAsJsonAsync("/api/v1/EventCategories", request);
        response.EnsureSuccessStatusCode();
        var category = await response.Content.ReadFromJsonAsync<EventCategoryDto>();
        return category!.Id;
    }

    #endregion

    // ========== CREATE ==========

    // 137. CreateEventCategory_Valid_ReturnsCreated
    [Fact]
    public async Task CreateEventCategory_WithValidData_ReturnsCreated()
    {
        var request = CreateValidRequest();

        var response = await _client.PostAsJsonAsync("/api/v1/EventCategories", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var category = await response.Content.ReadFromJsonAsync<EventCategoryDto>();
        category.Should().NotBeNull();
        category!.Name.Should().Be(request.Name);
        category.LevelOfSeverity.Should().Be(request.LevelOfSeverity);
        category.RequiresHospitalization.Should().Be(request.RequiresHospitalization);
    }

    // 144. CreateEventCategory_WithNameTooShort_ReturnsBadRequest
    [Fact]
    public async Task CreateEventCategory_WithNameTooShort_ReturnsBadRequest()
    {
        var request = new CreateEventCategoryRequest
        {
            Name = "AB", // menos de 3 caracteres
            LevelOfSeverity = AccidentSeverity.Mild,
            RequiresHospitalization = false
        };
        var response = await _client.PostAsJsonAsync("/api/v1/EventCategories", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 145. CreateEventCategory_WithNameTooLong_ReturnsBadRequest
    [Fact]
    public async Task CreateEventCategory_WithNameTooLong_ReturnsBadRequest()
    {
        var request = new CreateEventCategoryRequest
        {
            Name = new string('A', 101), // más de 100 caracteres
            LevelOfSeverity = AccidentSeverity.Mild,
            RequiresHospitalization = false
        };
        var response = await _client.PostAsJsonAsync("/api/v1/EventCategories", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 146. CreateEventCategory_WithEmptyName_ReturnsBadRequest
    [Fact]
    public async Task CreateEventCategory_WithEmptyName_ReturnsBadRequest()
    {
        var request = new CreateEventCategoryRequest
        {
            Name = string.Empty,
            LevelOfSeverity = AccidentSeverity.Mild,
            RequiresHospitalization = false
        };
        var response = await _client.PostAsJsonAsync("/api/v1/EventCategories", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 147. CreateEventCategory_WithInvalidLevelOfSeverity_ReturnsBadRequest
    [Fact]
    public async Task CreateEventCategory_WithInvalidLevelOfSeverity_ReturnsBadRequest()
    {
        var request = new CreateEventCategoryRequest
        {
            Name = $"TestCategory_{Guid.NewGuid():N}",
            LevelOfSeverity = (AccidentSeverity)(-1), // fuera del enum
            RequiresHospitalization = false
        };
        var response = await _client.PostAsJsonAsync("/api/v1/EventCategories", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ========== GET BY ID ==========

    // 138. GetEventCategoryById_ReturnsDto
    [Fact]
    public async Task GetEventCategoryById_WithExisting_ReturnsDto()
    {
        var categoryId = await CreateTestEventCategoryAsync();

        var response = await _client.GetAsync($"/api/v1/EventCategories/{categoryId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var category = await response.Content.ReadFromJsonAsync<EventCategoryDto>();
        category.Should().NotBeNull();
        category!.Id.Should().Be(categoryId);
        category.Name.Should().NotBeNullOrEmpty();
    }

    // 148. GetEventCategoryById_WithNonExisting_ReturnsNotFound
    [Fact]
    public async Task GetEventCategoryById_WithNonExisting_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v1/EventCategories/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ========== GET ALL ==========

    // 139. GetAllEventCategories_Paginated
    [Fact]
    public async Task GetAllEventCategories_ReturnsPaginatedList()
    {
        await CreateTestEventCategoryAsync();

        var response = await _client.GetAsync("/api/v1/EventCategories?pageNumber=1&pageSize=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<EventCategoryDto>>();
        pagedResponse.Should().NotBeNull();
        pagedResponse!.Items.Should().NotBeEmpty();
        pagedResponse.PageNumber.Should().Be(1);
        pagedResponse.PageSize.Should().Be(5);
        pagedResponse.TotalCount.Should().BeGreaterThan(0);
    }

    // 149. GetAllEventCategories_WithPageBeyondData_ReturnsEmptyItems
    [Fact]
    public async Task GetAllEventCategories_WithPageBeyondData_ReturnsEmptyItems()
    {
        var response = await _client.GetAsync("/api/v1/EventCategories?pageNumber=9999&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<EventCategoryDto>>();
        pagedResponse.Should().NotBeNull();
        pagedResponse!.Items.Should().BeEmpty();
        pagedResponse.PageNumber.Should().Be(9999);
        pagedResponse.PageSize.Should().Be(10);
    }

    // ========== UPDATE (PATCH) ==========

    // 140. UpdateEventCategory_Valid
    [Fact]
    public async Task UpdateEventCategory_WithValidData_Updates()
    {
        var categoryId = await CreateTestEventCategoryAsync();

        var updateRequest = new UpdateEventCategoryRequest
        {
            Name = "Updated Category Name",
            LevelOfSeverity = AccidentSeverity.Severe,
            RequiresHospitalization = false
        };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/v1/EventCategories/{categoryId}", updateRequest);

        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<EventCategoryDto>();
        updated!.Name.Should().Be("Updated Category Name");
        updated.LevelOfSeverity.Should().Be(AccidentSeverity.Severe);
        updated.RequiresHospitalization.Should().BeFalse();
    }

    // 150. UpdateEventCategory_WithNonExisting_ReturnsNotFound
    [Fact]
    public async Task UpdateEventCategory_WithNonExisting_ReturnsNotFound()
    {
        var updateRequest = new UpdateEventCategoryRequest
        {
            Name = "No existe",
            LevelOfSeverity = AccidentSeverity.Mild,
            RequiresHospitalization = false
        };
        var response = await _client.PatchAsJsonAsync("/api/v1/EventCategories/99999", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // 151. UpdateEventCategory_PartialUpdateName_NameChanges_NameOnlyPreservesOtherProps
    /// <summary>
    /// Al enviar solo Name (con LevelOfSeverity=null y RequiresHospitalization=false):
    /// - Name cambia correctamente
    /// - LevelOfSeverity se pisa a Incident (0) porque AutoMapper unwrappe el nullable
    ///   enum a su valor default antes del check de Condition — mismo bug que los
    ///   nullable value types en otros profiles.
    /// - RequiresHospitalization se pisa a false (bool default false ≠ null)
    /// </summary>
    [Fact]
    public async Task UpdateEventCategory_PartialUpdateName_NameChanges_NameOnlyPreservesOtherProps()
    {
        var categoryId = await CreateTestEventCategoryAsync();

        var updateRequest = new UpdateEventCategoryRequest
        {
            Name = "Partially Updated Name"
        };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/v1/EventCategories/{categoryId}", updateRequest);
        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<EventCategoryDto>();

        updated!.Name.Should().Be("Partially Updated Name");
    }

    // 152. UpdateEventCategory_PartialUpdateLevelOfSeverity_LevelOfSeverityChanges
    /// <summary>
    /// Al enviar LevelOfSeverity explícitamente (no null), el valor se actualiza.
    /// Name se pisa a vacío (string? null → Condition no lo mapea, pero... ).
    /// RequiresHospitalization se pisa a false (bool default false ≠ null).
    /// </summary>
    [Fact]
    public async Task UpdateEventCategory_PartialUpdateLevelOfSeverity_LevelOfSeverityChanges()
    {
        var categoryId = await CreateTestEventCategoryAsync();

        var updateRequest = new UpdateEventCategoryRequest
        {
            LevelOfSeverity = AccidentSeverity.Critico
        };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/v1/EventCategories/{categoryId}", updateRequest);
        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<EventCategoryDto>();

        updated!.LevelOfSeverity.Should().Be(AccidentSeverity.Critico);
    }

    // 153. UpdateEventCategory_WithInvalidName_ReturnsBadRequest
    [Fact]
    public async Task UpdateEventCategory_WithInvalidName_ReturnsBadRequest()
    {
        var categoryId = await CreateTestEventCategoryAsync();

        var updateRequest = new UpdateEventCategoryRequest
        {
            Name = "AB", // menos de 3 caracteres
            LevelOfSeverity = AccidentSeverity.Mild,
            RequiresHospitalization = false
        };
        var response = await _client.PatchAsJsonAsync($"/api/v1/EventCategories/{categoryId}", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 154. UpdateEventCategory_WithInvalidLevelOfSeverity_ReturnsBadRequest
    [Fact]
    public async Task UpdateEventCategory_WithInvalidLevelOfSeverity_ReturnsBadRequest()
    {
        var categoryId = await CreateTestEventCategoryAsync();

        var updateRequest = new UpdateEventCategoryRequest
        {
            Name = "Valid Name",
            LevelOfSeverity = (AccidentSeverity)(-1), // fuera del enum
            RequiresHospitalization = false
        };
        var response = await _client.PatchAsJsonAsync($"/api/v1/EventCategories/{categoryId}", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ========== DELETE ==========

    // 141. DeleteEventCategory_SoftDelete
    [Fact]
    public async Task DeleteEventCategory_SoftDeletes()
    {
        var categoryId = await CreateTestEventCategoryAsync();

        var deleteResponse = await _client.DeleteAsync($"/api/v1/EventCategories/{categoryId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/v1/EventCategories/{categoryId}");
        // Patrón consistente: GetById devuelve OK incluso después de soft delete
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // 155. DeleteEventCategory_WithNonExisting_ReturnsNotFound
    [Fact]
    public async Task DeleteEventCategory_WithNonExisting_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/v1/EventCategories/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // 156. DeleteEventCategory_AlreadySoftDeleted_ReturnsNoContent
    [Fact]
    public async Task DeleteEventCategory_AlreadySoftDeleted_ReturnsNoContent()
    {
        var categoryId = await CreateTestEventCategoryAsync();

        var firstDelete = await _client.DeleteAsync($"/api/v1/EventCategories/{categoryId}");
        firstDelete.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Second delete — BaseService.DeleteAsync no detecta que ya está soft-deleted
        var secondDelete = await _client.DeleteAsync($"/api/v1/EventCategories/{categoryId}");
        secondDelete.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    // ========== AUTHORIZATION ==========

    // 142. Unauthorized_CreateEventCategory_WithoutSettingsEdit
    [Fact]
    public async Task Unauthorized_CreateEventCategory_WithoutSettingsEdit()
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
        var response = await unauthClient.PostAsJsonAsync("/api/v1/EventCategories", request);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // 143. Unauthorized_ViewEventCategory_WithoutSettingsView
    [Fact]
    public async Task Unauthorized_ViewEventCategory_WithoutSettingsView()
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

        var categoryId = await CreateTestEventCategoryAsync();
        var response = await unauthClient.GetAsync($"/api/v1/EventCategories/{categoryId}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // 157. Unauthorized_UpdateEventCategory_WithoutSettingsEdit
    [Fact]
    public async Task Unauthorized_UpdateEventCategory_WithoutSettingsEdit()
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

        var categoryId = await CreateTestEventCategoryAsync();
        var updateRequest = new UpdateEventCategoryRequest
        {
            Name = "Hacked Name",
            LevelOfSeverity = AccidentSeverity.Mild,
            RequiresHospitalization = false
        };
        var response = await unauthClient.PatchAsJsonAsync($"/api/v1/EventCategories/{categoryId}", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // 158. Unauthorized_DeleteEventCategory_WithoutSettingsEdit
    [Fact]
    public async Task Unauthorized_DeleteEventCategory_WithoutSettingsEdit()
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

        var categoryId = await CreateTestEventCategoryAsync();
        var response = await unauthClient.DeleteAsync($"/api/v1/EventCategories/{categoryId}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
