# sicoain.UnitTests

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![xUnit](https://img.shields.io/badge/xUnit-2.9.3-1E8E3E?logo=celery)](https://xunit.net/)
[![FluentAssertions](https://img.shields.io/badge/FluentAssertions-8.10.0-512BD4)](https://fluentassertions.com/)
[![Moq](https://img.shields.io/badge/Moq-4.20.72-512BD4)](https://github.com/devlooped/moq)
[![EF Core](https://img.shields.io/badge/EF_Core_InMemory-10.0.8-512BD4?logo=entity)](https://docs.microsoft.com/ef/core/)
[![Coverlet](https://img.shields.io/badge/coverlet-6.0.4-512BD4)](https://github.com/coverlet-coverage/coverlet)
[![License](https://img.shields.io/badge/license-MIT-blue)]()

Comprehensive unit test suite for the **sicoain.net** platform — a workplace accident management system built on ASP.NET Core Identity with FluentValidation.

> **766 tests** across **78 test files**, covering every service layer and input validator in the application.

---

## Table of Contents

- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Test Breakdown](#test-breakdown)
  - [Service Tests (120)](#service-tests-120)
  - [Validator Tests (645)](#validator-tests-645)
- [Testing Patterns](#testing-patterns)
  - [Service Layer](#service-layer)
  - [Validator Layer](#validator-layer)
  - [File Uploads & Race Conditions](#file-uploads--race-conditions)
  - [Identity with Non-Default Key Types](#identity-with-non-default-key-types)
- [Running Tests](#running-tests)
- [Coverage](#coverage)

---

## Tech Stack

| Technology | Version | Purpose |
|---|---|---|
| **.NET** | `10.0` | Target framework |
| **xUnit** | `2.9.3` | Test framework |
| **xUnit.Runner.VisualStudio** | `3.1.4` | Visual Studio test integration |
| **FluentAssertions** | `8.10.0` | Expressive, readable assertions |
| **Moq** | `4.20.72` | Mock-based dependency isolation |
| **EF Core InMemory** | `10.0.8` | In-memory database provider for integration-style service tests |
| **coverlet.collector** | `6.0.4` | Code coverage data collector |

The test project references the following source projects:
- **`sicoain.api`** — ASP.NET Core API project (controllers, services, validators, data layer)
- **`sicoain.shared`** — Shared domain layer (entities, DTOs, enums)

---

## Project Structure

```
tests/sicoain.UnitTests/
├── README.md                          # This file
├── sicoain.UnitTests.csproj           # Project file
├── UnitTest1.cs                       # Scaffold placeholder (to be removed)
│
├── Services/                          # Service layer tests (23 files)
│   ├── AccidentServiceTests.cs        #   3 tests
│   ├── AccidentTypeServiceTests.cs    #   6 tests
│   ├── AttachmentServiceTests.cs      #   9 tests
│   ├── AuthServiceTests.cs            #  18 tests
│   ├── BranchServiceTests.cs          #   3 tests
│   ├── BusinessServiceTests.cs        #   3 tests
│   ├── CookieManagerTests.cs          #   8 tests
│   ├── CorrectiveActionServiceTests.cs#   3 tests
│   ├── DepartmentServiceTests.cs      #   3 tests
│   ├── DigitalEvidenceServiceTests.cs #   6 tests
│   ├── EmployeeServiceTests.cs        #   3 tests
│   ├── EventCategoryServiceTests.cs   #   3 tests
│   ├── HealthPromotionEntityServiceTests.cs # 3 tests
│   ├── IpAddressProviderTests.cs      #   4 tests
│   ├── JwtTokenGeneratorTests.cs      #   5 tests
│   ├── OccupationalRiskAdministratorServiceTests.cs # 3 tests
│   ├── PermissionServiceTests.cs      #   2 tests
│   ├── PositionServiceTests.cs        #   3 tests
│   ├── RefreshTokenGeneratorTests.cs  #   3 tests
│   ├── RiskClassServiceTests.cs       #   3 tests
│   ├── RoleSyncServiceTests.cs        #   4 tests
│   ├── UserServiceTests.cs            #  19 tests
│   └── WitnessServiceTests.cs         #   3 tests
│
└── Validators/                        # FluentValidation tests (55 files)
    ├── LoginRequestValidatorTests.cs  #   8 tests
    │
    ├── Accidents/
    │   ├── CreateAccidentRequestValidatorTests.cs      # 12 tests
    │   └── UpdateAccidentRequestValidatorTests.cs      # 14 tests
    │
    ├── AccidentTypes/
    │   ├── CreateAccidentTypeRequestValidatorTests.cs  #  9 tests
    │   └── UpdateAccidentTypeRequestValidatorTests.cs  #  9 tests
    │
    ├── Attachments/
    │   ├── CreateAttachmentRequestValidatorTests.cs    # 16 tests
    │   └── UpdateAttachmentRequestValidatorTests.cs    #  4 tests
    │
    ├── Branches/
    │   ├── CreateBranchRequestValidatorTests.cs        # 10 tests
    │   ├── CreateBranchEmailRequestValidatorTests.cs   #  5 tests
    │   ├── CreateBranchPhoneRequestValidatorTests.cs   #  4 tests
    │   ├── UpdateBranchRequestValidatorTests.cs        # 10 tests
    │   ├── UpdateBranchEmailRequestValidatorTests.cs   #  3 tests
    │   └── UpdateBranchPhoneRequestValidatorTests.cs   #  1 tests
    │
    ├── Businesses/
    │   ├── CreateBusinessRequestValidatorTests.cs      #  9 tests
    │   ├── CreateBusinessEmailRequestValidatorTests.cs #  5 tests
    │   ├── CreateBusinessPhoneRequestValidatorTests.cs #  4 tests
    │   ├── UpdateBusinessRequestValidatorTests.cs      #  9 tests
    │   ├── UpdateBusinessEmailRequestValidatorTests.cs #  3 tests
    │   └── UpdateBusinessPhoneRequestValidatorTests.cs #  1 tests
    │
    ├── CorrectiveActions/
    │   ├── CreateCorrectiveActionRequestValidatorTests.cs  # 18 tests
    │   └── UpdateCorrectiveActionRequestValidatorTests.cs  # 21 tests
    │
    ├── Departments/
    │   ├── CreateDepartmentRequestValidatorTests.cs    # 10 tests
    │   └── UpdateDepartmentRequestValidatorTests.cs    #  9 tests
    │
    ├── DigitalEvidences/
    │   ├── CreateDigitalEvidenceRequestValidatorTests.cs   # 22 tests
    │   └── UpdateDigitalEvidenceRequestValidatorTests.cs   # 16 tests
    │
    ├── Employees/
    │   ├── CreateEmployeeRequestValidatorTests.cs      # 32 tests
    │   ├── CreateEmployeeEmailRequestValidatorTests.cs #  3 tests
    │   ├── CreateEmployeePhoneRequestValidatorTests.cs #  0 tests
    │   ├── UpdateEmployeeRequestValidatorTests.cs      # 62 tests
    │   ├── UpdateEmployeeEmailRequestValidatorTests.cs #  2 tests
    │   └── UpdateEmployeePhoneRequestValidatorTests.cs #  0 tests
    │
    ├── EventCategories/
    │   ├── CreateEventCategoryRequestValidatorTests.cs #  7 tests
    │   └── UpdateEventCategoryRequestValidatorTests.cs #  9 tests
    │
    ├── HealthPromotionEntities/
    │   ├── CreateHealthPromotionEntityRequestValidatorTests.cs       #  9 tests
    │   ├── CreateHealthPromotionEntityEmailRequestValidatorTests.cs  #  5 tests
    │   ├── CreateHealthPromotionEntityPhoneRequestValidatorTests.cs  #  4 tests
    │   ├── UpdateHealthPromotionEntityRequestValidatorTests.cs       #  9 tests
    │   ├── UpdateHealthPromotionEntityEmailRequestValidatorTests.cs  #  3 tests
    │   └── UpdateHealthPromotionEntityPhoneRequestValidatorTests.cs  #  1 tests
    │
    ├── OccupationalRiskAdministrators/
    │   ├── CreateOccupationalRiskAdministratorRequestValidatorTests.cs       #  7 tests
    │   ├── CreateOccupationalRiskAdministratorEmailRequestValidatorTests.cs  #  5 tests
    │   ├── CreateOccupationalRiskAdministratorPhoneRequestValidatorTests.cs  #  4 tests
    │   ├── UpdateOccupationalRiskAdministratorRequestValidatorTests.cs       #  7 tests
    │   ├── UpdateOccupationalRiskAdministratorEmailRequestValidatorTests.cs  #  3 tests
    │   └── UpdateOccupationalRiskAdministratorPhoneRequestValidatorTests.cs  #  1 tests
    │
    ├── Positions/
    │   ├── CreatePositionRequestValidatorTests.cs      # 11 tests
    │   └── UpdatePositionRequestValidatorTests.cs      # 13 tests
    │
    ├── RiskClasses/
    │   ├── CreateRiskClassRequestValidatorTests.cs     # 11 tests
    │   └── UpdateRiskClassRequestValidatorTests.cs     # 14 tests
    │
    ├── Users/
    │   ├── CreateUserRequestValidatorTests.cs          # 11 tests
    │   ├── UpdateUserRequestValidatorTests.cs          #  9 tests
    │   ├── ChangePasswordRequestValidatorTests.cs      #  8 tests
    │   └── AssignOrRemoveRoleRequestValidatorTests.cs  #  4 tests
    │
    └── Witnesses/
        ├── CreateWitnessRequestValidatorTests.cs       # 15 tests
        └── UpdateWitnessRequestValidatorTests.cs       # 14 tests
```

---

## Test Breakdown

### Service Tests (120)

Service tests validate the **application service layer** — the classes that orchestrate business logic, coordinate between repositories, and handle cross-cutting concerns like authentication and file storage.

#### Auth & Security (38 tests)

| Test File | Tests | Coverage |
|---|---|---|
| `AuthServiceTests.cs` | 18 | Login, refresh token rotation, revoke, get current user, validate refresh token, revoke all user tokens — including failure paths (user not found, wrong password, expired token, inactive token, missing cookie) |
| `JwtTokenGeneratorTests.cs` | 5 | Valid JWT structure, sub/email/fullName/jti claims, expiration within tolerance, HMAC-SHA256 signature algorithm, additional custom claims |
| `RefreshTokenGeneratorTests.cs` | 3 | Non-empty Base64 output, 64-byte cryptographic entropy, different tokens on each call |
| `CookieManagerTests.cs` | 8 | HttpOnly/Secure/SameSite flags, expiration, get/delete operations, null `HttpContext` resilience |
| `IpAddressProviderTests.cs` | 4 | X-Forwarded-For header parsing, fallback to `RemoteIpAddress`, null context handling |

#### User Management (29 tests)

| Test File | Tests | Coverage |
|---|---|---|
| `UserServiceTests.cs` | 19 | GetById (exists, deleted returns null), Create (success, failure), Update, Delete (soft delete), GetAll (pagination, excludes deleted), GetByEmail, EmailExists, AssignRole, RemoveRole (exists, deleted user), GetUserRoles (exists, deleted user), **ChangePassword** (user not found, valid change) |
| `RoleSyncServiceTests.cs` | 4 | Create missing roles, update changed role names, remove orphaned custom roles, duplicate prevention |
| `PermissionServiceTests.cs` | 2 | Empty role list returns empty, multiple roles return distinct permission names |

#### File Upload Services (15 tests)

| Test File | Tests | Coverage |
|---|---|---|
| `AttachmentServiceTests.cs` | 9 | GetAll (paged), GetById (exists, not found), GetByEntityId, Upload (valid request, empty Base64), UpdateMetadata, Delete (exists, not found throws `KeyNotFoundException`) |
| `DigitalEvidenceServiceTests.cs` | 6 | GetAll (paged), GetById (exists), GetByAccidentId, Upload (valid request), UpdateMetadata, Delete |

#### CRUD Domain Services (38 tests)

| Test File | Tests | Coverage | Notes |
|---|---|---|---|
| `AccidentTypeServiceTests.cs` | 6 | **Full CRUD cycle** — GetAll, GetById (exists/not-exists), Create, Update, Delete | Validates the generic `BaseService<TEntity>` implementation |
| `AccidentServiceTests.cs` | 3 | GetAll with related entities, GetById (exists/not-exists) | Overrides `GetAllAsync` with `.Include()` |
| `BranchServiceTests.cs` | 3 | GetAll with business name, GetById (exists/not-exists) | Overrides `GetAllAsync` with business name projection |
| `CorrectiveActionServiceTests.cs` | 3 | GetAll with accident description, GetById (exists/not-exists) | Overrides `GetAllAsync` with accident description |
| `EmployeeServiceTests.cs` | 3 | GetAll with related entities, GetById (exists/not-exists) | Overrides `GetAllAsync` to include Business, Branch, Department, Position |
| `PositionServiceTests.cs` | 3 | GetAll with department name, GetById (exists/not-exists) | Overrides `GetAllAsync` with department join |
| `WitnessServiceTests.cs` | 3 | GetAll with related data, GetById (exists/not-exists) | Overrides `GetAllAsync` with accident + employee projection |
| `BusinessServiceTests.cs` | 3 | GetAll, GetById (exists/not-exists) | Pure `BaseService` — generic CRUD |
| `DepartmentServiceTests.cs` | 3 | GetAll, GetById (exists/not-exists) | Pure `BaseService` — generic CRUD |
| `EventCategoryServiceTests.cs` | 3 | GetAll, GetById (exists/not-exists) | Pure `BaseService` — generic CRUD |
| `HealthPromotionEntityServiceTests.cs` | 3 | GetAll, GetById (exists/not-exists) | Pure `BaseService` — generic CRUD |
| `OccupationalRiskAdministratorServiceTests.cs` | 3 | GetAll, GetById (exists/not-exists) | Pure `BaseService` — generic CRUD |
| `RiskClassServiceTests.cs` | 3 | GetAll, GetById (exists/not-exists) | Pure `BaseService` — generic CRUD |

### Validator Tests (645)

Validator tests use FluentValidation's `TestValidate` helper to verify that each domain input validator correctly accepts valid data and rejects invalid data across all validation rules.

The validator test files mirror the source validator directory structure under `src/sicoain.api/Validators/`, organized by domain:

#### Domain Coverage

| Domain | Test Files | Tests | Key Validation Rules |
|---|---|---|---|
| **Employees** | 6 | ~99 | Document type/number, first/second name, surnames, address, state/municipality, 6 foreign keys, hiring/termination dates, email format, Colombian phone (+57) |
| **CorrectiveActions** | 2 | 39 | Title, description, priority, status, action type |
| **DigitalEvidences** | 2 | 38 | File name, MIME type, Base64 content, chain of custody, taken-at datetime (not future), taken-by name |
| **Accidents** | 2 | 26 | Description, event date, accident type FK, event category FK, employee FK |
| **Witnesses** | 2 | 29 | Witness name/contact, statement, accident FK, employee FK |
| **Branches** | 6 | ~33 | Branch name, type, address, business FK, email format, Colombian phone |
| **Businesses** | 6 | ~31 | Business name, NIT, email format, Colombian phone |
| **AccidentTypes** | 2 | 18 | Name, code, description, severity |
| **EventCategories** | 2 | 16 | Name, severity level |
| **Departments** | 2 | 19 | Name, code |
| **Positions** | 2 | 24 | Name, description, risk class FK, department FK |
| **RiskClasses** | 2 | 25 | Name, code, contribution rate (range 0.1–50.0%) |
| **HealthPromotionEntities** | 6 | ~31 | Name, address, email, phone |
| **OccupationalRiskAdministrators** | 6 | ~27 | Name, address, email, phone |
| **Attachments** | 2 | 20 | File name, MIME type, Base64 content, entity type FK |
| **Users** | 4 | 32 | Email, password (length/complexity), full name, role assignment |
| **Login** | 1 | 8 | Email format, password not empty |

---

## Testing Patterns

### Service Layer

#### 1. Mock-Based (DI-heavy services)

Services with complex dependency graphs (like `AuthService` with 7 dependencies) use **Moq** to isolate the system under test:

```csharp
public AuthServiceTests()
{
    var userStoreMock = new Mock<IUserStore<User>>();
    _userManagerMock = new Mock<UserManager<User>>(
        userStoreMock.Object,
        new Mock<IOptions<IdentityOptions>>().Object,
        new Mock<IPasswordHasher<User>>().Object,
        Array.Empty<IUserValidator<User>>(),
        Array.Empty<IPasswordValidator<User>>(),
        new Mock<ILookupNormalizer>().Object,
        new Mock<IdentityErrorDescriber>().Object,
        new Mock<IServiceProvider>().Object,
        new Mock<ILogger<UserManager<User>>>().Object);

    _authService = new AuthService(
        _userManagerMock.Object,
        _signInManagerMock.Object,
        _jwtGeneratorMock.Object,
        // ...
    );
}
```

Used for: `AuthService`, `UserService`, `CookieManager`, `JwtTokenGenerator`, `PermissionService`

#### 2. EF Core InMemory (query-heavy services)

Services that query data (like `BaseService<TEntity>` descendants) use **real `ApplicationDbContext` backed by EF Core InMemory** for integration-style tests:

```csharp
var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
    .Options;
var context = new ApplicationDbContext(contextOptions);
// Seed data directly via DbSet
context.Users.AddRange(user1, user2);
await context.SaveChangesAsync();
```

Used for: `AccidentService`, `AccidentTypeService`, `BranchService`, `EmployeeService`, all generic CRUD services, `RoleSyncService`

#### 3. `Testable*` Inner Classes (file I/O)

Services that perform file I/O (`AttachmentService`, `DigitalEvidenceService`) use a **virtual method pattern** to avoid race conditions from mutating `Directory.GetCurrentDirectory()`:

```csharp
// Source: virtual method for testability
protected virtual string GetCurrentBasePath() => Directory.GetCurrentDirectory();

// Test: inner class overrides the method
private class TestableAttachmentService : AttachmentService
{
    public string TestBasePath { get; set; } = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    protected override string GetCurrentBasePath() => TestBasePath;
}
```

#### 4. Identity with Non-Default Key Types

The application uses `User : IdentityUser<int>` (integer primary key), which requires the **4-parameter `UserStore<TUser, TRole, TContext, TKey>` overload**:

```csharp
// ✅ Works for int-keyed IdentityUser
var userStore = new UserStore<User, IdentityRole<int>, ApplicationDbContext, int>(context);

// ❌ Does NOT compile: 3-param overload constrains TUser : IdentityUser<string>
var userStore = new UserStore<User, ApplicationDbContext, int>(context);
```

### Validator Layer

All validator tests follow the **FluentValidation.TestHelper** pattern:

```csharp
[Fact]
public void Should_Have_Error_When_DocumentNumber_IsEmpty()
{
    var model = CreateValidRequest() with { DocumentNumber = "" };
    var result = _validator.TestValidate(model);
    result.ShouldHaveValidationErrorFor(x => x.DocumentNumber);
}

[Theory]
[InlineData("")]           // empty
[InlineData("A")]          // too short
public void Should_Have_Error_When_FirstName_Invalid(string firstName)
{
    var model = CreateValidRequest() with { FirstName = firstName };
    var result = _validator.TestValidate(model);
    result.ShouldHaveValidationErrorFor(x => x.FirstName);
}
```

Key conventions:
- **`CreateValidRequest()`** — factory method returning a valid base request using `with` expressions for mutation
- **`[Theory]` `[InlineData]`** — parameterized tests for multiple invalid values
- **`[Fact]`** — single-value edge cases (null, boundary values)
- **`ShouldHaveValidationErrorFor`** / **`ShouldNotHaveValidationErrorFor`** — FluentValidation-specific assertions
- **`When()` conditions** — tests for conditional validation (e.g., "only validate phone format when phone is not null")

---

## Running Tests

```bash
# Run all tests
dotnet test tests/sicoain.UnitTests/sicoain.UnitTests.csproj

# Run tests with verbose output
dotnet test tests/sicoain.UnitTests/sicoain.UnitTests.csproj -v n

# Run tests with coverage
dotnet test tests/sicoain.UnitTests/sicoain.UnitTests.csproj --collect:"XPlat Code Coverage"

# Run a specific test class
dotnet test tests/sicoain.UnitTests/sicoain.UnitTests.csproj --filter "FullyQualifiedName~AuthServiceTests"

# Run only service tests
dotnet test tests/sicoain.UnitTests/sicoain.UnitTests.csproj --filter "FullyQualifiedName~Services"

# Run only validator tests
dotnet test tests/sicoain.UnitTests/sicoain.UnitTests.csproj --filter "FullyQualifiedName~Validators"

# Build without running tests (verify compilation)
dotnet build tests/sicoain.UnitTests/sicoain.UnitTests.csproj
```

---

## Coverage

| Category | Files | Tests | Status |
|---|---|---|---|
| Service Tests | 23 | 120 | ✅ All passing |
| Validator Tests | 55 | 645 | ✅ All passing |
| **Total** | **78** | **766** | **✅ 0 failures, 0 skipped** |

The test suite is run as part of the development workflow via `dotnet test`. All tests must pass before merging to `master`.

---

## Contributing

When adding new tests:

1. **Service tests**: Place in `Services/` following the `{ServiceName}Tests.cs` naming convention
2. **Validator tests**: Place in `Validators/{Domain}/` matching the source validator's subdirectory
3. Use **AAA pattern** (Arrange, Act, Assert) consistently
4. Test **both success and failure paths** for every public method
5. Use **`[Theory]` with `[InlineData]`** for parameterized invalid-value tests
6. Use **`CreateValidRequest()` factory + `with` expressions** for validator test data
7. **Never modify source code DTOs** to fix tests — adjust the test to match the DTO
