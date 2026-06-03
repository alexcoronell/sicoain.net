using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using sicoain.IntegrationTests.Fixtures;
using sicoain.IntegrationTests.Utilities;
using sicoain.shared.DTOs;
using sicoain.shared.DTOs.Witnesses;
using sicoain.shared.Entities;
using sicoain.shared.Enums;

namespace sicoain.IntegrationTests.Controllers;

public class WitnessesControllerTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly CookieHandler _cookieHandler;
    private readonly IntegrationTestWebAppFactory _factory;

    public WitnessesControllerTests(IntegrationTestWebAppFactory factory)
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
            RiskClassId = riskClass.Id
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
            Surname = "Witness",
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
            Description = "Accident for witness test",
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
            RiskClassId = riskClass.Id
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
            FirstName = "Witness",
            SecondName = "Employee",
            Surname = "ForTest",
            SecondSurname = "One",
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

    private CreateWitnessRequest CreateValidWitnessRequestWithEmployeeId(int accidentId, int employeeId)
    {
        return new CreateWitnessRequest
        {
            AccidentId = accidentId,
            EmployeeId = employeeId,
            Statement = new string('A', 60) // mínimo 50 caracteres
        };
    }

    private CreateWitnessRequest CreateValidWitnessRequestWithExternalName(int accidentId)
    {
        return new CreateWitnessRequest
        {
            AccidentId = accidentId,
            WitnessName = "Juan Pérez Externo",
            WitnessContact = "juan@example.com",
            Statement = new string('B', 60)
        };
    }

    /// <summary>
    /// WitnessDto does not have an Id property, so after creating via API
    /// we query the DB to get the created witness ID.
    /// </summary>
    private async Task<int> CreateTestWitnessAsync()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = CreateValidWitnessRequestWithExternalName(accidentId);
        var response = await _client.PostAsJsonAsync("/api/v1/Witnesses", request);
        response.EnsureSuccessStatusCode();

        await using var context = CreateDbContext();
        var witness = await context.Witnesses
            .OrderByDescending(w => w.Id)
            .FirstAsync();
        return witness.Id;
    }

    #endregion

    // ========== CREATE ==========

    // 122. CreateWitness_WithEmployeeId_Valid
    [Fact]
    public async Task CreateWitness_WithEmployeeId_Valid()
    {
        var accidentId = await CreateTestAccidentAsync();
        var employeeId = await CreateTestEmployeeAsync();
        var request = CreateValidWitnessRequestWithEmployeeId(accidentId, employeeId);

        var response = await _client.PostAsJsonAsync("/api/v1/Witnesses", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var witness = await response.Content.ReadFromJsonAsync<WitnessDto>();
        witness.Should().NotBeNull();
        witness!.AccidentId.Should().Be(accidentId);
        witness.EmployeeId.Should().Be(employeeId);
        witness.Statement.Should().Be(request.Statement);
        witness.EmployeeFullname.Should().NotBeNullOrEmpty();
    }

    // 123. CreateWitness_WithExternalName_Valid
    [Fact]
    public async Task CreateWitness_WithExternalName_Valid()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = CreateValidWitnessRequestWithExternalName(accidentId);

        var response = await _client.PostAsJsonAsync("/api/v1/Witnesses", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var witness = await response.Content.ReadFromJsonAsync<WitnessDto>();
        witness.Should().NotBeNull();
        witness!.AccidentId.Should().Be(accidentId);
        witness.WitnessName.Should().Be(request.WitnessName);
        witness.WitnessContact.Should().Be(request.WitnessContact);
        witness.Statement.Should().Be(request.Statement);
        witness.EmployeeId.Should().Be(0); // 0 porque no tiene EmployeeId
    }

    // 124. CreateWitness_WithoutBothEmployeeIdAndName_ReturnsBadRequest
    [Fact]
    public async Task CreateWitness_WithoutBothEmployeeIdAndName_ReturnsBadRequest()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = new CreateWitnessRequest
        {
            AccidentId = accidentId,
            Statement = "Declaración del testigo sin identidad que cumple con los 50 caracteres mínimo"
        };
        var response = await _client.PostAsJsonAsync("/api/v1/Witnesses", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // 125. CreateWitness_WithInvalidAccidentId_ThrowsDbUpdateException
    [Fact]
    public async Task CreateWitness_WithInvalidAccidentId_ThrowsDbUpdateException()
    {
        var request = new CreateWitnessRequest
        {
            AccidentId = 99999,
            WitnessName = "Testigo Externo",
            Statement = new string('C', 60)
        };
        Func<Task> act = () => _client.PostAsJsonAsync("/api/v1/Witnesses", request);
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    // 126. CreateWitness_WithStatementTooShort_ReturnsBadRequest
    [Fact]
    public async Task CreateWitness_WithStatementTooShort_ReturnsBadRequest()
    {
        var accidentId = await CreateTestAccidentAsync();
        var request = new CreateWitnessRequest
        {
            AccidentId = accidentId,
            WitnessName = "Testigo Externo",
            Statement = "Too short" // menos de 50 caracteres
        };
        var response = await _client.PostAsJsonAsync("/api/v1/Witnesses", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ========== GET BY ID ==========

    // 127. GetWitnessById_WithEmployee_IncludesEmployeeFullname
    [Fact]
    public async Task GetWitnessById_WithEmployee_IncludesEmployeeFullname()
    {
        var accidentId = await CreateTestAccidentAsync();
        var employeeId = await CreateTestEmployeeAsync();
        var request = CreateValidWitnessRequestWithEmployeeId(accidentId, employeeId);
        var createResponse = await _client.PostAsJsonAsync("/api/v1/Witnesses", request);

        // WitnessDto doesn't have Id, so query the DB for the created witness ID
        await using var context = CreateDbContext();
        var created = await context.Witnesses
            .OrderByDescending(w => w.Id)
            .FirstAsync();

        var response = await _client.GetAsync($"/api/v1/Witnesses/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var witness = await response.Content.ReadFromJsonAsync<WitnessDto>();
        witness.Should().NotBeNull();
        witness!.EmployeeFullname.Should().NotBeNullOrEmpty();
    }

    // 128. GetWitnessById_WithNonExisting_ReturnsNotFound
    [Fact]
    public async Task GetWitnessById_WithNonExisting_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v1/Witnesses/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ========== GET ALL ==========

    // 129. GetAllWitnesses_ReturnsPaginatedList
    [Fact]
    public async Task GetAllWitnesses_ReturnsPaginatedList()
    {
        await CreateTestWitnessAsync();

        var response = await _client.GetAsync("/api/v1/Witnesses?pageNumber=1&pageSize=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<WitnessDto>>();
        pagedResponse.Should().NotBeNull();
        pagedResponse!.Items.Should().NotBeEmpty();
        pagedResponse.PageNumber.Should().Be(1);
        pagedResponse.PageSize.Should().Be(5);
        pagedResponse.TotalCount.Should().BeGreaterThan(0);
    }

    // 130. GetAllWitnesses_WithPageBeyondData_ReturnsEmptyItems
    [Fact]
    public async Task GetAllWitnesses_WithPageBeyondData_ReturnsEmptyItems()
    {
        var response = await _client.GetAsync("/api/v1/Witnesses?pageNumber=9999&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<WitnessDto>>();
        pagedResponse.Should().NotBeNull();
        pagedResponse!.Items.Should().BeEmpty();
        pagedResponse.PageNumber.Should().Be(9999);
        pagedResponse.PageSize.Should().Be(10);
    }

    // ========== UPDATE (PATCH) ==========

    // 131. UpdateWitness_ChangeStatement_Updates
    [Fact]
    public async Task UpdateWitness_ChangeStatement_Updates()
    {
        var witnessId = await CreateTestWitnessAsync();

        var updateRequest = new UpdateWitnessRequest
        {
            Statement = "Declaración actualizada con nuevos detalles que cumple con los 50 caracteres mínimos"
        };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/v1/Witnesses/{witnessId}", updateRequest);
        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await patchResponse.Content.ReadFromJsonAsync<WitnessDto>();
        updated!.Statement.Should().Be("Declaración actualizada con nuevos detalles que cumple con los 50 caracteres mínimos");
    }

    // 132. UpdateWitness_WithNonExisting_ReturnsNotFound
    [Fact]
    public async Task UpdateWitness_WithNonExisting_ReturnsNotFound()
    {
        var updateRequest = new UpdateWitnessRequest { Statement = "Declaración para test de witness no existente que cumple 50 caracteres" };
        var response = await _client.PatchAsJsonAsync("/api/v1/Witnesses/99999", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ========== DELETE ==========

    // 133. DeleteWitness_SoftDeletes
    [Fact]
    public async Task DeleteWitness_SoftDeletes()
    {
        var witnessId = await CreateTestWitnessAsync();

        var deleteResponse = await _client.DeleteAsync($"/api/v1/Witnesses/{witnessId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Soft delete: GET still returns the entity
        var getResponse = await _client.GetAsync($"/api/v1/Witnesses/{witnessId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify soft delete in database
        await using var verifyCtx = CreateDbContext();
        var witnessInDb = await verifyCtx.Witnesses.FindAsync(witnessId);
        witnessInDb.Should().NotBeNull();
        witnessInDb!.IsDeleted.Should().BeTrue();
    }

    // 134. DeleteWitness_WithNonExisting_ReturnsNotFound
    [Fact]
    public async Task DeleteWitness_WithNonExisting_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/v1/Witnesses/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ========== AUTHORIZATION ==========

    // 135. Unauthorized_CreateWitness_WithoutAccidentsCreate
    [Fact]
    public async Task Unauthorized_CreateWitness_WithoutPermission()
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
        var request = CreateValidWitnessRequestWithExternalName(accidentId);
        var response = await unauthClient.PostAsJsonAsync("/api/v1/Witnesses", request);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // 136. Unauthorized_ViewWitness_WithoutAccidentsView
    [Fact]
    public async Task Unauthorized_ViewWitness_WithoutPermission()
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

        var witnessId = await CreateTestWitnessAsync();
        var response = await unauthClient.GetAsync($"/api/v1/Witnesses/{witnessId}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // 137. Unauthorized_UpdateWitness_WithoutAccidentsEdit
    [Fact]
    public async Task Unauthorized_UpdateWitness_WithoutPermission()
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

        var witnessId = await CreateTestWitnessAsync();
        var updateRequest = new UpdateWitnessRequest { Statement = "Declaración actualizada por usuario sin permisos que cumple 50 chars" };
        var response = await unauthClient.PatchAsJsonAsync($"/api/v1/Witnesses/{witnessId}", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
