# sicoain.api — Architecture Catalog

> **Technical reference for the API layer of SICOAIN (Sistema de Control de Accidentes e Incidentes)**
> Comprehensive catalog of all abstractions, controllers, services, data layer components, validators, mappings, repositories, configuration, and infrastructure.

---

## Table of Contents

- [Project Configuration](#project-configuration)
- [Packages & Dependencies](#packages--dependencies)
- [Architecture Overview](#architecture-overview)
- [Directory Map](#directory-map)
- [Abstractions (26 interfaces)](#abstractions-26-interfaces)
  - [Service Interfaces](#service-interfaces)
  - [Infrastructure Interfaces](#infrastructure-interfaces)
  - [Base Service Interface](#base-service-interface)
  - [Interface Dependency Map](#interface-dependency-map)
- [Controllers (19 files)](#controllers-19-files)
  - [Base Controllers](#base-controllers)
  - [Auth Controller](#auth-controller)
  - [CRUD Controllers — Accidents Policy Set](#crud-controllers--accidents-policy-set)
  - [CRUD Controllers — Employees Policy Set](#crud-controllers--employees-policy-set)
  - [CRUD Controllers — Settings Policy Set](#crud-controllers--settings-policy-set)
  - [Non-CRUD Controllers](#non-crud-controllers)
  - [Policy-to-Controller Mapping](#policy-to-controller-mapping)
- [Services (24 files)](#services-24-files)
  - [Base Service (abstract)](#base-service-abstract)
  - [Pure BaseService Implementations](#pure-baseservice-implementations)
  - [BaseService + Include Overrides](#baseservice--include-overrides)
  - [Fully Custom Services](#fully-custom-services)
  - [Infrastructure Services](#infrastructure-services)
  - [Auth Services](#auth-services)
  - [Service Category Map](#service-category-map)
- [Data Layer](#data-layer)
  - [ApplicationDbContext](#applicationdbcontext)
  - [Entity Relationship Diagram (Logical)](#entity-relationship-diagram-logical)
  - [Seeders](#seeders)
  - [Migrations](#migrations)
- [Repositories (1 file)](#repositories-1-file)
- [Mappings (1 profile)](#mappings-1-profile)
- [Validators (~57 files)](#validators-57-files)
  - [Base Validators](#base-validators)
  - [Validator Directory Map](#validator-directory-map)
- [Configuration](#configuration)
- [Program.cs Pipeline](#programcs-pipeline)

---

## Project Configuration

| Property | Value |
|----------|-------|
| **Project file** | `src/sicoain.api/sicoain.api.csproj` |
| **SDK** | `Microsoft.NET.Sdk.Web` |
| **Target framework** | `net10.0` |
| **Nullable** | Enabled |
| **Implicit usings** | Enabled |
| **Assembly** | `sicoain.api` |
| **Root namespace** | `sicoain.api` |
| **Analysis mode** | `All` (Roslyn + SecurityCodeScan + Meziantou) |
| **Launch URL (HTTP)** | `http://localhost:5078` |
| **Launch URL (HTTPS)** | `https://localhost:7241` |

---

## Packages & Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| `Asp.Versioning.Mvc` | `10.0.0` | API versioning via URL segment (`/api/v1/`) |
| `AutoMapper.Extensions.Microsoft.DependencyInjection` | `12.0.1` | Object mapping (Entity ↔ DTO) |
| `FluentValidation` | `12.1.1` | Request validation rules |
| `FluentValidation.AspNetCore` | `11.3.1` | Auto-validation pipeline integration |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | `10.0.7` | JWT bearer token authentication |
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | `10.0.7` | ASP.NET Core Identity with EF Core |
| `Microsoft.EntityFrameworkCore.SqlServer` | `10.0.7` | SQL Server database provider |
| `Microsoft.EntityFrameworkCore.Tools` | `10.0.7` | EF Core migrations tooling |
| `Microsoft.EntityFrameworkCore.Design` | `10.0.7` | EF Core design-time support |
| `Swashbuckle.AspNetCore` | `7.0.0` | Swagger/OpenAPI documentation |
| `Meziantou.Analyzer` | `3.0.69` | .NET best-practices analyzer |
| `SecurityCodeScan.VS2019` | `5.6.7` | Security vulnerability analyzer |

**Project Reference:** `sicoain.shared` (shared domain layer — entities, DTOs, enums, constants)

---

## Architecture Overview

```
┌─────────────────────────────────────────────────┐
│                  Controllers                      │
│  (API layer — HTTP concerns, auth, validation)    │
├─────────────────────────────────────────────────┤
│                  Abstractions                     │
│  (Interface contracts for DI and testability)     │
├─────────────────────────────────────────────────┤
│                   Services                        │
│  (Business logic, orchestration, file handling)   │
├─────────────────────────────────────────────────┤
│              Repositories / Data                  │
│  (EF Core DbContext, seeders, data access)        │
├─────────────────────────────────────────────────┤
│           Mappings (AutoMapper Profiles)          │
│  (Entity ↔ DTO transformation)                   │
├─────────────────────────────────────────────────┤
│                 Validators                        │
│  (FluentValidation — request validation rules)    │
└─────────────────────────────────────────────────┘
         depends on ▼
┌─────────────────────────────────────────────────┐
│                 sicoain.shared                    │
│  (Entities, DTOs, Enums, Constants)               │
└─────────────────────────────────────────────────┘
```

### Key Architectural Decisions

1. **Generic CRUD via BaseCrudController + BaseService** — 15 of 19 controllers inherit from `BaseCrudController<TDto, TCreate, TUpdate>` which provides `GET`, `GET {id}`, `POST`, `PATCH {id}`, `DELETE {id}`. Only `AuthController`, `UserController`, `AttachmentsController`, and `DigitalEvidencesController` extend `BaseApiController` directly.

2. **JWT in HttpOnly cookies** — Access tokens are transmitted via `access_token` cookie (not `Authorization` header). Refresh tokens via `refresh_token` cookie. CSRF protection via anti-forgery tokens and `SameSite=Strict`.

3. **Dynamic permission policies** — On startup, all `Permission` names from the database are registered as authorization policies via `RequireClaim("Permission", permName)`. Controllers apply `[Authorize(Policy = "...")]` attributes using these dynamic policies.

4. **Soft delete convention** — `BaseService.DeleteAsync()` checks for an `IsDeleted` property via reflection; if present, performs soft delete. Otherwise falls back to hard delete. `UserService` implements soft delete manually through `UserManager`.

5. **Seeders run on startup** — `RoleSeeder` → `IRoleSyncService` → `PermissionSeeder` → `RolePermissionSeeder` run in sequence during application startup to ensure base data exists.

---

## Directory Map

| Directory | Files | Purpose |
|-----------|-------|---------|
| `Abstractions/` | 26 | Interface contracts for DI |
| `Constants/` | 0 | *(reserved)* |
| `Controllers/` | 19 | API endpoint definitions |
| `Data/` | 1 + 3 seeders | DbContext and seeders |
| `Data/Seeders/` | 3 | Database seeding logic |
| `Mappings/` | 1 | AutoMapper profiles |
| `Migrations/` | 3 | EF Core migrations |
| `Repositories/` | 1 | Data access layer |
| `Services/` | 24 | Business logic |
| `Validators/` | 19 directories (~57 files) | FluentValidation rules |

---

## Abstractions (26 interfaces)

All interfaces live under `sicoain.api.Abstractions`. Organized by role:

### Service Interfaces

| Interface | Extends | Type Parameters | Key Methods (beyond IBaseService) |
|-----------|---------|-----------------|------------------------------------|
| `IAccidentService` | `IBaseService<AccidentDto, CreateAccidentRequest, UpdateAccidentRequest>` | — | *(none — pure CRUD)* |
| `IAccidentTypeService` | `IBaseService<AccidentTypeDto, CreateAccidentTypeRequest, UpdateAccidentTypeRequest>` | — | *(none)* |
| `IAttachmentService` | *(standalone)* | — | `GetByEntityIdAsync(AttachmentEntityType, int)`, `UploadAsync(CreateAttachmentRequest)`, `UpdateMetadataAsync(int, UpdateAttachmentRequest)`, `DeleteAsync(int)` |
| `IBranchService` | `IBaseService<BranchDto, CreateBranchRequest, UpdateBranchRequest>` | — | *(none)* |
| `IBusinessService` | `IBaseService<BusinessDto, CreateBusinessRequest, UpdateBusinessRequest>` | — | *(none)* |
| `ICorrectiveActionService` | `IBaseService<CorrectiveActionDto, CreateCorrectiveActionRequest, UpdateCorrectiveActionRequest>` | — | *(none)* |
| `IDepartmentService` | `IBaseService<DepartmentDto, CreateDepartmentRequest, UpdateDepartmentRequest>` | — | *(none)* |
| `IDigitalEvidenceService` | *(standalone)* | — | `GetByAccidentIdAsync(int)`, `UploadAsync(CreateDigitalEvidenceRequest)`, `UpdateMetadataAsync(int, UpdateDigitalEvidenceRequest)`, `DeleteAsync(int)` |
| `IEmployeeService` | `IBaseService<EmployeeDto, CreateEmployeeRequest, UpdateEmployeeRequest>` | — | *(none)* |
| `IEventCategoryService` | `IBaseService<EventCategoryDto, CreateEventCategoryRequest, UpdateEventCategoryRequest>` | — | *(none)* |
| `IHealthPromotionEntityService` | `IBaseService<HealthPromotionEntityDto, CreateHealthPromotionEntityRequest, UpdateHealthPromotionEntityRequest>` | — | *(none)* |
| `IOccupationalRiskAdministratorService` | `IBaseService<OccupationalRiskAdministratorDto, ...>` | — | *(none)* |
| `IPermissionService` | *(standalone)* | — | `GetUserPermissionNameAsync(User)` |
| `IPositionService` | `IBaseService<PositionDto, CreatePositionRequest, UpdatePositionRequest>` | — | *(none)* |
| `IRiskClassService` | `IBaseService<RiskClassDto, CreateRiskClassRequest, UpdateRiskClassRequest>` | — | *(none)* |
| `IRoleSyncService` | *(standalone)* | — | `SynchronizeRoleAsync()` |
| `IUserService` | `IBaseService<UserDto, CreateUserRequest, UpdateUserRequest>` | — | `GetByEmailAsync(string)`, `EmailExistsAsync(string)`, `AssignRoleAsync(int, string)`, `RemoveRoleAsync(int, string)`, `ChangePasswordAsync(int, ChangePasswordRequest)`, `GetUserRolesAsync(int)` |
| `IWitnessService` | `IBaseService<WitnessDto, CreateWitnessRequest, UpdateWitnessRequest>` | — | *(none)* |

### Infrastructure Interfaces

| Interface | Key Methods | Purpose |
|-----------|-------------|---------|
| `IAuthenticationProvider` | `AuthenticateAsync(LoginRequest)` | Authentication abstraction |
| `IAuthService` | `LoginAsync(LoginRequest)`, `RefreshTokenAsync()`, `RevokeTokenAsync()`, `GetCurrentUserAsync(ClaimsPrincipal)`, `ValidateRefreshTokenAsync(string)`, `RevokeAllUserTokensAsync(int)` | Full auth lifecycle |
| `ICookieManager` | `SetTokenCookie(string, string, int)`, `GetCookieValue(string)`, `DeleteCookie(string)`, `GetHttpContext()` | HttpOnly cookie operations |
| `IIpAddressProvider` | `GetCurrentIpAddress()` | Client IP resolution (with X-Forwarded-For support) |
| `IJwtTokenGenerator` | `GenerateToken(User, List<Claim>?)` | JWT creation with custom claims |
| `IRefreshTokenGenerator` | `GenerateToken()` | Cryptographic random token generation |
| `IRefreshTokenRepository` | `AddAsync(RefreshToken)`, `GetByTokenAsync(string)`, `RevokeAsync(...)`, `RevokeAllForUserAsync(...)`, `UpdateAsync(...)`, `SaveChangesAsync(...)` | Refresh token persistence |

### Base Service Interface

```csharp
public interface IBaseService<TDto, TCreateRequest, TUpdateRequest>
    where TDto : class
    where TCreateRequest : class
    where TUpdateRequest : class
{
    Task<PagedResponse<TDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
    Task<TDto?> GetByIdAsync(int id);
    Task<TDto> CreateAsync(TCreateRequest request);
    Task<TDto?> UpdateAsync(int id, TUpdateRequest request);
    Task<bool> DeleteAsync(int id);
}
```

### Interface Dependency Map

```
IAuthService
├── IJwtTokenGenerator
├── IRefreshTokenGenerator
├── IRefreshTokenRepository
├── ICookieManager
├── IIpAddressProvider
└── IPermissionService

IRefreshTokenRepository         → (uses ApplicationDbContext directly)
IPermissionService              → UserManager<User>, RoleManager<IdentityRole<int>>, ApplicationDbContext
IRoleSyncService                → RoleManager<IdentityRole<int>>, ApplicationDbContext
IUserService                    → UserManager<User>, RoleManager<IdentityRole<int>>, IMapper

All IBaseService implementations → ApplicationDbContext, IMapper
AttachmentService               → ApplicationDbContext, IWebHostEnvironment
DigitalEvidenceService          → ApplicationDbContext, IWebHostEnvironment
```

---

## Controllers (19 files)

### Base Controllers

#### `BaseApiController` (abstract)

```csharp
namespace sicoain.api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
    }
}
```

Foundation for ALL controllers. Provides:
- `[ApiController]` — automatic model validation, binding
- `[Route("api/v1/[controller]")]` — URL segment versioning

#### `BaseCrudController<TDto, TCreateRequest, TUpdateRequest>` (abstract)

```csharp
[Authorize]
public abstract class BaseCrudController<TDto, TCreateRequest, TUpdateRequest> : BaseApiController
```

Provides generic CRUD endpoints via `IBaseService<TDto, TCreateRequest, TUpdateRequest>`:

| HTTP Method | Route | Action | Returns |
|-------------|-------|--------|---------|
| `GET` | `/api/v1/{controller}` | `GetAll(pageNumber, pageSize)` | `PagedResponse<TDto>` |
| `GET` | `/api/v1/{controller}/{id}` | `GetById(int id)` | `TDto` |
| `POST` | `/api/v1/{controller}` | `Create(TCreateRequest)` | `CreatedAtAction → TDto` |
| `PATCH` | `/api/v1/{controller}/{id}` | `Update(id, TUpdateRequest)` | `TDto` |
| `DELETE` | `/api/v1/{controller}/{id}` | `Delete(int id)` | `NoContent` |

Uses reflection (`GetProperty("Id")`) on `GetId(TDto)` for the `CreatedAtAction` location header. All overridable (`virtual`).

---

### Auth Controller

#### `AuthController`

| Route | Action | Auth | Description |
|-------|--------|------|-------------|
| `POST /api/v1/auth/login` | `Login(LoginRequest)` | `[AllowAnonymous]` | Email/password authentication. Returns `AuthResponse`, sets `access_token` and `refresh_token` HttpOnly cookies |
| `POST /api/v1/auth/refresh` | `Refresh()` | `[AllowAnonymous]` | Uses `refresh_token` cookie to issue new token pair (rotation) |
| `POST /api/v1/auth/logout` | `Logout()` | `[Authorize]` | Revokes refresh token, clears cookies |
| `POST /api/v1/auth/me` | `GetCurrentUser()` | `[Authorize]` | Returns `{ Id, Email, FullName }` of authenticated user |

---

### CRUD Controllers — Accidents Policy Set

#### `UserController`

Extends `BaseCrudController<UserDto, CreateUserRequest, UpdateUserRequest>`.

| Route | Action | Policy | Notes |
|-------|--------|--------|-------|
| `GET /` | `GetAll` | `Users.View` | Paginated user list (soft-delete filtered) |
| `GET /{id}` | `GetById` | `Users.View` | By primary key |
| `GET /email/{email}` | `GetByEmailAsync` | `Users.View` | Lookup by email |
| `GET /email-exists/{email}` | `EmailExistsAsync` | `Users.View` | Boolean check |
| `GET /roles/{id}` | `GetUserRolesAsync` | `Users.View` | List assigned roles |
| `POST /` | `Create` | `Users.Create` | Creates user + assigns roles |
| `PATCH /{id}` | `Update` | `Users.Edit` | Partial update |
| `PATCH /assign-role/{id}` | `AssignRoleAsync` | `Users.Edit` | Adds role assignment |
| `PATCH /remove-role/{id}` | `RemoveRoleAsync` | `Users.Edit` | Removes role assignment |
| `PATCH /change-password/{id}` | `ChangePasswordAsync` | `Users.Edit` | Password change |
| `DELETE /{id}` | `Delete` | `Users.Delete` | Soft delete |

#### `AccidentsController`

| Action | Policy |
|--------|--------|
| GetAll, GetById | `Accidents.View` |
| Create | `Accidents.Create` |
| Update | `Accidents.Edit` |
| Delete | `Accidents.Delete` |

#### `WitnessesController`

Same policy set as AccidentsController: `Accidents.View/Create/Edit/Delete`.

#### `EmployeesController`

| Action | Policy |
|--------|--------|
| GetAll, GetById | `Employees.View` |
| Create | `Employees.Create` |
| Update | `Employees.Edit` |
| Delete | `Employees.Delete` |

---

### CRUD Controllers — Settings Policy Set

All the following controllers use the same policy mapping:

| Action | Policy |
|--------|--------|
| GetAll, GetById | `Settings.View` |
| Create, Update, Delete | `Settings.Edit` |

Controllers in this group:
- `AccidentTypesController`
- `BranchesController`
- `BusinessesController`
- `CorrectiveActionsController`
- `DepartmentsController`
- `EventCategoriesController`
- `HealthPromotionEntitiesController`
- `OccupationalRiskAdministratorsController`
- `PositionsController`
- `RiskClassesController`

---

### Non-CRUD Controllers

#### `AttachmentsController`

**Extends `BaseApiController`** (NOT `BaseCrudController`). Polymorphic attachment management using dynamic inline authorization:

```csharp
[ApiController]
[Authorize]
public class AttachmentsController : BaseApiController
```

| Route | Action | Auth | Notes |
|-------|--------|------|-------|
| `GET /` | `GetAll` | Inline: checks entity type → `Accidents.View` or `Employees.View` | Query params: `entityType`, `entityId` |
| `GET /{id}` | `GetById` | Same inline logic | — |
| `POST /` | `Upload` | Inline: `Accidents.Create` or `Employees.Create` | `[FromForm]` multipart, `[ValidateAntiForgeryToken]`, Base64 content |
| `PATCH /{id}` | `UpdateMetadata` | Inline: `Accidents.Edit` or `Employees.Edit` | Description only |
| `DELETE /{id}` | `Delete` | Inline: `Accidents.Edit` or `Employees.Edit` | Hard delete (file + record) |

Auth switches on `AttachmentEntityType` enum (Accident, CorrectiveAction, Witness, Employee) using `User.HasClaim("Permission", permName)`.

#### `DigitalEvidencesController`

**Extends `BaseApiController`**. Uses standard `[Authorize(Policy)]` attributes:

| Route | Action | Policy |
|-------|--------|--------|
| `GET /` | `GetAll` | `Accidents.View` |
| `GET /{id}` | `GetById` | `Accidents.View` |
| `GET /by-accident/{accidentId}` | `GetByAccidentId` | `Accidents.View` |
| `POST /` | `Upload` | `Accidents.Create` |
| `PATCH /{id}` | `UpdateMetadata` | `Accidents.Edit` |
| `DELETE /{id}` | `Delete` | `Accidents.Delete` |

Uses `[FromForm]`, `[ValidateAntiForgeryToken]` on mutation endpoints.

---

### Policy-to-Controller Mapping

| Policy | Controllers Using It |
|--------|---------------------|
| `Accidents.View` | Accidents, Witnesses, Attachments (dynamic), DigitalEvidences |
| `Accidents.Create` | Accidents, Attachments (dynamic), DigitalEvidences |
| `Accidents.Edit` | Accidents, Witnesses, Attachments (dynamic), DigitalEvidences |
| `Accidents.Delete` | Accidents, DigitalEvidences |
| `Employees.View` | Employees, Attachments (dynamic) |
| `Employees.Create` | Employees, Attachments (dynamic) |
| `Employees.Edit` | Employees, Attachments (dynamic) |
| `Employees.Delete` | Employees |
| `Users.View` | User |
| `Users.Create` | User |
| `Users.Edit` | User |
| `Users.Delete` | User |
| `Settings.View` | AccidentTypes, Branches, Businesses, CorrectiveActions, Departments, EventCategories, HealthPromotionEntities, OccupationalRiskAdministrators, Positions, RiskClasses |
| `Settings.Edit` | AccidentTypes, Branches, Businesses, CorrectiveActions, Departments, EventCategories, HealthPromotionEntities, OccupationalRiskAdministrators, Positions, RiskClasses |
| *(none — AllowAnonymous)* | Auth (login, refresh) |
| *(none — Authorize)* | Auth (logout, me) |

---

## Services (24 files)

### Base Service (abstract)

```csharp
public abstract class BaseService<TEntity, TDto, TCreateRequest, TUpdateRequest>
    : IBaseService<TDto, TCreateRequest, TUpdateRequest>
    where TEntity : class
```

**File:** `Services/BaseService.cs`
**Dependencies:** `ApplicationDbContext`, `IMapper`

| Method | Implementation |
|--------|---------------|
| `GetAllAsync` | `_context.Set<TEntity>()` → paginated → `_mapper.Map<List<TDto>>` |
| `GetByIdAsync` | `_context.Set<TEntity>().FindAsync(id)` → `_mapper.Map<TDto>` |
| `CreateAsync` | `_mapper.Map<TEntity>` → `AddAsync` → SaveChanges → `_mapper.Map<TDto>` |
| `UpdateAsync` | Find → `_mapper.Map(request, entity)` → SaveChanges → `_mapper.Map<TDto>` |
| `DeleteAsync` | **Soft delete** if entity has `IsDeleted` property (reflection), else hard delete |
| `ExistsAsync` | `AnyAsync(e => EF.Property<int>(e, "Id") == id)` |

---

### Pure BaseService Implementations

These services inherit `BaseService` with **no overrides** — pure generic CRUD:

| Service | Entity → DTO Mapping |
|---------|---------------------|
| `AccidentTypeService` | `AccidentType` → `AccidentTypeDto` |
| `BusinessService` | `Business` → `BusinessDto` |
| `DepartmentService` | `Department` → `DepartmentDto` |
| `EventCategoryService` | `EventCategory` → `EventCategoryDto` |
| `HealthPromotionEntityService` | `HealthPromotionEntity` → `HealthPromotionEntityDto` |
| `OccupationalRiskAdministratorService` | `OccupationalRiskAdministrator` → `OccupationalRiskAdministratorDto` |
| `RiskClassService` | `RiskClass` → `RiskClassDto` |

---

### BaseService + Include Overrides

These services override `GetAllAsync` and `GetByIdAsync` to add `.Include()` calls for navigation properties:

| Service | Includes | Max Depth |
|---------|----------|-----------|
| `AccidentService` | `.Include(e => e.Employee)`, `.Include(e => e.AccidentType)`, `.Include(p => p.EventCategory)` | 1 |
| `BranchService` | `.Include(e => e.Business)` | 1 |
| `CorrectiveActionService` | `.Include(e => e.Accident)` | 1 |
| `EmployeeService` | `.Include(e => e.Business)`, `.Include(e => e.Branch)`, `.Include(e => e.Position).ThenInclude(p => p!.Department)` | 2 |
| `PositionService` | `.Include(e => e.Department)` | 1 |
| `WitnessService` | `.Include(e => e.Accident)`, `.Include(e => e.Employee)` | 1 |

---

### Fully Custom Services

#### `AttachmentService` (134 lines)

- **Interface:** `IAttachmentService` (standalone — NOT `IBaseService`)
- **Dependencies:** `ApplicationDbContext`, `IWebHostEnvironment`
- **File storage:** `wwwroot/uploads/`
- **Integrity:** SHA-256 hash verification on upload
- **Unique method:** `GetByEntityIdAsync(AttachmentEntityType entityType, int id)` — polymorphic query by entity type discriminator
- **Delete:** Removes physical file + database record
- **Testability:** `GetCurrentBasePath()` is `virtual`

#### `DigitalEvidenceService` (135 lines)

- **Interface:** `IDigitalEvidenceService` (standalone — NOT `IBaseService`)
- **Dependencies:** `ApplicationDbContext`, `IWebHostEnvironment`
- **File storage:** `wwwroot/uploads/` (same as Attachment)
- **Unique method:** `GetByAccidentIdAsync(int accidentId)` — filter evidence by parent accident
- **Extra fields vs Attachment:** `TakenAt`, `TakenByName`, `ChainOfCustody`
- **Testability:** `GetCurrentBasePath()` is `virtual`

#### `UserService` (184 lines)

- **Interface:** `IUserService` extends `IBaseService<UserDto, CreateUserRequest, UpdateUserRequest>`
- **Dependencies:** `UserManager<User>`, `RoleManager<IdentityRole<int>>`, `IMapper`
- **Uses UserManager** instead of DbContext directly (Identity integration)
- **Unique methods:** `GetByEmailAsync`, `EmailExistsAsync`, `AssignRoleAsync`, `RemoveRoleAsync`, `ChangePasswordAsync`, `GetUserRolesAsync`
- **Soft delete:** `IsDeleted = true`, `DeletedAt = DateTime.UtcNow` via UserManager

---

### Infrastructure Services

| Service | Interface | Lines | Key Behavior |
|---------|-----------|-------|-------------|
| `JwtTokenGenerator` | `IJwtTokenGenerator` | 54 | Creates JWT with `sub`, `email`, `jti`, `fullName`, and optional `Permission` claims. Reads SecretKey/Issuer/Audience/ExpirationMinutes from `JwtSettings` config section |
| `RefreshTokenGenerator` | `IRefreshTokenGenerator` | 14 | Generates 64-byte cryptographic random token (`RandomNumberGenerator.GetBytes(64)` → Base64) |
| `CookieManager` | `ICookieManager` | 57 | Sets/gets/deletes HttpOnly cookies. Uses `SameSiteMode.Strict`, `Secure` based on request scheme |
| `IpAddressProvider` | `IIpAddressProvider` | 33 | Resolves client IP from `X-Forwarded-For` header (proxy support) or `RemoteIpAddress` |
| `PermissionService` | `IPermissionService` | 48 | Resolves user's permissions: `UserManager.GetRolesAsync` → `CustomRoles` → `RolePermissions` → `Permissions.Name` |
| `RoleSyncService` | `IRoleSyncService` | 73 | Synchronizes `IdentityRole<int>` with custom `Roles` table. Creates missing custom roles, updates names, removes orphans |

---

### Auth Services

#### `AuthService` (196 lines)

The central authentication orchestrator, implementing `IAuthService`:

**Dependencies:** `UserManager<User>`, `SignInManager<User>`, `IJwtTokenGenerator`, `IRefreshTokenGenerator`, `IRefreshTokenRepository`, `ICookieManager`, `IIpAddressProvider`, `IPermissionService`

| Method | Flow |
|--------|------|
| `LoginAsync` | Find user by email → CheckPasswordSignInAsync (with lockout) → Load permission claims → Generate JWT → Generate refresh token → Persist refresh token → Set cookies (access_token: 15min, refresh_token: 7d) → Return AuthResponse |
| `RefreshTokenAsync` | Read refresh_token cookie → Find in DB → Validate IsActive → Revoke old token (rotation) → Generate new JWT + refresh token → Set new cookies |
| `RevokeTokenAsync` | Read cookie → Find token → Revoke with reason "Logout" → Delete both cookies |
| `GetCurrentUserAsync` | Extract `NameIdentifier` claim → `FindByIdAsync` |
| `ValidateRefreshTokenAsync` | Find by token string → Check IsActive |
| `RevokeAllUserTokensAsync` | Delegates to `IRefreshTokenRepository.RevokeAllForUserAsync` |

---

### Service Category Map

| Category | Count | Services |
|----------|-------|----------|
| **Pure BaseService** | 7 | AccidentType, Business, Department, EventCategory, HealthPromotionEntity, OccupationalRiskAdministrator, RiskClass |
| **BaseService + Includes** | 6 | Accident, Branch, CorrectiveAction, Employee, Position, Witness |
| **Fully Custom (CRUD alternative)** | 3 | Attachment, DigitalEvidence, User |
| **Auth** | 1 | AuthService |
| **Auth Infrastructure** | 4 | JwtTokenGenerator, RefreshTokenGenerator, CookieManager, IpAddressProvider |
| **Cross-cutting** | 2 | PermissionService, RoleSyncService |
| **Repository** | 1 | RefreshTokenRepository |

---

## Data Layer

### ApplicationDbContext

**File:** `Data/ApplicationDbContext.cs`
**Namespace:** `sicoain.api.Data`
**Base class:** `IdentityDbContext<User, IdentityRole<int>, int>`

#### DbSet Properties (organized by domain)

```csharp
// CORE BUSINESS
DbSet<Employee> Employees
DbSet<Accident> Accidents
DbSet<Witness> Witnesses
DbSet<CorrectiveAction> CorrectiveActions
DbSet<CorrectiveActionTracking> CorrectiveActionTrackings

// FILES & EVIDENCE
DbSet<Attachment> Attachments
DbSet<DigitalEvidence> DigitalEvidences

// ORGANIZATIONAL STRUCTURE
DbSet<Business> Businesses
DbSet<Branch> Branches
DbSet<Department> Departments
DbSet<Position> Positions

// CATALOGS & CLASSIFICATIONS
DbSet<EventCategory> EventCategories
DbSet<AccidentType> AccidentTypes
DbSet<RiskClass> RiskClasses

// EXTERNAL ENTITIES
DbSet<OccupationalRiskAdministrator> OccupationalRiskAdministrators
DbSet<HealthPromotionEntity> HealthPromotionEntities

// CUSTOM ROLES & PERMISSIONS
DbSet<Roles> CustomRoles
DbSet<Permissions> Permissions
DbSet<RolePermissions> RolePermissions

// PHONES & EMAILS (polymorphic)
DbSet<BusinessEmail> BusinessEmails
DbSet<BusinessPhone> BusinessPhones
DbSet<BranchEmail> BranchEmails
DbSet<BranchPhone> BranchPhones
DbSet<EmployeeEmail> EmployeeEmails
DbSet<EmployeePhone> EmployeePhones
DbSet<OccupationalRiskAdministratorEmail> OccupationalRiskAdministratorEmails
DbSet<OccupationalRiskAdministratorPhone> OccupationalRiskAdministratorPhones
DbSet<HealthPromotionEntityEmail> HealthPromotionEntityEmails
DbSet<HealthPromotionEntityPhone> HealthPromotionEntityPhones

// AUTH
DbSet<RefreshToken> RefreshTokens
```

#### Identity Table Mapping

| ASP.NET Default | Custom Name |
|----------------|-------------|
| `AspNetUsers` | `Users` |
| `AspNetRoles` | `Roles` |
| `AspNetUserRoles` | `UserRoles` |
| `AspNetUserClaims` | `UserClaims` |
| `AspNetUserLogins` | `UserLogins` |
| `AspNetUserTokens` | `UserTokens` |
| `AspNetRoleClaims` | `RoleClaims` |

#### Relationship Configuration (OnModelCreating)

**Accident relationships:**
```
Accident ──Restrict──→ Employee
Accident ──Restrict──→ AccidentType
Accident ──Restrict──→ EventCategory
Accident ──Cascade──→ Witnesses
Accident ──Cascade──→ CorrectiveActions
Accident ──Cascade──→ DigitalEvidences

Witness ──Cascade──→ Accident
Witness ──Restrict──→ Employee

CorrectiveAction ──Cascade──→ Accident
CorrectiveActionTracking ──Cascade──→ CorrectiveAction

DigitalEvidence ──Cascade──→ Accident

Employee ──Restrict──→ Business
Employee ──Restrict──→ Branch
Employee ──Restrict──→ Position
Position ──Restrict──→ Department
Branch ──Cascade──→ Business
```

**Key pattern:** `Restrict` prevents accidental cascade deletes on core entities. `Cascade` is used for dependent child collections (witnesses, trackings, evidence, branch → business).

---

### Entity Relationship Diagram (Logical)

```
Business
├── BusinessPhone (FK → Business)
├── BusinessEmail (FK → Business)
└── Branch (Cascade)
    ├── BranchPhone (FK → Branch)
    ├── BranchEmail (FK → Branch)
    └── Employee (Restrict)
        ├── EmployeePhone (FK → Employee)
        ├── EmployeeEmail (FK → Employee)
        ├── EmployeeContact (FK → Employee)
        │   ├── EmployeeContactPhone (FK → EmployeeContact)
        │   └── EmployeeContactEmail (FK → EmployeeContact)
        └── Accident (Restrict → Employee)
            ├── DigitalEvidence (Cascade)
            ├── Witness (Cascade → Accident, Restrict → Employee)
            │   └── Employee (Restrict)
            └── CorrectiveAction (Cascade)
                └── CorrectiveActionTracking (Cascade)

Employee ──Restrict──→ Position
Position ──Restrict──→ Department
Position ──Restrict──→ RiskClass
Employee ──Restrict──→ HealthPromotionEntity
Employee ──Restrict──→ OccupationalRiskAdministrator

IdentityUser<int> ←── RefreshToken
```

---

### Seeders

#### `RoleSeeder`

- **Type:** `static class`
- **Phase:** 1 (runs first)
- **Creates Identity roles:** `Admin`, `Investigator`, `Supervisor`, `Consultant`
- **Uses:** `RoleManager<IdentityRole<int>>`
- **Idempotent:** Checks `RoleExistsAsync` before creating

#### `RoleSyncService.SynchronizeRoleAsync`

- **Phase:** 2 (runs after RoleSeeder)
- **Synchronizes** Identity roles → custom `Roles` table
- Creates missing `Roles` entries with `IdentityRoleId` FK
- Updates names if changed
- Removes orphan custom roles

#### `PermissionSeeder`

- **Type:** `static class`
- **Phase:** 3 (runs after sync)
- **Discovers all permissions** from `AppPermissions` constants via reflection
- **Merges** with existing DB permissions (idempotent — only adds missing)
- **Derives** `Module` and `Action` from permission name (e.g., `"Accidents.View"` → Module: `"Accidents"`, Action: `"View"`)

#### `RolePermissionSeeder`

- **Type:** `static class`
- **Phase:** 4 (runs last)
- **Assigns permissions to roles:**
  - `Admin` → ALL permissions
  - `Investigator` → `Accidents.View/Create/Edit`, `Reports.View`, `Employees.View`
  - `Supervisor` → `Accidents.View`, `Employees.View`
  - `Consultant` → `Reports.View`, `Accidents.View`
- **Idempotent:** Checks for existing assignments before inserting

---

### Migrations

| Migration | Created | Description |
|-----------|---------|-------------|
| `20260526194841_InitialCreate` | 2026-05-26 | Full initial schema: all entity tables, Identity tables, relationships |

---

## Repositories (1 file)

### `RefreshTokenRepository`

**File:** `Repositories/RefreshTokenRepository.cs`
**Interface:** `IRefreshTokenRepository`
**Dependencies:** `ApplicationDbContext`

| Method | Implementation |
|--------|---------------|
| `AddAsync` | `_context.RefreshTokens.AddAsync` |
| `GetByTokenAsync` | `.Include(rt => rt.User).FirstOrDefaultAsync(rt => rt.Token == token)` |
| `RevokeAsync` | Calls `token.Revoke(revokedByIp, reason)` (in-memory, caller must SaveChanges) |
| `RevokeAllForUserAsync` | Queries active tokens for user → revokes each → returns count |
| `UpdateAsync` | `_context.RefreshTokens.Update` |
| `SaveChangesAsync` | Delegates to `_context.SaveChangesAsync` |

---

## Mappings (1 profile)

### `UserProfile`

**File:** `Mappings/UserProfile.cs`
**AutoMapper Profile** registered via `builder.Services.AddAutoMapper(typeof(Program))`.

| Mapping | Direction | Notes |
|---------|-----------|-------|
| `User → UserDto` | Entity → DTO | `IsActive` = `!IsDeleted`. Maps audit fields explicitly |
| `CreateUserRequest → User` | DTO → Entity | `UserName = Email`. Ignores identity-managed fields (PasswordHash, SecurityStamp, etc.). Sets `CreatedAt = DateTime.UtcNow`, `IsDeleted = false` |
| `UpdateUserRequest → User` | DTO → Entity | Conditional mapping: only updates non-null properties (`Condition((src, dest, srcMember) => srcMember != null)`) |

**Note:** AutoMapper is registered but only `UserProfile` is explicitly defined. The generic CRUD controllers rely on `_mapper.Map<List<TDto>>(items)` which works for simple entity → DTO mappings without explicit profiles (convention-based mapping).

---

## Validators (~57 files)

**Framework:** FluentValidation
**Registration:** `builder.Services.AddValidatorsFromAssemblyContaining<Program>()` (auto-discovers all validators)
**Pipeline:** `AddFluentValidationAutoValidation` with `DisableDataAnnotationsValidation = true`

### Base Validators

#### `LoginRequestValidator`

| Field | Rules |
|-------|-------|
| `Email` | `NotEmpty`, `EmailAddress` |
| `Password` | `NotEmpty`, `MinLength(8)`, `MaxLength(32)` |

#### `BaseCreateEntityEmailRequestValidator<T>`

Generic base for email sub-entity creation:
| Field | Rules |
|-------|-------|
| `Email` | `NotEmpty`, `EmailAddress` |

**Inherited by:** CreateBusinessEmailRequestValidator, CreateBranchEmailRequestValidator, CreateEmployeeEmailRequestValidator, CreateEmployeeContactEmailRequestValidator, CreateHealthPromotionEntityEmailRequestValidator, CreateOccupationalRiskAdministratorEmailRequestValidator

#### `BaseCreateEntityPhoneRequestValidator<T>`

Generic base for phone sub-entity creation:
| Field | Rules |
|-------|-------|
| `Phone` | `NotEmpty`, `Matches(@"^(\+?57)?[0-9]{10}$")` (Colombian format: 10 digits or +57 prefix) |

**Inherited by:** CreateBusinessPhoneRequestValidator, CreateBranchPhoneRequestValidator, CreateEmployeePhoneRequestValidator, CreateEmployeeContactPhoneRequestValidator, CreateHealthPromotionEntityPhoneRequestValidator, CreateOccupationalRiskAdministratorPhoneRequestValidator

---

### Validator Directory Map

| Directory | Validators | Entity/Request |
|-----------|------------|----------------|
| `Validators/Accidents/` | 2 | CreateAccidentRequest, UpdateAccidentRequest |
| `Validators/AccidentTypes/` | 2 | CreateAccidentTypeRequest, UpdateAccidentTypeRequest |
| `Validators/Attachments/` | 2 | CreateAttachmentRequest (Base64 validation), UpdateAttachmentRequest |
| `Validators/Branches/` | 6 | Create/Update Branch, BranchPhone (create/update), BranchEmail (create/update) |
| `Validators/Businesses/` | 7 | Create/Update Business, BusinessPhone (create/update), BusinessEmail (create/update) |
| `Validators/CorrectiveActions/` | 2 | CreateCorrectiveAction, UpdateCorrectiveAction |
| `Validators/Departments/` | 2 | CreateDepartment, UpdateDepartment |
| `Validators/DigitalEvidences/` | 2 | CreateDigitalEvidence (Base64 validation), UpdateDigitalEvidence |
| `Validators/Employees/` | 8 | Create/Update Employee, EmployeePhone (create/update), EmployeeEmail (create/update), EmployeeContactPhone/Email validators |
| `Validators/EventCategories/` | 2 | CreateEventCategory, UpdateEventCategory |
| `Validators/HealthPromotionEntities/` | 7 | Create/Update HPE, HPE Phone (create/update), HPE Email (create/update) |
| `Validators/OccupationalRiskAdministrators/` | 7 | Create/Update ORA, ORA Phone (create/update), ORA Email (create/update) |
| `Validators/Positions/` | 2 | CreatePosition, UpdatePosition |
| `Validators/RiskClasses/` | 2 | CreateRiskClass, UpdateRiskClass |
| `Validators/Users/` | 4 | CreateUser, UpdateUser, ChangePassword, AssignOrRemoveRole |
| `Validators/Witnesses/` | 2 | CreateWitness, UpdateWitness |

**Total: ~57 validator files across 17 domain directories + 3 root validators.**

#### Common Validation Patterns

1. **Create validators:** `NotEmpty`, `Length(min, max)` on required fields, `MaximumLength` on optional, `GreaterThan(0)` on FK IDs, `IsInEnum` on enum values
2. **Update validators:** Same rules but wrapped with `.When(x => x.Field.HasValue)` or `.When(x => !string.IsNullOrWhiteSpace(x.Field))` for partial updates
3. **Phone validators:** Colombian format regex `^(\+?57)?[0-9]{10}$`
4. **Email validators:** `.EmailAddress()` validation
5. **Base64 content validation:** Custom `IsValidBase64` method in Attachment and DigitalEvidence validators

---

## Configuration

### `appsettings.json` (base)

```json
{
  "JwtSettings": {
    "SecretKey": "SICOAIN-SUPER-SECRET-KEY-FOR-JWT-2024-YOUR-LONG-KEY-HERE-32+CHARS",
    "Issuer": "SICOAIN-API",
    "Audience": "SICOAIN-Frontend",
    "ExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### `appsettings.Development.json` (override)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=SICOAIN_DB;User Id=sa;Password=51c04in!2024;TrustServerCertificate=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

**Database:** SQL Server 2022+ via Docker (`localhost:1433`), database `SICOAIN_DB`, `sa` account.

---

## Program.cs Pipeline

Complete registration order in `Program.cs`:

```
1. API Versioning (Asp.Versioning.UrlSegmentApiVersionReader)
2. DbContext (ApplicationDbContext → SQL Server)
3. Identity (User + IdentityRole<int> + EF Stores + DefaultTokenProviders)
   └── Password: digit, 8+ len, non-alphanum, upper, lower
   └── Lockout: 15min, 5 attempts
   └── User: UniqueEmail, strict username chars
4. Authentication (JwtBearer)
   └── Cookie reader: reads access_token from HttpOnly cookie
   └── Token validation: issuer, audience, lifetime, signing key
5. Anti-forgery (CSRF: X-CSRF-TOKEN header, HttpOnly cookie)
6. Authorization (empty — policies added dynamically below)
7. Dynamic Permission Policies (reads all Permissions from DB → AddPolicy per perm)
8. CORS (StrictCors: localhost:5000,5001, AllowCredentials)
9. Swagger (JWT Bearer security definition)
10. DI Registration (26 services/interfaces)
11. HttpContextAccessor
12. Controllers (AutoValidateAntiforgeryToken filter)
13. FluentValidation (auto-validation, no DataAnnotations)
14. AutoMapper
─── Pipeline ───
15. Swagger UI (dev only)
16. HttpsRedirection
17. Routing
18. CORS
19. Authentication
20. Authorization
21. MapControllers
22. Startup Seeders:
    a. RoleSeeder → Identity roles
    b. RoleSyncService → custom Roles table
    c. PermissionSeeder → permissions from AppPermissions
    d. RolePermissionSeeder → role-permission assignments
23. app.Run()
```

---

## Namespace Map

| Namespace | Location | Contents |
|-----------|----------|----------|
| `sicoain.api` | Root | `Program.cs` |
| `sicoain.api.Abstractions` | `Abstractions/` | 26 interface contracts |
| `sicoain.api.Controllers` | `Controllers/` | 19 API controllers |
| `sicoain.api.Data` | `Data/` | `ApplicationDbContext` |
| `sicoain.api.Data.Seeders` | `Data/Seeders/` | 3 static seeders |
| `sicoain.api.Mappings` | `Mappings/` | `UserProfile` AutoMapper profile |
| `sicoain.api.Repositories` | `Repositories/` | `RefreshTokenRepository` |
| `sicoain.api.Services` | `Services/` | 24 service implementations |
| `sicoain.api.Validators` | `Validators/` | Root validators (Login, BaseEmail, BasePhone) |
| `sicoain.api.Validators.Accidents` | `Validators/Accidents/` | Accident request validators |
| `sicoain.api.Validators.AccidentTypes` | `Validators/AccidentTypes/` | AccidentType request validators |
| `sicoain.api.Validators.Attachments` | `Validators/Attachments/` | Attachment request validators |
| `sicoain.api.Validators.Businesses` | `Validators/Businesses/` | Business request validators |
| `sicoain.api.Validators.CorrectiveActions` | `Validators/CorrectiveActions/` | CorrectiveAction validators |
| `sicoain.api.Validators.Departments` | `Validators/Departments/` | Department validators |
| `sicoain.api.Validators.DigitalEvidences` | `Validators/DigitalEvidences/` | DigitalEvidence validators |
| `sicoain.api.Validators.Employees` | `Validators/Employees/` | Employee validators |
| `sicoain.api.Validators.EventCategories` | `Validators/EventCategories/` | EventCategory validators |
| `sicoain.api.Validators.HealthPromotionEntities` | `Validators/HealthPromotionEntities/` | HPE validators |
| `sicoain.api.Validators.OccupationalRiskAdministrators` | `Validators/OccupationalRiskAdministrators/` | ORA validators |
| `sicoain.api.Validators.Positions` | `Validators/Positions/` | Position validators |
| `sicoain.api.Validators.RiskClasses` | `Validators/RiskClasses/` | RiskClass validators |
| `sicoain.api.Validators.Users` | `Validators/Users/` | User request validators |
| `sicoain.api.Validators.Witnesses` | `Validators/Witnesses/` | Witness validators |
