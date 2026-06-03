using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using sicoain.IntegrationTests.Fixtures;
using sicoain.IntegrationTests.Utilities;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Attachments;
using sicoain.shared.Entities;
using sicoain.shared.Enums;

namespace sicoain.IntegrationTests.Controllers;

public class AttachmentsControllerTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly CookieHandler _cookieHandler;
    private readonly IntegrationTestWebAppFactory _factory;

    public AttachmentsControllerTests(IntegrationTestWebAppFactory factory)
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

    #region Helpers para crear entidades relacionadas (Accident, Employee)

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
        var branch = new Branch { Name = $"TestBranch_{Guid.NewGuid():N}", BusinessId = business.Id, Business = business };
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
            Surname = "Attachment",
            SecondSurname = "Test",
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
            Description = "Accident for attachment test",
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

    private async Task<int> CreateTestEmployeeAsync()
    {
        await using var context = CreateDbContext();

        // Business
        var business = new Business { Name = $"TestBusiness_{Guid.NewGuid():N}" };
        context.Businesses.Add(business);
        await context.SaveChangesAsync();

        // Branch
        var branch = new Branch { Name = $"TestBranch_{Guid.NewGuid():N}", BusinessId = business.Id, Business = business };
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
            FirstName = "Employee",
            SecondName = "For",
            Surname = "Attachment",
            SecondSurname = "Test",
            State = "Cundinamarca",
            Municipality = "Bogotá",
            Neighborhood = "Centro",
            AddressStreet = "Calle 456",
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

        return employee.Id;
    }

    #endregion

    #region Helpers para Attachment

    private string GenerateValidBase64Image()
    {
        // 1x1 pixel PNG (transparent) in Base64
        return "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==";
    }

    private CreateAttachmentRequest CreateValidAttachmentRequestForAccident(int accidentId)
    {
        return new CreateAttachmentRequest
        {
            FileName = "test.png",
            MimeType = "image/png",
            Description = "Test attachment for accident",
            EntityType = AttachmentEntityType.Accident,
            EntityId = accidentId,
            Base64Content = GenerateValidBase64Image()
        };
    }

    private CreateAttachmentRequest CreateValidAttachmentRequestForEmployee(int employeeId)
    {
        return new CreateAttachmentRequest
        {
            FileName = "employee_doc.pdf",
            MimeType = "application/pdf",
            Description = "Employee document",
            EntityType = AttachmentEntityType.Employee,
            EntityId = employeeId,
            Base64Content = GenerateValidBase64Image() // same, but for test
        };
    }

    private MultipartFormDataContent CreateMultipartContent(CreateAttachmentRequest request)
    {
        var content = new MultipartFormDataContent();
        content.Add(new StringContent(request.FileName), "FileName");
        content.Add(new StringContent(request.MimeType), "MimeType");
        content.Add(new StringContent(request.Description), "Description");
        content.Add(new StringContent(((int)request.EntityType).ToString()), "EntityType");
        content.Add(new StringContent(request.EntityId.ToString()), "EntityId");
        content.Add(new StringContent(request.Base64Content), "Base64Content");
        return content;
    }

    private async Task<int> CreateTestAttachmentAsync(int accidentId)
    {
        var request = CreateValidAttachmentRequestForAccident(accidentId);
        using var content = CreateMultipartContent(request);
        var response = await _client.PostAsync("/api/v1/Attachments", content);
        response.EnsureSuccessStatusCode();
        var attachment = await response.Content.ReadFromJsonAsync<AttachmentDto>();
        return attachment!.Id;
    }

    #endregion

    // ========== UPLOAD (CREATE) ==========

    [Fact]
    public async Task UploadAttachment_ToAccident_ReturnsCreated()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = CreateValidAttachmentRequestForAccident(accidentId);
        using var content = CreateMultipartContent(request);

        var response = await _client.PostAsync("/api/v1/Attachments", content);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var attachment = await response.Content.ReadFromJsonAsync<AttachmentDto>();
        attachment.Should().NotBeNull();
        attachment!.FileName.Should().Be(request.FileName);
        attachment.EntityType.Should().Be(request.EntityType);
        attachment.EntityId.Should().Be(accidentId);
        attachment.FilePath.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task UploadAttachment_ToEmployee_ReturnsCreated()
    {
        var employeeId = await CreateTestEmployeeAsync();
        var request = CreateValidAttachmentRequestForEmployee(employeeId);
        using var content = CreateMultipartContent(request);

        var response = await _client.PostAsync("/api/v1/Attachments", content);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var attachment = await response.Content.ReadFromJsonAsync<AttachmentDto>();
        attachment.Should().NotBeNull();
        attachment!.FileName.Should().Be(request.FileName);
        attachment.EntityType.Should().Be(request.EntityType);
        attachment.EntityId.Should().Be(employeeId);
    }

    [Fact]
    public async Task UploadAttachment_WithInvalidEntityType_ReturnsBadRequest()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = CreateValidAttachmentRequestForAccident(accidentId);
        // Invalid EntityType (e.g., 99)
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(request.FileName), "FileName");
        content.Add(new StringContent(request.MimeType), "MimeType");
        content.Add(new StringContent(request.Description), "Description");
        content.Add(new StringContent("99"), "EntityType");
        content.Add(new StringContent(request.EntityId.ToString()), "EntityId");
        content.Add(new StringContent(request.Base64Content), "Base64Content");

        var response = await _client.PostAsync("/api/v1/Attachments", content);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadAttachment_WithInvalidBase64_ReturnsBadRequest()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = CreateValidAttachmentRequestForAccident(accidentId);
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(request.FileName), "FileName");
        content.Add(new StringContent(request.MimeType), "MimeType");
        content.Add(new StringContent(request.Description), "Description");
        content.Add(new StringContent(((int)request.EntityType).ToString()), "EntityType");
        content.Add(new StringContent(request.EntityId.ToString()), "EntityId");
        content.Add(new StringContent("This is not base64!"), "Base64Content");

        var response = await _client.PostAsync("/api/v1/Attachments", content);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadAttachment_WithEmptyFileName_ReturnsBadRequest()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = CreateValidAttachmentRequestForAccident(accidentId) with { FileName = string.Empty };
        using var content = CreateMultipartContent(request);

        var response = await _client.PostAsync("/api/v1/Attachments", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadAttachment_WithEmptyMimeType_ReturnsBadRequest()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = CreateValidAttachmentRequestForAccident(accidentId) with { MimeType = string.Empty };
        using var content = CreateMultipartContent(request);

        var response = await _client.PostAsync("/api/v1/Attachments", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadAttachment_WithDescriptionTooLong_ReturnsBadRequest()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = CreateValidAttachmentRequestForAccident(accidentId) with
        {
            Description = new string('X', 501)
        };
        using var content = CreateMultipartContent(request);

        var response = await _client.PostAsync("/api/v1/Attachments", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadAttachment_WithEntityIdZero_ReturnsBadRequest()
    {
        var request = new CreateAttachmentRequest
        {
            FileName = "test.png",
            MimeType = "image/png",
            Description = "Test with entityId zero",
            EntityType = AttachmentEntityType.Accident,
            EntityId = 0,
            Base64Content = GenerateValidBase64Image()
        };
        using var content = CreateMultipartContent(request);

        var response = await _client.PostAsync("/api/v1/Attachments", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UploadAttachment_WithEmptyBase64Content_ReturnsBadRequest()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = CreateValidAttachmentRequestForAccident(accidentId) with { Base64Content = string.Empty };
        using var content = CreateMultipartContent(request);

        var response = await _client.PostAsync("/api/v1/Attachments", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ========== GET BY ID ==========

    [Fact]
    public async Task GetAttachmentById_WithExisting_ReturnsDto()
    {
        var accidentId = await CreateTestAccidentAsync();
        var attachmentId = await CreateTestAttachmentAsync(accidentId);

        var response = await _client.GetAsync($"/api/v1/Attachments/{attachmentId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var attachment = await response.Content.ReadFromJsonAsync<AttachmentDto>();
        attachment.Should().NotBeNull();
        attachment!.Id.Should().Be(attachmentId);
        attachment.FileName.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetAttachmentById_WithNonExisting_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v1/Attachments/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ========== GET ALL ==========

    [Fact]
    public async Task GetAllAttachments_ReturnsPaginatedList()
    {
        var accidentId = await CreateTestAccidentAsync();
        await CreateTestAttachmentAsync(accidentId);

        var response = await _client.GetAsync("/api/v1/Attachments?pageNumber=1&pageSize=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<AttachmentDto>>();
        pagedResponse.Should().NotBeNull();
        pagedResponse!.Items.Should().NotBeEmpty();
        pagedResponse.PageNumber.Should().Be(1);
        pagedResponse.PageSize.Should().Be(5);
        pagedResponse.TotalCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetAllAttachments_WithPageBeyondData_ReturnsEmpty()
    {
        var response = await _client.GetAsync("/api/v1/Attachments?pageNumber=9999&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<AttachmentDto>>();
        pagedResponse.Should().NotBeNull();
        pagedResponse!.Items.Should().BeEmpty();
        pagedResponse.PageNumber.Should().Be(9999);
        pagedResponse.TotalCount.Should().BeGreaterThanOrEqualTo(0);
    }

    // ========== GET BY ENTITY ==========

    [Fact]
    public async Task GetAttachmentsByEntityId_ReturnsList()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = CreateValidAttachmentRequestForAccident(accidentId);
        using var content = CreateMultipartContent(request);
        await _client.PostAsync("/api/v1/Attachments", content);

        var response = await _client.GetAsync($"/api/v1/Attachments/by-entity?entityType={(int)AttachmentEntityType.Accident}&id={accidentId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var attachments = await response.Content.ReadFromJsonAsync<List<AttachmentDto>>();
        attachments.Should().NotBeEmpty();
        attachments!.First().EntityId.Should().Be(accidentId);
    }

    [Fact]
    public async Task GetAttachmentsByEntityId_WithNonExistingEntity_ReturnsEmptyList()
    {
        var response = await _client.GetAsync($"/api/v1/Attachments/by-entity?entityType={(int)AttachmentEntityType.Accident}&id=999999");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var attachments = await response.Content.ReadFromJsonAsync<List<AttachmentDto>>();
        attachments.Should().NotBeNull();
        attachments.Should().BeEmpty();
    }

    // ========== UPDATE METADATA ==========

    [Fact]
    public async Task UpdateAttachmentMetadata_OnlyDescription()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = CreateValidAttachmentRequestForAccident(accidentId);
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(request.FileName), "FileName");
        content.Add(new StringContent(request.MimeType), "MimeType");
        content.Add(new StringContent("Original description"), "Description");
        content.Add(new StringContent(((int)request.EntityType).ToString()), "EntityType");
        content.Add(new StringContent(request.EntityId.ToString()), "EntityId");
        content.Add(new StringContent(request.Base64Content), "Base64Content");
        var uploadResponse = await _client.PostAsync("/api/v1/Attachments", content);
        var attachment = await uploadResponse.Content.ReadFromJsonAsync<AttachmentDto>();

        var updateRequest = new UpdateAttachmentRequest { Description = "Updated description" };
        using var updateContent = new MultipartFormDataContent();
        updateContent.Add(new StringContent(updateRequest.Description!), "Description");
        var patchResponse = await _client.PatchAsync($"/api/v1/Attachments/{attachment!.Id}", updateContent);
        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<AttachmentDto>();
        updated!.Description.Should().Be("Updated description");
    }

    [Fact]
    public async Task UpdateAttachmentMetadata_WithNonExisting_ReturnsNotFound()
    {
        var updateRequest = new UpdateAttachmentRequest { Description = "Updated description" };
        using var updateContent = new MultipartFormDataContent();
        updateContent.Add(new StringContent(updateRequest.Description!), "Description");

        var response = await _client.PatchAsync("/api/v1/Attachments/999999", updateContent);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateAttachmentMetadata_WithDescriptionTooLong_ReturnsBadRequest()
    {
        var accidentId = await CreateTestAccidentAsync();
        var attachmentId = await CreateTestAttachmentAsync(accidentId);

        var updateRequest = new UpdateAttachmentRequest { Description = new string('X', 501) };
        using var updateContent = new MultipartFormDataContent();
        updateContent.Add(new StringContent(updateRequest.Description), "Description");

        var response = await _client.PatchAsync($"/api/v1/Attachments/{attachmentId}", updateContent);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ========== DELETE ==========

    [Fact]
    public async Task DeleteAttachment_RemovesFileAndRecord()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = CreateValidAttachmentRequestForAccident(accidentId);
        using var content = CreateMultipartContent(request);
        var uploadResponse = await _client.PostAsync("/api/v1/Attachments", content);
        var attachment = await uploadResponse.Content.ReadFromJsonAsync<AttachmentDto>();

        var deleteResponse = await _client.DeleteAsync($"/api/v1/Attachments/{attachment!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/v1/Attachments/{attachment.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteAttachment_WithNonExisting_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/v1/Attachments/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ========== AUTHORIZATION ==========

    [Fact]
    public async Task Unauthorized_UploadToAccident_WithoutPermission()
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

        var accidentId = await CreateTestAccidentAsync();
        var request = CreateValidAttachmentRequestForAccident(accidentId);
        using var content = CreateMultipartContent(request);

        var response = await unauthClient.PostAsync("/api/v1/Attachments", content);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Unauthorized_UploadToEmployee_WithoutPermission()
    {
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var email = $"noperm2{Guid.NewGuid():N}@test.com";
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

        var employeeId = await CreateTestEmployeeAsync();
        var request = CreateValidAttachmentRequestForEmployee(employeeId);
        using var content = CreateMultipartContent(request);

        var response = await unauthClient.PostAsync("/api/v1/Attachments", content);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Unauthorized_GetAttachmentById_WithoutAccidentsView()
    {
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var email = $"noviewatt{Guid.NewGuid():N}@test.com";
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

        // Admin creates an attachment
        var accidentId = await CreateTestAccidentAsync();
        var attachmentId = await CreateTestAttachmentAsync(accidentId);

        // Unauthorized user tries to get it
        var response = await unauthClient.GetAsync($"/api/v1/Attachments/{attachmentId}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Unauthorized_UpdateAttachment_WithoutAccidentsEdit()
    {
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var email = $"noupdatt{Guid.NewGuid():N}@test.com";
        var user = new User
        {
            UserName = email,
            Email = email,
            FullName = "No Update User",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        await userManager.CreateAsync(user, "NoUpd123!");

        var unauthHandler = new CookieHandler();
        var unauthClient = _factory.CreateDefaultClient(unauthHandler);
        await unauthClient.PostAsJsonAsync("/api/v1/Auth/login", new { Email = email, Password = "NoUpd123!" });

        // Admin creates an attachment
        var accidentId = await CreateTestAccidentAsync();
        var attachmentId = await CreateTestAttachmentAsync(accidentId);

        // Unauthorized user tries to update it
        var updateRequest = new UpdateAttachmentRequest { Description = "Hacked description" };
        using var updateContent = new MultipartFormDataContent();
        updateContent.Add(new StringContent(updateRequest.Description!), "Description");

        var response = await unauthClient.PatchAsync($"/api/v1/Attachments/{attachmentId}", updateContent);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Unauthorized_DeleteAttachment_WithoutAccidentsEdit()
    {
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var email = $"nodelatt{Guid.NewGuid():N}@test.com";
        var user = new User
        {
            UserName = email,
            Email = email,
            FullName = "No Delete User",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        await userManager.CreateAsync(user, "NoDel123!");

        var unauthHandler = new CookieHandler();
        var unauthClient = _factory.CreateDefaultClient(unauthHandler);
        await unauthClient.PostAsJsonAsync("/api/v1/Auth/login", new { Email = email, Password = "NoDel123!" });

        // Admin creates an attachment
        var accidentId = await CreateTestAccidentAsync();
        var attachmentId = await CreateTestAttachmentAsync(accidentId);

        // Unauthorized user tries to delete it
        var response = await unauthClient.DeleteAsync($"/api/v1/Attachments/{attachmentId}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
