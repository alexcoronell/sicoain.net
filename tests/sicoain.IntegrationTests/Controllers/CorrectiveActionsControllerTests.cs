using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using sicoain.IntegrationTests.Fixtures;
using sicoain.IntegrationTests.Utilities;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.CorrectiveActions;
using sicoain.shared.Entities;
using sicoain.shared.Enums;

namespace sicoain.IntegrationTests.Controllers;

public class CorrectiveActionsControllerTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly CookieHandler _cookieHandler;
    private readonly IntegrationTestWebAppFactory _factory;

    public CorrectiveActionsControllerTests(IntegrationTestWebAppFactory factory)
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
        var branch = new Branch { Name = $"TestBranch_{Guid.NewGuid():N}", BusinessId = business.Id, Business = business };
        context.Branches.Add(branch);
        await context.SaveChangesAsync();

        // Department
        var department = new Department { Name = $"TestDept_{Guid.NewGuid():N}" };
        context.Departments.Add(department);
        await context.SaveChangesAsync();

        // RiskClass (assume seeder exists; fallback)
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
            RiskClassId = riskClass.Id
        };
        context.Positions.Add(position);
        await context.SaveChangesAsync();

        // EPS and ARL (from seeder or create)
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
            Surname = "Integration",
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
            Description = "Test accident for corrective action",
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

    private CreateCorrectiveActionRequest CreateValidRequest(int accidentId)
    {
        return new CreateCorrectiveActionRequest
        {
            Title = $"TestAction_{Guid.NewGuid():N}",
            Description = "Descripción de la acción correctiva",
            DueDate = DateTime.Today.AddDays(7),
            Status = StatusAction.Proposal,
            Priority = Priority.Medium,
            AccidentId = accidentId
        };
    }

    private async Task<int> CreateTestCorrectiveActionAsync()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = CreateValidRequest(accidentId);
        var response = await _client.PostAsJsonAsync("/api/v1/CorrectiveActions", request);
        response.EnsureSuccessStatusCode();
        var action = await response.Content.ReadFromJsonAsync<CorrectiveActionDto>();
        return action!.Id;
    }

    #endregion

    // ========== CREATE ==========

    // 102. CreateCorrectiveAction_WithValidData_ReturnsCreated
    [Fact]
    public async Task CreateCorrectiveAction_WithValidData_ReturnsCreated()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = CreateValidRequest(accidentId);

        var response = await _client.PostAsJsonAsync("/api/v1/CorrectiveActions", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var action = await response.Content.ReadFromJsonAsync<CorrectiveActionDto>();
        action.Should().NotBeNull();
        action!.Title.Should().Be(request.Title);
        action.Description.Should().Be(request.Description);
        action.DueDate.Should().Be(request.DueDate);
        action.Status.Should().Be(request.Status);
        action.Priority.Should().Be(request.Priority);
        action.AccidentId.Should().Be(accidentId);
    }

    // 103. CreateCorrectiveAction_WithInvalidAccidentId_ThrowsDbUpdateException
    [Fact]
    public async Task CreateCorrectiveAction_WithInvalidAccidentId_ThrowsDbUpdateException()
    {
        var request = new CreateCorrectiveActionRequest
        {
            Title = "Invalid Action",
            Description = "FK violation",
            DueDate = DateTime.Today.AddDays(7),
            Status = StatusAction.Proposal,
            Priority = Priority.Low,
            AccidentId = 99999
        };
        Func<Task> act = () => _client.PostAsJsonAsync("/api/v1/CorrectiveActions", request);
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    // 104. CreateCorrectiveAction_WithInvalidPriority_ReturnsBadRequest
    [Fact]
    public async Task CreateCorrectiveAction_WithInvalidPriority_ReturnsBadRequest()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = CreateValidRequest(accidentId);
        var invalidRequest = new
        {
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            Status = "Proposal",
            Priority = "InvalidValue",
            AccidentId = accidentId
        };
        var response = await _client.PostAsJsonAsync("/api/v1/CorrectiveActions", invalidRequest);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 105. CreateCorrectiveAction_WithTitleTooShort_ReturnsBadRequest
    [Fact]
    public async Task CreateCorrectiveAction_WithTitleTooShort_ReturnsBadRequest()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = new CreateCorrectiveActionRequest
        {
            Title = "AB", // MinLength(3)
            Description = "Valid description for testing",
            DueDate = DateTime.Today.AddDays(7),
            Status = StatusAction.Proposal,
            Priority = Priority.Medium,
            AccidentId = accidentId
        };
        var response = await _client.PostAsJsonAsync("/api/v1/CorrectiveActions", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 106. CreateCorrectiveAction_WithDescriptionTooShort_ReturnsBadRequest
    [Fact]
    public async Task CreateCorrectiveAction_WithDescriptionTooShort_ReturnsBadRequest()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = new CreateCorrectiveActionRequest
        {
            Title = "Valid Title",
            Description = "AB", // MinLength(3)
            DueDate = DateTime.Today.AddDays(7),
            Status = StatusAction.Proposal,
            Priority = Priority.Medium,
            AccidentId = accidentId
        };
        var response = await _client.PostAsJsonAsync("/api/v1/CorrectiveActions", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 107. CreateCorrectiveAction_WithPastDueDate_ReturnsBadRequest
    [Fact]
    public async Task CreateCorrectiveAction_WithPastDueDate_ReturnsBadRequest()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = new CreateCorrectiveActionRequest
        {
            Title = "Valid Title",
            Description = "Valid description for testing",
            DueDate = DateTime.Today.AddDays(-1), // yesterday — GreaterThanOrEqualTo(DateTime.Today)
            Status = StatusAction.Proposal,
            Priority = Priority.Medium,
            AccidentId = accidentId
        };
        var response = await _client.PostAsJsonAsync("/api/v1/CorrectiveActions", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 108. CreateCorrectiveAction_WithInvalidAccidentIdZero_ReturnsBadRequest
    [Fact]
    public async Task CreateCorrectiveAction_WithInvalidAccidentIdZero_ReturnsBadRequest()
    {
        var request = new CreateCorrectiveActionRequest
        {
            Title = "Valid Title",
            Description = "Valid description for testing",
            DueDate = DateTime.Today.AddDays(7),
            Status = StatusAction.Proposal,
            Priority = Priority.Medium,
            AccidentId = 0 // GreaterThan(0) validation
        };
        var response = await _client.PostAsJsonAsync("/api/v1/CorrectiveActions", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ========== GET BY ID ==========

    // 109. GetCorrectiveActionById_WithExisting_ReturnsDto
    [Fact]
    public async Task GetCorrectiveActionById_WithExisting_ReturnsDto()
    {
        var correctiveActionId = await CreateTestCorrectiveActionAsync();

        var response = await _client.GetAsync($"/api/v1/CorrectiveActions/{correctiveActionId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var action = await response.Content.ReadFromJsonAsync<CorrectiveActionDto>();
        action.Should().NotBeNull();
        action!.Id.Should().Be(correctiveActionId);
        action.Title.Should().NotBeNullOrEmpty();
        action.AccidentId.Should().BeGreaterThan(0);
    }

    // 110. GetCorrectiveActionById_WithNonExisting_ReturnsNotFound
    [Fact]
    public async Task GetCorrectiveActionById_WithNonExisting_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v1/CorrectiveActions/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ========== GET ALL ==========

    // 111. GetAllCorrectiveActions_ReturnsPaginatedList
    [Fact]
    public async Task GetAllCorrectiveActions_ReturnsPaginatedList()
    {
        await CreateTestCorrectiveActionAsync();

        var response = await _client.GetAsync("/api/v1/CorrectiveActions?pageNumber=1&pageSize=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<CorrectiveActionDto>>();
        pagedResponse.Should().NotBeNull();
        pagedResponse!.Items.Should().NotBeEmpty();
        pagedResponse.PageNumber.Should().Be(1);
        pagedResponse.PageSize.Should().Be(5);
        pagedResponse.TotalCount.Should().BeGreaterThan(0);
    }

    // 112. GetAllCorrectiveActions_WithPageBeyondData_ReturnsEmptyItems
    [Fact]
    public async Task GetAllCorrectiveActions_WithPageBeyondData_ReturnsEmptyItems()
    {
        var response = await _client.GetAsync("/api/v1/CorrectiveActions?pageNumber=9999&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<CorrectiveActionDto>>();
        pagedResponse.Should().NotBeNull();
        pagedResponse!.Items.Should().BeEmpty();
        pagedResponse.PageNumber.Should().Be(9999);
        pagedResponse.PageSize.Should().Be(10);
    }

    // ========== UPDATE (PATCH) ==========

    // 113. UpdateCorrectiveAction_ChangeStatus_Updates
    [Fact]
    public async Task UpdateCorrectiveAction_ChangeStatus_Updates()
    {
        var correctiveActionId = await CreateTestCorrectiveActionAsync();

        var updateRequest = new UpdateCorrectiveActionRequest
        {
            Status = StatusAction.Approved,
            Priority = Priority.High
        };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/v1/CorrectiveActions/{correctiveActionId}", updateRequest);
        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<CorrectiveActionDto>();
        updated!.Status.Should().Be(StatusAction.Approved);
        updated.Priority.Should().Be(Priority.High);
    }

    // 114. UpdateCorrectiveAction_WithNonExisting_ReturnsNotFound
    [Fact]
    public async Task UpdateCorrectiveAction_WithNonExisting_ReturnsNotFound()
    {
        var updateRequest = new UpdateCorrectiveActionRequest { Title = "Non Existing" };
        var response = await _client.PatchAsJsonAsync("/api/v1/CorrectiveActions/99999", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // 115. UpdateCorrectiveAction_PartialUpdateOnlyTitle_OtherFieldsUnchanged
    [Fact]
    public async Task UpdateCorrectiveAction_PartialUpdateOnlyTitle_OtherFieldsUnchanged()
    {
        var accidentId = await CreateTestAccidentAsync();
        var createRequest = CreateValidRequest(accidentId);
        var createResponse = await _client.PostAsJsonAsync("/api/v1/CorrectiveActions", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<CorrectiveActionDto>();

        // Update only the Title, leave everything else null (no change)
        var updateRequest = new UpdateCorrectiveActionRequest { Title = "Partially Updated Title" };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/v1/CorrectiveActions/{created!.Id}", updateRequest);

        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<CorrectiveActionDto>();
        updated!.Title.Should().Be("Partially Updated Title");
        updated.Description.Should().Be(createRequest.Description);
        updated.DueDate.Should().Be(createRequest.DueDate);
        updated.Status.Should().Be(createRequest.Status);
        updated.Priority.Should().Be(createRequest.Priority);
        updated.AccidentId.Should().Be(createRequest.AccidentId);
    }

    // 116. UpdateCorrectiveAction_PartialUpdateOnlyStatus_OtherFieldsUnchanged
    [Fact]
    public async Task UpdateCorrectiveAction_PartialUpdateOnlyStatus_OtherFieldsUnchanged()
    {
        var accidentId = await CreateTestAccidentAsync();
        var createRequest = CreateValidRequest(accidentId);
        var createResponse = await _client.PostAsJsonAsync("/api/v1/CorrectiveActions", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<CorrectiveActionDto>();

        // Update only Status and Priority, leave everything else null (no change)
        var updateRequest = new UpdateCorrectiveActionRequest
        {
            Status = StatusAction.Approved,
            Priority = Priority.Critical
        };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/v1/CorrectiveActions/{created!.Id}", updateRequest);

        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<CorrectiveActionDto>();
        updated!.Status.Should().Be(StatusAction.Approved);
        updated.Priority.Should().Be(Priority.Critical);
        updated.Title.Should().Be(createRequest.Title);
        updated.Description.Should().Be(createRequest.Description);
        updated.DueDate.Should().Be(createRequest.DueDate);
        updated.AccidentId.Should().Be(createRequest.AccidentId);
    }

    // ========== DELETE ==========

    // 117. DeleteCorrectiveAction_SoftDeletes
    [Fact]
    public async Task DeleteCorrectiveAction_SoftDeletes()
    {
        var correctiveActionId = await CreateTestCorrectiveActionAsync();

        var deleteResponse = await _client.DeleteAsync($"/api/v1/CorrectiveActions/{correctiveActionId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/v1/CorrectiveActions/{correctiveActionId}");
        // Patrón consistente: GetById devuelve OK incluso después de soft delete
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // 118. DeleteCorrectiveAction_WithNonExisting_ReturnsNotFound
    [Fact]
    public async Task DeleteCorrectiveAction_WithNonExisting_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/v1/CorrectiveActions/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ========== AUTHORIZATION ==========

    // 119. Unauthorized_DeleteCorrectiveAction_WithoutPermission
    [Fact]
    public async Task Unauthorized_DeleteCorrectiveAction_WithoutPermission()
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

        var correctiveActionId = await CreateTestCorrectiveActionAsync();
        var deleteResponse = await unauthClient.DeleteAsync($"/api/v1/CorrectiveActions/{correctiveActionId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // 120. Unauthorized_ViewCorrectiveAction_WithoutSettingsView
    [Fact]
    public async Task Unauthorized_ViewCorrectiveAction_WithoutSettingsView()
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

        var actionId = await CreateTestCorrectiveActionAsync();
        var response = await unauthClient.GetAsync($"/api/v1/CorrectiveActions/{actionId}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // 121. Unauthorized_CreateCorrectiveAction_WithoutSettingsEdit
    [Fact]
    public async Task Unauthorized_CreateCorrectiveAction_WithoutPermission()
    {
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var email = $"nocreate{Guid.NewGuid():N}@test.com";
        var user = new User
        {
            UserName = email,
            Email = email,
            FullName = "No Create User",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        await userManager.CreateAsync(user, "NoCreate123!");

        var unauthHandler = new CookieHandler();
        var unauthClient = _factory.CreateDefaultClient(unauthHandler);
        await unauthClient.PostAsJsonAsync("/api/v1/Auth/login", new { Email = email, Password = "NoCreate123!" });

        var accidentId = await CreateTestAccidentAsync();
        var request = CreateValidRequest(accidentId);
        var response = await unauthClient.PostAsJsonAsync("/api/v1/CorrectiveActions", request);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
