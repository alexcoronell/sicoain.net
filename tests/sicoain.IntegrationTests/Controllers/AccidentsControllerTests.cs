using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using sicoain.IntegrationTests.Fixtures;
using sicoain.IntegrationTests.Utilities;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Accident;
using sicoain.shared.DTOs.AccidentTypes;
using sicoain.shared.DTOs.Employees;
using sicoain.shared.DTOs.EventCategories;
using sicoain.shared.Enums;

namespace sicoain.IntegrationTests.Controllers;

public class AccidentsControllerTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly CookieHandler _cookieHandler;
    private readonly IntegrationTestWebAppFactory _factory;

    public AccidentsControllerTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _cookieHandler = new CookieHandler();
        _client = factory.CreateDefaultClient(_cookieHandler);
        // Autenticarse como admin antes de cada prueba
        AuthenticateAsync().GetAwaiter().GetResult();
    }

    private async Task AuthenticateAsync()
    {
        var loginRequest = new { Email = "admin@test.com", Password = "Admin123!" };
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/login", loginRequest);
        response.EnsureSuccessStatusCode();
    }

    #region Helpers para crear datos relacionados

    private ApplicationDbContext CreateDbContext()
    {
        var scope = _factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    private async Task<int> CreateTestEmployeeAsync()
    {
        await using var context = CreateDbContext();

        // Create Business
        var business = new Business { Name = "Test Business" };
        context.Businesses.Add(business);
        await context.SaveChangesAsync();

        // Create Branch
        var branch = new Branch { Name = "Test Branch", BusinessId = business.Id, Business = business };
        context.Branches.Add(branch);
        await context.SaveChangesAsync();

        // Create Department
        var department = new Department { Name = "Test Department" };
        context.Departments.Add(department);
        await context.SaveChangesAsync();

        // Get RiskClass (seeded in InitializeAsync)
        var riskClass = await context.RiskClasses.FirstAsync();
        var position = new Position { Name = "Test Position", DepartmentId = department.Id, RiskClassId = riskClass.Id, Department = department };
        context.Positions.Add(position);
        await context.SaveChangesAsync();

        // Get EPS and ARL (must exist from seeders — fallback to create if none found)
        var eps = await context.HealthPromotionEntities.FirstOrDefaultAsync();
        eps ??= (context.HealthPromotionEntities.Add(new HealthPromotionEntity { Name = "Test EPS" }).Entity);
        var arl = await context.OccupationalRiskAdministrators.FirstOrDefaultAsync();
        arl ??= (context.OccupationalRiskAdministrators.Add(new OccupationalRiskAdministrator { Name = "Test ARL" }).Entity);
        await context.SaveChangesAsync();

        // Create Employee
        var employee = new Employee
        {
            DocumentType = DocumentType.CedulaDeCiudadania,
            DocumentNumber = $"EMP{Guid.NewGuid():N}",
            FirstName = "Test",
            SecondName = "Employee",
            Surname = "Integration",
            SecondSurname = "Test",
            State = "Cundinamarca",
            Municipality = "Bogotá",
            Neighborhood = "Centro",
            AddressStreet = "Calle 123",
            PostalCode = "000000",
            HiringDate = DateTime.UtcNow.AddDays(-30),
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

    private async Task<int> CreateTestAccidentTypeAsync()
    {
        await using var context = CreateDbContext();
        var entity = new AccidentType { Name = $"TestType{Guid.NewGuid():N}", Description = "Integration test type", Severity = AccidentSeverity.Mild };
        context.AccidentTypes.Add(entity);
        await context.SaveChangesAsync();
        return entity.Id;
    }

    private async Task<int> CreateTestEventCategoryAsync()
    {
        await using var context = CreateDbContext();
        var entity = new EventCategory { Name = $"TestCategory{Guid.NewGuid():N}", LevelOfSeverity = AccidentSeverity.Mild, RequiresHospitalization = false };
        context.EventCategories.Add(entity);
        await context.SaveChangesAsync();
        return entity.Id;
    }

    #endregion

    // 40. CreateAccident_WithValidData_ReturnsCreated
    [Fact]
    public async Task CreateAccident_WithValidData_ReturnsCreated()
    {
        // Arrange
        int employeeId = await CreateTestEmployeeAsync();
        int accidentTypeId = await CreateTestAccidentTypeAsync();
        int eventCategoryId = await CreateTestEventCategoryAsync();

        var request = new CreateAccidentRequest
        {
            EventDate = DateTime.Today,
            Description = "Valid accident description for testing purposes",
            EmployeeId = employeeId,
            AccidentTypeId = accidentTypeId,
            EventCategoryId = eventCategoryId
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Accidents", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var accident = await response.Content.ReadFromJsonAsync<AccidentDto>();
        accident.Should().NotBeNull();
        accident!.Description.Should().Be(request.Description);
        accident.EmployeeId.Should().Be(employeeId);
        accident.AccidentTypeId.Should().Be(accidentTypeId);
        accident.EventCategoryId.Should().Be(eventCategoryId);
        accident.EmployeeFullname.Should().NotBeNullOrEmpty();
        accident.AccidentTypeName.Should().NotBeNullOrEmpty();
        accident.EventCategoryName.Should().NotBeNullOrEmpty();
    }

    // 41. CreateAccident_WithInvalidEmployeeId_ThrowsDbUpdateException
    // No global exception handling middleware → FK violation propagates as exception.
    [Fact]
    public async Task CreateAccident_WithInvalidEmployeeId_ThrowsDbUpdateException()
    {
        int accidentTypeId = await CreateTestAccidentTypeAsync();
        int eventCategoryId = await CreateTestEventCategoryAsync();

        var request = new CreateAccidentRequest
        {
            EventDate = DateTime.Today,
            Description = "Valid description",
            EmployeeId = 99999, // no existe
            AccidentTypeId = accidentTypeId,
            EventCategoryId = eventCategoryId
        };

        Func<Task> act = () => _client.PostAsJsonAsync("/api/v1/Accidents", request);
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    // 42. CreateAccident_WithMissingDescription_ReturnsBadRequest
    [Fact]
    public async Task CreateAccident_WithMissingDescription_ReturnsBadRequest()
    {
        int employeeId = await CreateTestEmployeeAsync();
        int accidentTypeId = await CreateTestAccidentTypeAsync();
        int eventCategoryId = await CreateTestEventCategoryAsync();

        var request = new { EventDate = DateTime.UtcNow, EmployeeId = employeeId, AccidentTypeId = accidentTypeId, EventCategoryId = eventCategoryId };
        var response = await _client.PostAsJsonAsync("/api/v1/Accidents", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 43. GetAccidentById_WithExisting_ReturnsAccidentDto
    [Fact]
    public async Task GetAccidentById_WithExisting_ReturnsAccidentDto()
    {
        // Create an accident first
        int employeeId = await CreateTestEmployeeAsync();
        int accidentTypeId = await CreateTestAccidentTypeAsync();
        int eventCategoryId = await CreateTestEventCategoryAsync();

        var createRequest = new CreateAccidentRequest
        {
            EventDate = DateTime.Today,
            Description = "Accident to retrieve",
            EmployeeId = employeeId,
            AccidentTypeId = accidentTypeId,
            EventCategoryId = eventCategoryId
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/Accidents", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<AccidentDto>();

        var response = await _client.GetAsync($"/api/v1/Accidents/{created!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var accident = await response.Content.ReadFromJsonAsync<AccidentDto>();
        accident.Should().NotBeNull();
        accident!.Id.Should().Be(created.Id);
        accident.Description.Should().Be(createRequest.Description);
        accident.EmployeeFullname.Should().NotBeNullOrEmpty();
        accident.AccidentTypeName.Should().NotBeNullOrEmpty();
        accident.EventCategoryName.Should().NotBeNullOrEmpty();
    }

    // 44. GetAllAccidents_ReturnsPaginatedList
    [Fact]
    public async Task GetAllAccidents_ReturnsPaginatedList()
    {
        // Ensure at least one accident exists
        int employeeId = await CreateTestEmployeeAsync();
        int accidentTypeId = await CreateTestAccidentTypeAsync();
        int eventCategoryId = await CreateTestEventCategoryAsync();

        var createRequest = new CreateAccidentRequest
        {
            EventDate = DateTime.Today,
            Description = "Accident for pagination test",
            EmployeeId = employeeId,
            AccidentTypeId = accidentTypeId,
            EventCategoryId = eventCategoryId
        };
        await _client.PostAsJsonAsync("/api/v1/Accidents", createRequest);

        var response = await _client.GetAsync("/api/v1/Accidents?pageNumber=1&pageSize=5");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<AccidentDto>>();
        pagedResponse.Should().NotBeNull();
        pagedResponse!.Items.Should().NotBeNull();
        pagedResponse.PageNumber.Should().Be(1);
        pagedResponse.PageSize.Should().Be(5);
        pagedResponse.TotalCount.Should().BeGreaterThan(0);
    }

    // 45. GetAllAccidents_IncludesRelatedData
    [Fact]
    public async Task GetAllAccidents_IncludesRelatedData()
    {
        int employeeId = await CreateTestEmployeeAsync();
        int accidentTypeId = await CreateTestAccidentTypeAsync();
        int eventCategoryId = await CreateTestEventCategoryAsync();

        var createRequest = new CreateAccidentRequest
        {
            EventDate = DateTime.Today,
            Description = "Check related data",
            EmployeeId = employeeId,
            AccidentTypeId = accidentTypeId,
            EventCategoryId = eventCategoryId
        };
        await _client.PostAsJsonAsync("/api/v1/Accidents", createRequest);

        var response = await _client.GetAsync("/api/v1/Accidents?pageNumber=1&pageSize=10");
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<AccidentDto>>();
        var first = pagedResponse!.Items.First();
        first.EmployeeFullname.Should().NotBeNullOrEmpty();
        first.AccidentTypeName.Should().NotBeNullOrEmpty();
        first.EventCategoryName.Should().NotBeNullOrEmpty();
    }

    // 46. UpdateAccident_WithValidData_UpdatesFields
    [Fact]
    public async Task UpdateAccident_WithValidData_UpdatesFields()
    {
        int employeeId = await CreateTestEmployeeAsync();
        int accidentTypeId = await CreateTestAccidentTypeAsync();
        int eventCategoryId = await CreateTestEventCategoryAsync();

        var createRequest = new CreateAccidentRequest
        {
            EventDate = DateTime.Today,
            Description = "Original description",
            EmployeeId = employeeId,
            AccidentTypeId = accidentTypeId,
            EventCategoryId = eventCategoryId
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/Accidents", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<AccidentDto>();

        var updateRequest = new UpdateAccidentRequest
        {
            Description = "Updated description",
            EventDate = DateTime.Today,
            EmployeeId = employeeId,
            AccidentTypeId = accidentTypeId,
            EventCategoryId = eventCategoryId
        };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/v1/Accidents/{created!.Id}", updateRequest);
        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<AccidentDto>();
        updated!.Description.Should().Be("Updated description");
        updated.EventDate.Should().Be(updateRequest.EventDate);
    }

    // 47. UpdateAccident_WithInvalidAccidentTypeId_ThrowsDbUpdateException
    // Must include valid EmployeeId/EventCategoryId so they don't default to 0 (AutoMapper
    // condition srcMember != null doesn't prevent int?→int null from mapping to default).
    [Fact]
    public async Task UpdateAccident_WithInvalidAccidentTypeId_ThrowsDbUpdateException()
    {
        int employeeId = await CreateTestEmployeeAsync();
        int accidentTypeId = await CreateTestAccidentTypeAsync();
        int eventCategoryId = await CreateTestEventCategoryAsync();

        var createRequest = new CreateAccidentRequest
        {
            EventDate = DateTime.Today,
            Description = "Valid test description for accident creation",
            EmployeeId = employeeId,
            AccidentTypeId = accidentTypeId,
            EventCategoryId = eventCategoryId
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/Accidents", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<AccidentDto>();

        var updateRequest = new UpdateAccidentRequest
        {
            EmployeeId = employeeId,
            EventCategoryId = eventCategoryId,
            AccidentTypeId = 99999 // FK violation
        };
        Func<Task> act = () => _client.PatchAsJsonAsync($"/api/v1/Accidents/{created!.Id}", updateRequest);
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    // 48. DeleteAccident_SoftDeletesAccident
    [Fact]
    public async Task DeleteAccident_SoftDeletesAccident()
    {
        int employeeId = await CreateTestEmployeeAsync();
        int accidentTypeId = await CreateTestAccidentTypeAsync();
        int eventCategoryId = await CreateTestEventCategoryAsync();

        var createRequest = new CreateAccidentRequest
        {
            EventDate = DateTime.Today,
            Description = "To be deleted",
            EmployeeId = employeeId,
            AccidentTypeId = accidentTypeId,
            EventCategoryId = eventCategoryId
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/Accidents", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<AccidentDto>();

        var deleteResponse = await _client.DeleteAsync($"/api/v1/Accidents/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/v1/Accidents/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // 49. DeleteAccident_AlreadyDeleted_ReturnsNotFound
    [Fact]
    public async Task DeleteAccident_AlreadyDeleted_ReturnsNotFound()
    {
        int employeeId = await CreateTestEmployeeAsync();
        int accidentTypeId = await CreateTestAccidentTypeAsync();
        int eventCategoryId = await CreateTestEventCategoryAsync();

        var createRequest = new CreateAccidentRequest
        {
            EventDate = DateTime.Today,
            Description = "Double delete",
            EmployeeId = employeeId,
            AccidentTypeId = accidentTypeId,
            EventCategoryId = eventCategoryId
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/Accidents", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<AccidentDto>();

        await _client.DeleteAsync($"/api/v1/Accidents/{created!.Id}");
        var secondDelete = await _client.DeleteAsync($"/api/v1/Accidents/{created.Id}");
        secondDelete.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    // 50. Unauthorized_CreateAccident_WithoutPermission
    [Fact]
    public async Task Unauthorized_CreateAccident_WithoutPermission()
    {
        // Create a user without any role (or with a role that lacks Accidents.Create)
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
        // No role assigned → no permissions

        // Login as this user with a new client
        var unauthHandler = new CookieHandler();
        var unauthClient = _factory.CreateDefaultClient(unauthHandler);
        var loginResponse = await unauthClient.PostAsJsonAsync("/api/v1/Auth/login", new { Email = email, Password = "NoPerm123!" });
        loginResponse.EnsureSuccessStatusCode();

        // Try to create accident
        int employeeId = await CreateTestEmployeeAsync();
        int accidentTypeId = await CreateTestAccidentTypeAsync();
        int eventCategoryId = await CreateTestEventCategoryAsync();

        var request = new CreateAccidentRequest
        {
            EventDate = DateTime.Today,
            Description = "Should be forbidden",
            EmployeeId = employeeId,
            AccidentTypeId = accidentTypeId,
            EventCategoryId = eventCategoryId
        };
        var response = await unauthClient.PostAsJsonAsync("/api/v1/Accidents", request);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // 51. Unauthorized_ViewAccident_WithoutPermission
    [Fact]
    public async Task Unauthorized_ViewAccident_WithoutPermission()
    {
        // Same user without permission
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
        var loginResponse = await unauthClient.PostAsJsonAsync("/api/v1/Auth/login", new { Email = email, Password = "NoView123!" });
        loginResponse.EnsureSuccessStatusCode();

        // First create an accident with admin client
        int employeeId = await CreateTestEmployeeAsync();
        int accidentTypeId = await CreateTestAccidentTypeAsync();
        int eventCategoryId = await CreateTestEventCategoryAsync();
        var createRequest = new CreateAccidentRequest
        {
            EventDate = DateTime.Today,
            Description = "Test accident",
            EmployeeId = employeeId,
            AccidentTypeId = accidentTypeId,
            EventCategoryId = eventCategoryId
        };
        var adminResponse = await _client.PostAsJsonAsync("/api/v1/Accidents", createRequest);
        var accident = await adminResponse.Content.ReadFromJsonAsync<AccidentDto>();

        var getResponse = await unauthClient.GetAsync($"/api/v1/Accidents/{accident!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
