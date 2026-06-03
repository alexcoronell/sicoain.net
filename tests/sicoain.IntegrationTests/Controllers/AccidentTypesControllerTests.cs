using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using sicoain.IntegrationTests.Fixtures;
using sicoain.IntegrationTests.Utilities;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.AccidentTypes;
using sicoain.shared.Entities;
using sicoain.shared.Enums;

namespace sicoain.IntegrationTests.Controllers;

public class AccidentTypesControllerTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly CookieHandler _cookieHandler;
    private readonly IntegrationTestWebAppFactory _factory;

    public AccidentTypesControllerTests(IntegrationTestWebAppFactory factory)
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

    private CreateAccidentTypeRequest CreateValidRequest()
    {
        return new CreateAccidentTypeRequest
        {
            Name = $"TestType_{Guid.NewGuid():N}",
            Description = "Descripción del tipo de accidente",
            Severity = AccidentSeverity.Mild
        };
    }

    private async Task<int> CreateTestAccidentTypeAsync()
    {
        var request = CreateValidRequest();
        var response = await _client.PostAsJsonAsync("/api/v1/AccidentTypes", request);
        response.EnsureSuccessStatusCode();
        var accidentType = await response.Content.ReadFromJsonAsync<AccidentTypeDto>();
        return accidentType!.Id;
    }

    #endregion

    // ========== CREATE ==========

    // 132. CreateAccidentType_Valid_ReturnsCreated
    [Fact]
    public async Task CreateAccidentType_WithValidData_ReturnsCreated()
    {
        var request = CreateValidRequest();

        var response = await _client.PostAsJsonAsync("/api/v1/AccidentTypes", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var accidentType = await response.Content.ReadFromJsonAsync<AccidentTypeDto>();
        accidentType.Should().NotBeNull();
        accidentType!.Name.Should().Be(request.Name);
        accidentType.Description.Should().Be(request.Description);
        accidentType.Severity.Should().Be(request.Severity);
    }

    // 139. CreateAccidentType_WithNameTooShort_ReturnsBadRequest
    [Fact]
    public async Task CreateAccidentType_WithNameTooShort_ReturnsBadRequest()
    {
        var request = new CreateAccidentTypeRequest
        {
            Name = "AB", // menos de 3 caracteres
            Description = "Válido",
            Severity = AccidentSeverity.Mild
        };
        var response = await _client.PostAsJsonAsync("/api/v1/AccidentTypes", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 140. CreateAccidentType_WithNameTooLong_ReturnsBadRequest
    [Fact]
    public async Task CreateAccidentType_WithNameTooLong_ReturnsBadRequest()
    {
        var request = new CreateAccidentTypeRequest
        {
            Name = new string('A', 101), // más de 100 caracteres
            Description = "Válido",
            Severity = AccidentSeverity.Mild
        };
        var response = await _client.PostAsJsonAsync("/api/v1/AccidentTypes", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 141. CreateAccidentType_WithEmptyName_ReturnsBadRequest
    [Fact]
    public async Task CreateAccidentType_WithEmptyName_ReturnsBadRequest()
    {
        var request = new CreateAccidentTypeRequest
        {
            Name = string.Empty,
            Description = "Válido",
            Severity = AccidentSeverity.Mild
        };
        var response = await _client.PostAsJsonAsync("/api/v1/AccidentTypes", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 142. CreateAccidentType_WithInvalidSeverity_ReturnsBadRequest
    [Fact]
    public async Task CreateAccidentType_WithInvalidSeverity_ReturnsBadRequest()
    {
        var request = new CreateAccidentTypeRequest
        {
            Name = $"TestType_{Guid.NewGuid():N}",
            Description = "Válido",
            Severity = (AccidentSeverity)(-1) // fuera del enum
        };
        var response = await _client.PostAsJsonAsync("/api/v1/AccidentTypes", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 143. CreateAccidentType_WithDescriptionTooLong_ReturnsBadRequest
    [Fact]
    public async Task CreateAccidentType_WithDescriptionTooLong_ReturnsBadRequest()
    {
        var request = new CreateAccidentTypeRequest
        {
            Name = $"TestType_{Guid.NewGuid():N}",
            Description = new string('C', 251), // más de 250 caracteres
            Severity = AccidentSeverity.Mild
        };
        var response = await _client.PostAsJsonAsync("/api/v1/AccidentTypes", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ========== GET BY ID ==========

    // 133. GetAccidentTypeById_ReturnsDto
    [Fact]
    public async Task GetAccidentTypeById_WithExisting_ReturnsDto()
    {
        var accidentTypeId = await CreateTestAccidentTypeAsync();

        var response = await _client.GetAsync($"/api/v1/AccidentTypes/{accidentTypeId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var accidentType = await response.Content.ReadFromJsonAsync<AccidentTypeDto>();
        accidentType.Should().NotBeNull();
        accidentType!.Id.Should().Be(accidentTypeId);
        accidentType.Name.Should().NotBeNullOrEmpty();
    }

    // 144. GetAccidentTypeById_WithNonExisting_ReturnsNotFound
    [Fact]
    public async Task GetAccidentTypeById_WithNonExisting_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v1/AccidentTypes/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ========== GET ALL ==========

    // 134. GetAllAccidentTypes_Paginated
    [Fact]
    public async Task GetAllAccidentTypes_ReturnsPaginatedList()
    {
        await CreateTestAccidentTypeAsync();

        var response = await _client.GetAsync("/api/v1/AccidentTypes?pageNumber=1&pageSize=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<AccidentTypeDto>>();
        pagedResponse.Should().NotBeNull();
        pagedResponse!.Items.Should().NotBeEmpty();
        pagedResponse.PageNumber.Should().Be(1);
        pagedResponse.PageSize.Should().Be(5);
        pagedResponse.TotalCount.Should().BeGreaterThan(0);
    }

    // 145. GetAllAccidentTypes_WithPageBeyondData_ReturnsEmptyItems
    [Fact]
    public async Task GetAllAccidentTypes_WithPageBeyondData_ReturnsEmptyItems()
    {
        var response = await _client.GetAsync("/api/v1/AccidentTypes?pageNumber=9999&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<AccidentTypeDto>>();
        pagedResponse.Should().NotBeNull();
        pagedResponse!.Items.Should().BeEmpty();
        pagedResponse.PageNumber.Should().Be(9999);
        pagedResponse.PageSize.Should().Be(10);
    }

    // ========== UPDATE (PATCH) ==========

    // 135. UpdateAccidentType_Valid
    [Fact]
    public async Task UpdateAccidentType_WithValidData_Updates()
    {
        var accidentTypeId = await CreateTestAccidentTypeAsync();

        var updateRequest = new UpdateAccidentTypeRequest
        {
            Name = "Updated Accident Type Name",
            Description = "Descripción actualizada",
            Severity = AccidentSeverity.Severe
        };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/v1/AccidentTypes/{accidentTypeId}", updateRequest);

        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<AccidentTypeDto>();
        updated!.Name.Should().Be("Updated Accident Type Name");
        updated.Description.Should().Be("Descripción actualizada");
        updated.Severity.Should().Be(AccidentSeverity.Severe);
    }

    // 146. UpdateAccidentType_WithNonExisting_ReturnsNotFound
    [Fact]
    public async Task UpdateAccidentType_WithNonExisting_ReturnsNotFound()
    {
        var updateRequest = new UpdateAccidentTypeRequest
        {
            Name = "No existe",
            Description = "No existe",
            Severity = AccidentSeverity.Mild
        };
        var response = await _client.PatchAsJsonAsync("/api/v1/AccidentTypes/99999", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // 147. UpdateAccidentType_PartialUpdateDescription_OnlyChangesDescriptionAndSeverity
    /// <summary>
    /// Verifica que al enviar solo Description en el PATCH, el Name se mantiene
    /// (porque es string? null → AutoMapper Condition lo ignora).
    /// Severity se sobreescribe porque es un enum non-nullable que siempre se
    /// envía como 0 (Incident) cuando el cliente lo omite — limitación conocida
    /// del DTO con propiedades non-nullable.
    /// </summary>
    [Fact]
    public async Task UpdateAccidentType_PartialUpdateDescription_OnlyChangesDescriptionAndSeverity()
    {
        var accidentTypeId = await CreateTestAccidentTypeAsync();

        // Get original to know its Name
        var getResponse = await _client.GetAsync($"/api/v1/AccidentTypes/{accidentTypeId}");
        var original = await getResponse.Content.ReadFromJsonAsync<AccidentTypeDto>();
        var originalName = original!.Name;

        // PATCH with only Description (Name y Severity van a tener valores default)
        var updateRequest = new UpdateAccidentTypeRequest
        {
            Description = "Descripción parcialmente actualizada"
            // Name = null → AutoMapper Condition lo ignora ✓
            // Severity = Incident (default 0) → AutoMapper Condition lo acepta (no es null) ✗
        };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/v1/AccidentTypes/{accidentTypeId}", updateRequest);
        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<AccidentTypeDto>();

        // Name se mantiene porque vino null en el request
        updated!.Name.Should().Be(originalName);
        // Description se actualizó
        updated.Description.Should().Be("Descripción parcialmente actualizada");
        // Severity se pisó a Incident porque el valor default (0) no es null
        updated.Severity.Should().Be(AccidentSeverity.Incident);
    }

    // 148. UpdateAccidentType_WithInvalidName_ReturnsBadRequest
    [Fact]
    public async Task UpdateAccidentType_WithInvalidName_ReturnsBadRequest()
    {
        var accidentTypeId = await CreateTestAccidentTypeAsync();

        var updateRequest = new UpdateAccidentTypeRequest
        {
            Name = "AB", // menos de 3 caracteres
            Severity = AccidentSeverity.Mild
        };
        var response = await _client.PatchAsJsonAsync($"/api/v1/AccidentTypes/{accidentTypeId}", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ========== DELETE ==========

    // 136. DeleteAccidentType_SoftDelete
    [Fact]
    public async Task DeleteAccidentType_SoftDeletes()
    {
        var accidentTypeId = await CreateTestAccidentTypeAsync();

        var deleteResponse = await _client.DeleteAsync($"/api/v1/AccidentTypes/{accidentTypeId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/v1/AccidentTypes/{accidentTypeId}");
        // Patrón consistente: GetById devuelve OK incluso después de soft delete
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // 149. DeleteAccidentType_WithNonExisting_ReturnsNotFound
    [Fact]
    public async Task DeleteAccidentType_WithNonExisting_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/v1/AccidentTypes/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // 150. DeleteAccidentType_AlreadySoftDeleted_ReturnsNoContent
    [Fact]
    public async Task DeleteAccidentType_AlreadySoftDeleted_ReturnsNoContent()
    {
        var accidentTypeId = await CreateTestAccidentTypeAsync();

        // First delete
        var firstDelete = await _client.DeleteAsync($"/api/v1/AccidentTypes/{accidentTypeId}");
        firstDelete.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Second delete — BaseService.DeleteAsync no detecta que ya está soft-deleted
        var secondDelete = await _client.DeleteAsync($"/api/v1/AccidentTypes/{accidentTypeId}");
        secondDelete.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    // ========== AUTHORIZATION ==========

    // 137. Unauthorized_CreateAccidentType_WithoutSettingsEdit
    [Fact]
    public async Task Unauthorized_CreateAccidentType_WithoutPermission()
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
        var response = await unauthClient.PostAsJsonAsync("/api/v1/AccidentTypes", request);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // 138. Unauthorized_ViewAccidentType_WithoutSettingsView
    [Fact]
    public async Task Unauthorized_ViewAccidentType_WithoutPermission()
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

        var accidentTypeId = await CreateTestAccidentTypeAsync();
        var response = await unauthClient.GetAsync($"/api/v1/AccidentTypes/{accidentTypeId}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // 151. Unauthorized_UpdateAccidentType_WithoutSettingsEdit
    [Fact]
    public async Task Unauthorized_UpdateAccidentType_WithoutPermission()
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

        var accidentTypeId = await CreateTestAccidentTypeAsync();
        var updateRequest = new UpdateAccidentTypeRequest
        {
            Name = "Hacked Name",
            Severity = AccidentSeverity.Mild
        };
        var response = await unauthClient.PatchAsJsonAsync($"/api/v1/AccidentTypes/{accidentTypeId}", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // 152. Unauthorized_DeleteAccidentType_WithoutSettingsEdit
    [Fact]
    public async Task Unauthorized_DeleteAccidentType_WithoutPermission()
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

        var accidentTypeId = await CreateTestAccidentTypeAsync();
        var response = await unauthClient.DeleteAsync($"/api/v1/AccidentTypes/{accidentTypeId}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
