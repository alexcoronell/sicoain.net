using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using sicoain.IntegrationTests.Fixtures;
using sicoain.IntegrationTests.Utilities;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.DigitalEvidences;
using sicoain.shared.Entities;
using sicoain.shared.Enums;

namespace sicoain.IntegrationTests.Controllers;

public class DigitalEvidencesControllerTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly CookieHandler _cookieHandler;
    private readonly IntegrationTestWebAppFactory _factory;

    public DigitalEvidencesControllerTests(IntegrationTestWebAppFactory factory)
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

    private async Task<int> CreateTestAccidentAsync()
    {
        await using var context = CreateDbContext();

        // Business
        var business = new Business { Name = $"TestBusiness_{Guid.NewGuid():N}" };
        context.Businesses.Add(business);
        await context.SaveChangesAsync();

        // Branch
        var branch = new Branch
        {
            Name = $"TestBranch_{Guid.NewGuid():N}",
            BusinessId = business.Id,
            Business = business
        };
        context.Branches.Add(branch);
        await context.SaveChangesAsync();

        // Department
        var department = new Department { Name = $"TestDept_{Guid.NewGuid():N}" };
        context.Departments.Add(department);
        await context.SaveChangesAsync();

        // RiskClass
        var riskClass = await context.RiskClasses.FirstOrDefaultAsync();
        if (riskClass == null)
        {
            riskClass = new RiskClass { Name = "Clase I", Code = "I", ContributionRate = 0.005m, IsActive = true };
            context.RiskClasses.Add(riskClass);
            await context.SaveChangesAsync();
        }

        // Position
        var position = new Position
        {
            Name = $"TestPosition_{Guid.NewGuid():N}",
            DepartmentId = department.Id,
            Department = department,
            RiskClassId = riskClass.Id,
            RiskClass = riskClass
        };
        context.Positions.Add(position);
        await context.SaveChangesAsync();

        // EPS and ARL
        var eps = await context.HealthPromotionEntities.FirstOrDefaultAsync();
        if (eps == null)
        {
            eps = new HealthPromotionEntity { Name = "TestEPS" };
            context.HealthPromotionEntities.Add(eps);
            await context.SaveChangesAsync();
        }
        var arl = await context.OccupationalRiskAdministrators.FirstOrDefaultAsync();
        if (arl == null)
        {
            arl = new OccupationalRiskAdministrator { Name = "TestARL" };
            context.OccupationalRiskAdministrators.Add(arl);
            await context.SaveChangesAsync();
        }

        // Employee
        var employee = new Employee
        {
            DocumentType = DocumentType.CedulaDeCiudadania,
            DocumentNumber = $"EMP{Guid.NewGuid():N}"[..18],
            FirstName = "Test",
            SecondName = "Employee",
            Surname = "Digital",
            SecondSurname = "Evidence",
            State = "Cundinamarca",
            Municipality = "Bogotá",
            Neighborhood = "Centro",
            AddressStreet = "Calle 123",
            PostalCode = "110111",
            HiringDate = DateTime.Today.AddDays(-30),
            BusinessId = business.Id,
            BranchId = branch.Id,
            PositionId = position.Id,
            DepartmentId = department.Id,
            HealthPromotionEntityId = eps.Id,
            OccupationalRiskAdministratorId = arl.Id
        };
        context.Employees.Add(employee);
        await context.SaveChangesAsync();

        // AccidentType
        var accidentType = new AccidentType { Name = $"TestType_{Guid.NewGuid():N}", Severity = AccidentSeverity.Mild };
        context.AccidentTypes.Add(accidentType);
        await context.SaveChangesAsync();

        // EventCategory
        var eventCategory = new EventCategory { Name = $"TestCategory_{Guid.NewGuid():N}", LevelOfSeverity = AccidentSeverity.Mild };
        context.EventCategories.Add(eventCategory);
        await context.SaveChangesAsync();

        // Accident
        var accident = new Accident
        {
            EventDate = DateTime.Today,
            Description = "Accident for digital evidence test",
            EmployeeId = employee.Id,
            Employee = employee,
            AccidentTypeId = accidentType.Id,
            AccidentType = accidentType,
            EventCategoryId = eventCategory.Id,
            EventCategory = eventCategory
        };
        context.Accidents.Add(accident);
        await context.SaveChangesAsync();

        return accident.Id;
    }

    private string GenerateValidBase64Image() =>
        // 1x1 pixel PNG (transparent) in Base64
        "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==";

    private CreateDigitalEvidenceRequest CreateValidRequest(int accidentId) => new()
    {
        FileName = "evidence.jpg",
        MimeType = "image/jpeg",
        Description = "Foto del lugar del accidente",
        TakenAt = DateTime.UtcNow,
        TakenByName = "Inspector Pérez",
        ChainOfCustody = "Recolectada por inspector en escena",
        AccidentId = accidentId,
        Base64Content = GenerateValidBase64Image()
    };

    /// <summary>
    /// Builds a MultipartFormDataContent for creating a digital evidence.
    /// Skips fields set to null so they are omitted from the request.
    /// </summary>
    private MultipartFormDataContent BuildCreateForm(CreateDigitalEvidenceRequest request)
    {
        var content = new MultipartFormDataContent();
        content.Add(new StringContent(request.FileName), "FileName");
        content.Add(new StringContent(request.MimeType), "MimeType");
        content.Add(new StringContent(request.Description), "Description");
        content.Add(new StringContent(request.TakenAt.ToString("o")), "TakenAt");
        if (request.TakenByName != null)
            content.Add(new StringContent(request.TakenByName), "TakenByName");
        content.Add(new StringContent(request.ChainOfCustody), "ChainOfCustody");
        if (request.AccidentId.HasValue)
            content.Add(new StringContent(request.AccidentId.Value.ToString()), "AccidentId");
        content.Add(new StringContent(request.Base64Content), "Base64Content");
        return content;
    }

    /// <summary>
    /// Builds a MultipartFormDataContent for updating a digital evidence.
    /// Only includes non-null fields so partial updates work correctly.
    /// </summary>
    private MultipartFormDataContent BuildUpdateForm(UpdateDigitalEvidenceRequest request)
    {
        var content = new MultipartFormDataContent();
        if (request.Description != null)
            content.Add(new StringContent(request.Description), "Description");
        if (request.TakenAt.HasValue)
            content.Add(new StringContent(request.TakenAt.Value.ToString("o")), "TakenAt");
        if (request.TakenByName != null)
            content.Add(new StringContent(request.TakenByName), "TakenByName");
        if (request.ChainOfCustody != null)
            content.Add(new StringContent(request.ChainOfCustody), "ChainOfCustody");
        return content;
    }

    /// <summary>
    /// Creates a user with no permission claims and returns an authenticated
    /// HttpClient scoped to that user.
    /// </summary>
    private async Task<(HttpClient Client, string Email)> CreateUnauthenticatedClientAsync()
    {
        var email = $"noperm{Guid.NewGuid():N}@test.com";
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
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
        return (unauthClient, email);
    }

    /// <summary>
    /// Uploads a digital evidence and returns the created DTO.
    /// </summary>
    private async Task<DigitalEvidenceDto> CreateTestEvidenceAsync(int accidentId)
    {
        var request = CreateValidRequest(accidentId);
        using var content = BuildCreateForm(request);
        var response = await _client.PostAsync("/api/v1/DigitalEvidences", content);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<DigitalEvidenceDto>())!;
    }

    #endregion

    // ──────────────────────────────────────────────
    //  CRUD — Happy paths
    // ──────────────────────────────────────────────

    [Fact]
    public async Task UploadDigitalEvidence_ToAccident_ReturnsCreated()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = CreateValidRequest(accidentId);

        using var content = BuildCreateForm(request);
        var response = await _client.PostAsync("/api/v1/DigitalEvidences", content);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var evidence = await response.Content.ReadFromJsonAsync<DigitalEvidenceDto>();
        evidence.Should().NotBeNull();
        evidence!.FileName.Should().Be(request.FileName);
        evidence.Description.Should().Be(request.Description);
        evidence.AccidentId.Should().Be(accidentId);
        evidence.FilePath.Should().NotBeNullOrEmpty();
        evidence.TakenAt.Should().BeCloseTo(request.TakenAt, precision: TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetDigitalEvidenceById_WithExisting_ReturnsDto()
    {
        var accidentId = await CreateTestAccidentAsync();
        var evidence = await CreateTestEvidenceAsync(accidentId);

        var response = await _client.GetAsync($"/api/v1/DigitalEvidences/{evidence.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetched = await response.Content.ReadFromJsonAsync<DigitalEvidenceDto>();
        fetched.Should().NotBeNull();
        fetched!.Id.Should().Be(evidence.Id);
    }

    [Fact]
    public async Task GetByAccidentId_ReturnsEvidences()
    {
        var accidentId = await CreateTestAccidentAsync();
        await CreateTestEvidenceAsync(accidentId);

        var response = await _client.GetAsync($"/api/v1/DigitalEvidences/by-accident/{accidentId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<DigitalEvidenceDto>>();
        list.Should().NotBeEmpty();
        list!.First().AccidentId.Should().Be(accidentId);
    }

    [Fact]
    public async Task GetAllDigitalEvidences_ReturnsPaginatedList()
    {
        var accidentId = await CreateTestAccidentAsync();
        await CreateTestEvidenceAsync(accidentId);

        var response = await _client.GetAsync("/api/v1/DigitalEvidences?pageNumber=1&pageSize=5");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<DigitalEvidenceDto>>();
        pagedResponse.Should().NotBeNull();
        pagedResponse!.Items.Should().NotBeEmpty();
        pagedResponse.PageNumber.Should().Be(1);
        pagedResponse.PageSize.Should().Be(5);
        pagedResponse.TotalCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task UpdateDigitalEvidence_UpdateMetadata()
    {
        var accidentId = await CreateTestAccidentAsync();
        var evidence = await CreateTestEvidenceAsync(accidentId);

        var updateRequest = new UpdateDigitalEvidenceRequest
        {
            Description = "Updated description",
            TakenAt = DateTime.UtcNow.AddHours(-1),
            TakenByName = "Another Inspector",
            ChainOfCustody = "Updated chain"
        };
        using var patchContent = BuildUpdateForm(updateRequest);
        var patchResponse = await _client.PatchAsync($"/api/v1/DigitalEvidences/{evidence.Id}", patchContent);
        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<DigitalEvidenceDto>();
        updated!.Description.Should().Be("Updated description");
        updated.Id.Should().Be(evidence.Id);
        updated.FileName.Should().Be(evidence.FileName);
    }

    [Fact]
    public async Task DeleteDigitalEvidence_RemovesFile()
    {
        var accidentId = await CreateTestAccidentAsync();
        var evidence = await CreateTestEvidenceAsync(accidentId);

        var deleteResponse = await _client.DeleteAsync($"/api/v1/DigitalEvidences/{evidence.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/v1/DigitalEvidences/{evidence.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ──────────────────────────────────────────────
    //  Validation — Create
    // ──────────────────────────────────────────────

    [Fact]
    public async Task UploadDigitalEvidence_WithEmptyFileName_ReturnsBadRequest()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = CreateValidRequest(accidentId) with { FileName = string.Empty };
        using var content = BuildCreateForm(request);

        var response = await _client.PostAsync("/api/v1/DigitalEvidences", content);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadDigitalEvidence_WithFileNameTooLong_ReturnsBadRequest()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = CreateValidRequest(accidentId) with
        {
            FileName = new string('x', 256)
        };
        using var content = BuildCreateForm(request);

        var response = await _client.PostAsync("/api/v1/DigitalEvidences", content);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadDigitalEvidence_WithEmptyMimeType_ReturnsBadRequest()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = CreateValidRequest(accidentId) with { MimeType = string.Empty };
        using var content = BuildCreateForm(request);

        var response = await _client.PostAsync("/api/v1/DigitalEvidences", content);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadDigitalEvidence_WithDescriptionTooLong_ReturnsBadRequest()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = CreateValidRequest(accidentId) with
        {
            Description = new string('a', 501)
        };
        using var content = BuildCreateForm(request);

        var response = await _client.PostAsync("/api/v1/DigitalEvidences", content);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadDigitalEvidence_WithFutureTakenAt_ReturnsBadRequest()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = CreateValidRequest(accidentId) with
        {
            TakenAt = DateTime.UtcNow.AddDays(1)
        };
        using var content = BuildCreateForm(request);

        var response = await _client.PostAsync("/api/v1/DigitalEvidences", content);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadDigitalEvidence_WithEmptyBase64Content_ReturnsBadRequest()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = CreateValidRequest(accidentId) with { Base64Content = string.Empty };
        using var content = BuildCreateForm(request);

        var response = await _client.PostAsync("/api/v1/DigitalEvidences", content);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadDigitalEvidence_WithInvalidBase64_ReturnsBadRequest()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = CreateValidRequest(accidentId) with { Base64Content = "not-valid-base64!!" };
        using var content = BuildCreateForm(request);

        var response = await _client.PostAsync("/api/v1/DigitalEvidences", content);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadDigitalEvidence_WithAccidentIdZero_ReturnsBadRequest()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = CreateValidRequest(accidentId) with { AccidentId = 0 };
        using var content = BuildCreateForm(request);

        var response = await _client.PostAsync("/api/v1/DigitalEvidences", content);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadDigitalEvidence_WithTakenByNameTooLong_ReturnsBadRequest()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = CreateValidRequest(accidentId) with
        {
            TakenByName = new string('n', 151)
        };
        using var content = BuildCreateForm(request);

        var response = await _client.PostAsync("/api/v1/DigitalEvidences", content);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadDigitalEvidence_WithChainOfCustodyTooLong_ReturnsBadRequest()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = CreateValidRequest(accidentId) with
        {
            ChainOfCustody = new string('c', 501)
        };
        using var content = BuildCreateForm(request);

        var response = await _client.PostAsync("/api/v1/DigitalEvidences", content);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ──────────────────────────────────────────────
    //  Validation — Update
    // ──────────────────────────────────────────────

    [Fact]
    public async Task UpdateDigitalEvidence_WithDescriptionTooLong_ReturnsBadRequest()
    {
        var accidentId = await CreateTestAccidentAsync();
        var evidence = await CreateTestEvidenceAsync(accidentId);

        var updateRequest = new UpdateDigitalEvidenceRequest
        {
            Description = new string('a', 501)
        };
        using var patchContent = BuildUpdateForm(updateRequest);
        var response = await _client.PatchAsync($"/api/v1/DigitalEvidences/{evidence.Id}", patchContent);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateDigitalEvidence_WithFutureTakenAt_ReturnsBadRequest()
    {
        var accidentId = await CreateTestAccidentAsync();
        var evidence = await CreateTestEvidenceAsync(accidentId);

        var updateRequest = new UpdateDigitalEvidenceRequest
        {
            TakenAt = DateTime.UtcNow.AddDays(1)
        };
        using var patchContent = BuildUpdateForm(updateRequest);
        var response = await _client.PatchAsync($"/api/v1/DigitalEvidences/{evidence.Id}", patchContent);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ──────────────────────────────────────────────
    //  Negative / Edge cases
    // ──────────────────────────────────────────────

    [Fact]
    public async Task GetDigitalEvidenceById_WithNonExisting_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v1/DigitalEvidences/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetByAccidentId_WithNonExistingAccident_ReturnsEmptyList()
    {
        var response = await _client.GetAsync("/api/v1/DigitalEvidences/by-accident/99999");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<DigitalEvidenceDto>>();
        list.Should().NotBeNull();
        list!.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllDigitalEvidences_WithPageBeyondData_ReturnsEmpty()
    {
        var response = await _client.GetAsync("/api/v1/DigitalEvidences?pageNumber=999&pageSize=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<DigitalEvidenceDto>>();
        pagedResponse.Should().NotBeNull();
        pagedResponse!.Items.Should().BeEmpty();
        pagedResponse.PageNumber.Should().Be(999);
        pagedResponse.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task DeleteDigitalEvidence_WithNonExisting_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/v1/DigitalEvidences/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateDigitalEvidence_WithNonExisting_ReturnsNotFound()
    {
        var updateRequest = new UpdateDigitalEvidenceRequest
        {
            Description = "No matter"
        };
        using var patchContent = BuildUpdateForm(updateRequest);
        var response = await _client.PatchAsync("/api/v1/DigitalEvidences/99999", patchContent);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ──────────────────────────────────────────────
    //  Authorization
    // ──────────────────────────────────────────────

    [Fact]
    public async Task Unauthorized_UploadDigitalEvidence_WithoutAccidentsCreate()
    {
        var (unauthClient, _) = await CreateUnauthenticatedClientAsync();
        var accidentId = await CreateTestAccidentAsync();
        var request = CreateValidRequest(accidentId);
        using var content = BuildCreateForm(request);

        var response = await unauthClient.PostAsync("/api/v1/DigitalEvidences", content);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Unauthorized_GetAllDigitalEvidences_WithoutAccidentsView()
    {
        var (unauthClient, _) = await CreateUnauthenticatedClientAsync();

        var response = await unauthClient.GetAsync("/api/v1/DigitalEvidences");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Unauthorized_GetDigitalEvidenceById_WithoutAccidentsView()
    {
        var (unauthClient, _) = await CreateUnauthenticatedClientAsync();

        var response = await unauthClient.GetAsync("/api/v1/DigitalEvidences/1");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Unauthorized_GetByAccidentId_WithoutAccidentsView()
    {
        var (unauthClient, _) = await CreateUnauthenticatedClientAsync();

        var response = await unauthClient.GetAsync("/api/v1/DigitalEvidences/by-accident/1");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Unauthorized_UpdateDigitalEvidence_WithoutAccidentsEdit()
    {
        var (unauthClient, _) = await CreateUnauthenticatedClientAsync();
        var updateRequest = new UpdateDigitalEvidenceRequest { Description = "hack" };
        using var patchContent = BuildUpdateForm(updateRequest);

        var response = await unauthClient.PatchAsync("/api/v1/DigitalEvidences/1", patchContent);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Unauthorized_DeleteDigitalEvidence_WithoutAccidentsDelete()
    {
        var (unauthClient, _) = await CreateUnauthenticatedClientAsync();

        var response = await unauthClient.DeleteAsync("/api/v1/DigitalEvidences/1");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
