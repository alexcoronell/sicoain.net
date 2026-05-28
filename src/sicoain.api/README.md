# sicoain.api

> **RESTful Web API for SICOAIN — Sistema de Control de Accidentes e Incidentes**
> A production-grade ASP.NET Core Web API providing secure, permission-based access to occupational accident and incident control data, built for Colombian labor regulations.

[![.NET](https://img.shields.io/badge/.NET-10.0.7-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com)
[![C#](https://img.shields.io/badge/C%23-13-239120?style=flat-square&logo=csharp)](https://learn.microsoft.com/en-us/dotnet/csharp)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-10.0.7-512BD4?style=flat-square&logo=dotnet)](https://learn.microsoft.com/en-us/aspnet/core)
[![EF Core](https://img.shields.io/badge/EF%20Core-10.0.7-512BD4?style=flat-square&logo=dotnet)](https://learn.microsoft.com/en-us/ef/core)
[![JWT](https://img.shields.io/badge/Auth-JWT%20Bearer-000000?style=flat-square&logo=jsonwebtokens)](https://jwt.io)
[![Swagger](https://img.shields.io/badge/Swagger-7.0.0-85EA2D?style=flat-square&logo=swagger)](https://swashbuckle.AspNetCore)
[![FluentValidation](https://img.shields.io/badge/FluentValidation-12.1.1-E83533?style=flat-square)](https://docs.fluentvalidation.net)
[![AutoMapper](https://img.shields.io/badge/AutoMapper-12.0.1-8B89CC?style=flat-square)](https://automapper.org)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](LICENSE)

---

## Table of Contents

- [Overview](#overview)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Architecture](#architecture)
  - [Layered Architecture](#layered-architecture)
  - [Generic CRUD Pattern](#generic-crud-pattern)
  - [Request Pipeline](#request-pipeline)
- [Authentication & Authorization](#authentication--authorization)
  - [JWT with HttpOnly Cookies](#jwt-with-httponly-cookies)
  - [Refresh Token Rotation](#refresh-token-rotation)
  - [Dynamic Permission-Based Policies](#dynamic-permission-based-policies)
  - [RBAC Role Mapping](#rbac-role-mapping)
- [API Endpoints](#api-endpoints)
  - [Authentication](#authentication-endpoints)
  - [Core Business Entities](#core-business-entities)
  - [User Management](#user-management)
- [Security](#security)
  - [CSRF Protection](#csrf-protection)
  - [CORS Policy](#cors-policy)
  - [Password Policy](#password-policy)
  - [Account Lockout](#account-lockout)
  - [Static Code Analysis](#static-code-analysis)
- [Data Layer](#data-layer)
  - [Database Context](#database-context)
  - [Identity Table Mapping](#identity-table-mapping)
  - [Seeders](#seeders)
- [Validation Layer](#validation-layer)
- [Usage Examples](#usage-examples)
  - [Authentication Flow](#authentication-flow)
  - [CRUD Operations](#crud-operations)
  - [Pagination](#pagination)
- [Design Decisions](#design-decisions)
- [Configuration](#configuration)
- [Contributing](#contributing)
- [License](#license)

---

## Overview

**sicoain.api** is the RESTful API backend of the sicoain.net ecosystem, serving as the authoritative data layer for occupational accident and incident control. It exposes a versioned HTTP API consumed by the Blazor WebAssembly client (`sicoain.client`) and integrates with `sicoain.shared` for domain entities and DTOs.

### Key Capabilities

- **Complete CRUD** for 17 business entities via a generic controller/service pattern
- **Secure authentication** with JWT access tokens (15 min) stored in HttpOnly cookies + refresh tokens (7 days) with rotation
- **Dynamic authorization** — permission-based policies (`Accidents.View`, `Users.Create`, etc.) loaded from the database at startup
- **FluentValidation** integration for all incoming requests
- **Soft delete** support across all entities (reflected in both the domain model and the generic service layer)
- **Pagination** via `PagedResponse<T>` across all list endpoints
- **API versioning** (v1) via URL segment

---

## Technology Stack

| Category | Technology | Version | Purpose |
|----------|-----------|---------|---------|
| **Framework** | ASP.NET Core | `10.0.7` | Web API host |
| **Runtime** | .NET | `10.0` | Execution environment |
| **Language** | C# | `13` | Application code |
| **ORM** | Entity Framework Core | `10.0.7` | Data access (SQL Server) |
| **Identity** | ASP.NET Core Identity | `10.0.7` | User management, password hashing, lockout |
| **Authentication** | JWT Bearer | `10.0.7` | Token-based auth |
| **Mapping** | AutoMapper | `12.0.1` | Entity ↔ DTO mapping |
| **Validation** | FluentValidation | `12.1.1` | Request validation |
| **Validation ASP.NET** | FluentValidation.AspNetCore | `11.3.1` | Automatic validation pipeline |
| **API Versioning** | Asp.Versioning.Mvc | `10.0.0` | URL-based versioning |
| **Swagger** | Swashbuckle.AspNetCore | `7.0.0` | OpenAPI documentation |
| **Static Analysis** | Meziantou.Analyzer | `3.0.69` | .NET best practices analyzer |
| **Security Analysis** | SecurityCodeScan.VS2019 | `5.6.7` | Security vulnerability scanner |
| **Code Analysis** | Microsoft.CodeAnalysis.NetAnalyzers | `10.0.203` | Built-in Roslyn analyzers |
| **Design-time EF** | Microsoft.EntityFrameworkCore.Design | `10.0.7` | Migrations tooling |
| **EF Tools** | Microsoft.EntityFrameworkCore.Tools | `10.0.7` | CLI tooling |
| **Nullability** | `<Nullable>enable</Nullable>` | — | Compile-time null safety |
| **Code Style** | `<EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>` | — | Build-time style enforcement |

---

## Project Structure

```
src/sicoain.api/
├── Abstractions/                     # Interface contracts (26 files)
│   ├── IBaseService.cs              # Generic CRUD contract
│   ├── IAccidentService.cs          # Accident-specific extensions
│   ├── IUserService.cs              # User-specific extensions
│   ├── IAuthService.cs              # Authentication contract
│   ├── IJwtTokenGenerator.cs        # JWT generation contract
│   ├── IRefreshTokenGenerator.cs    # Refresh token generation
│   ├── IRefreshTokenRepository.cs   # Token persistence contract
│   ├── ICookieManager.cs            # HttpOnly cookie operations
│   ├── IIpAddressProvider.cs        # Client IP resolution
│   ├── IPermissionService.cs        # Permission resolution
│   ├── IRoleSyncService.cs          # Identity ↔ CustomRoles sync
│   ├── IAuthenticationProvider.cs   # Auth abstraction
│   └── 14 entity-specific service interfaces
├── Controllers/                      # API endpoints (19 files)
│   ├── BaseApiController.cs         # [ApiController], /api/v1/[controller]
│   ├── BaseCrudController.cs        # Generic CRUD with auth
│   ├── AuthController.cs            # /api/v1/Auth (login/refresh/logout/me)
│   ├── UserController.cs            # /api/v1/User (full user management)
│   ├── AccidentsController.cs       # /api/v1/Accidents
│   ├── EmployeesController.cs       # /api/v1/Employees
│   ├── BusinessesController.cs      # /api/v1/Businesses
│   ├── BranchesController.cs        # /api/v1/Branches
│   └── 11 additional CRUD controllers
├── Services/                         # Business logic (24 files)
│   ├── BaseService.cs               # Generic CRUD implementation
│   ├── AuthService.cs               # Authentication logic
│   ├── UserService.cs               # User management logic
│   ├── JwtTokenGenerator.cs         # JWT creation
│   ├── RefreshTokenGenerator.cs     # Cryptographic token generation
│   ├── CookieManager.cs             # Cookie abstraction
│   ├── IpAddressProvider.cs         # X-Forwarded-For aware IP resolution
│   ├── RoleSyncService.cs           # Identity ↔ CustomRoles sync
│   ├── PermissionService.cs         # User → Roles → Permissions resolution
│   └── 15 entity-specific CRUD services
├── Repositories/                     # Data access (1 file)
│   └── RefreshTokenRepository.cs    # Token persistence
├── Validators/                       # FluentValidation rules (40+ files)
│   ├── Users/
│   ├── Accidents/
│   ├── Employees/
│   ├── Businesses/
│   ├── Branches/
│   ├── ... (one directory per entity)
│   ├── LoginRequestValidator.cs
│   └── BaseCreateEntityEmailRequestValidator.cs
├── Data/
│   ├── ApplicationDbContext.cs      # EF Core context
│   └── Seeders/
│       ├── RoleSeeder.cs            # Creates Identity roles
│       ├── PermissionSeeder.cs      # Seeds AppPermissions from constants
│       └── RolePermissionSeeder.cs  # Maps roles ↔ permissions
├── Mappings/
│   └── UserProfile.cs               # AutoMapper profile
├── Migrations/                       # EF Core migrations
│   └── InitialCreate.cs
├── Constants/                        # (Planned) API-level constants
├── Properties/
│   └── launchSettings.json          # Dev server URLs
├── Program.cs                        # Host builder, DI, middleware pipeline
├── appsettings.json                  # JWT settings, logging
├── appsettings.Development.json      # Connection string, debug logging
└── sicoain.api.csproj               # Project file
```

**~130 source files** across 9 top-level directories.

---

## Architecture

### Layered Architecture

```
┌──────────────────────────────────────────────────────────────┐
│                    HTTP / HTTPS (TLS)                         │
├──────────────────────────────────────────────────────────────┤
│                        Controllers                           │
│     [Authorize(Policy = "Accidents.View")]                   │
│     Route: /api/v1/[controller]                              │
├──────────────────────────────────────────────────────────────┤
│                         Services                             │
│     BaseService<T>  │  AuthService  │  UserService  │  ...   │
├──────────────────────────────────────────────────────────────┤
│                    Abstractions (Interfaces)                  │
│     IBaseService<T>  │  IAuthService  │  IUserService        │
├──────────────────────────────────────────────────────────────┤
│                     Repositories                             │
│     RefreshTokenRepository                                   │
├──────────────────────────────────────────────────────────────┤
│                   Data (EF Core + Identity)                  │
│     ApplicationDbContext : IdentityDbContext<User, ..>       │
│     SQL Server (via Microsoft.Data.SqlClient)                │
└──────────────────────────────────────────────────────────────┘
```

### Generic CRUD Pattern

The API uses a **generic CRUD template** to eliminate boilerplate:

1. **`IBaseService<TDto, TCreateRequest, TUpdateRequest>`** — defines `GetAllAsync`, `GetByIdAsync`, `CreateAsync`, `UpdateAsync`, `DeleteAsync`.
2. **`BaseService<TEntity, TDto, TCreateRequest, TUpdateRequest>`** — implements CRUD using AutoMapper for entity ↔ DTO mapping, with reflection-based soft delete detection.
3. **`BaseCrudController<TDto, TCreateRequest, TUpdateRequest>`** — exposes RESTful endpoints (`GET`, `GET/{id}`, `POST`, `PATCH/{id}`, `DELETE/{id}`) with `[Authorize]` at the class level.

Concrete controllers override endpoints to add specific `[Authorize(Policy = "...")]` attributes. For example:

```
BaseCrudController<TDto, TCReq, TCRes>        (abstract, [Authorize])
└── AccidentsController                        (concrete)
    ├── GET  /api/v1/Accidents         → [Authorize(Policy = "Accidents.View")]
    ├── POST /api/v1/Accidents         → [Authorize(Policy = "Accidents.Create")]
    ├── PATCH /api/v1/Accidents/{id}   → [Authorize(Policy = "Accidents.Edit")]
    └── DELETE /api/v1/Accidents/{id}  → [Authorize(Policy = "Accidents.Delete")]
```

This pattern yields **15 entity controllers** in ~50 lines each instead of ~150.

### Request Pipeline

```
HTTP Request
    │
    ▼
┌──────────────────────┐
│  HTTPS Redirection   │
└──────────────────────┘
    │
    ▼
┌──────────────────────┐
│  CORS (StrictCors)   │
└──────────────────────┘
    │
    ▼
┌──────────────────────┐
│  Authentication      │
│  (JWT Bearer from    │
│   HttpOnly cookie)   │
└──────────────────────┘
    │
    ▼
┌──────────────────────┐
│  Authorization       │
│  (Permission policy) │
└──────────────────────┘
    │
    ▼
┌──────────────────────┐
│  Antiforgery         │
│  (CSRF token valid.) │
└──────────────────────┘
    │
    ▼
┌──────────────────────┐
│  Controller Action   │
│  ↓                   │
│  FluentValidation    │
│  (auto-validation)   │
│  ↓                   │
│  Service Layer       │
│  ↓                   │
│  EF Core / Identity  │
│  ↓                   │
│  SQL Server          │
└──────────────────────┘
    │
    ▼
HTTP Response
```

---

## Authentication & Authorization

### JWT with HttpOnly Cookies

Unlike typical JWTs sent via `Authorization: Bearer` header, this API reads tokens from **HttpOnly cookies**:

```
Set-Cookie: access_token=eyJhbGci...; HttpOnly; Secure; SameSite=Strict
```

The JWT middleware is configured to read from cookies (not headers):

```csharp
options.Events = new JwtBearerEvents
{
    OnMessageReceived = context =>
    {
        context.Token = context.Request.Cookies["access_token"];
        return Task.CompletedTask;
    }
};
```

**Why cookies instead of headers?**

| Aspect | HttpOnly Cookie | Authorization Header |
|--------|----------------|---------------------|
| XSS protection | ✅ Immune (JS can't read) | ❌ Vulnerable if stored in localStorage |
| CSRF protection | Requires antiforgery token | ✅ Immune (not automatic) |
| Mobile app support | Requires cookie management | ✅ Simpler |
| Token expiration | Managed by server | Client must handle |

### Refresh Token Rotation

The API implements **OWASP-recommended refresh token rotation**:

```
Login                     Refresh                    Logout
  │                         │                         │
  ▼                         ▼                         ▼
┌─────────────────────────────────────────────────────────────┐
│  access_token  (JWT, 15 min, HttpOnly)                       │
│  refresh_token (64-byte random, 7 days, HttpOnly)            │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  On refresh:                                                  │
│    1. Receive refresh_token from cookie                      │
│    2. Look up in database (RefreshTokens table)              │
│    3. Check IsActive (not revoked AND not expired)           │
│    4. REVOKE old token (set RevokedAt, RevokedByIp)         │
│    5. Generate NEW access_token + refresh_token              │
│    6. Store new refresh token with ReplacedByTokenId         │
│    7. Set new cookies                                        │
│                                                              │
│  Reuse detection:                                             │
│    If a revoked token is presented → possible theft          │
│    → Revoke ALL tokens for that user                         │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

Key implementation details:

- **Refresh tokens** are 64-byte cryptographically random values (via `RandomNumberGenerator.GetBytes(64)`), base64-encoded.
- **Stored in database** (not JWT) — enables server-side revocation.
- **Revocation tracking** records IP address and reason.
- **Token chain** via `ReplacedByTokenId` enables forensic audit of token usage.

### Dynamic Permission-Based Policies

Authorization policies are **loaded from the database at startup** — not hardcoded:

```csharp
// In Program.cs startup:
var permissions = dbContext.Permissions.Select(p => p.Name).ToList();
foreach (var perm in permissions)
{
    builder.Services.AddAuthorizationBuilder()
        .AddPolicy(perm, policy => policy.RequireClaim("Permission", perm));
}
```

When a user logs in, the `PermissionService` resolves their effective permissions:

```
User → UserManager.GetRolesAsync(user) → Identity Role names
  → Roles table (custom) → RoleIds
    → RolePermissions → Permissions → Permission names
      → Added as JWT claims: "Permission": "Accidents.View"
```

Each endpoint then enforces policies via attributes:

```csharp
[Authorize(Policy = "Accidents.Create")]
public override async Task<ActionResult<AccidentDto>> Create(...)
```

### RBAC Role Mapping

The API defines 4 seed roles with granular permissions:

| Role | Permissions | Typical User |
|------|------------|--------------|
| **Admin** | ALL permissions (full system access) | System administrator |
| **Investigator** | `Accidents.View/Create/Edit`, `Reports.View`, `Employees.View` | Accident investigator |
| **Supervisor** | `Accidents.View`, `Employees.View` | Shift supervisor |
| **Consultant** | `Reports.View`, `Accidents.View` | External consultant |

The `RoleSyncService` runs at startup to synchronize Identity roles (`AspNetRoles`) with the custom `Roles` table, ensuring consistency between the two role stores.

---

## API Endpoints

All endpoints are prefixed with `/api/v1/`.

### Authentication Endpoints

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| `POST` | `/Auth/login` | Anonymous | Authenticate with email + password. Sets `access_token` + `refresh_token` cookies. |
| `POST` | `/Auth/refresh` | Anonymous | Refresh expired access token using valid refresh token from cookie. |
| `POST` | `/Auth/logout` | Authorized | Revoke refresh token, clear cookies. |
| `POST` | `/Auth/me` | Authorized | Get current authenticated user's info. |

**Login response:**

```json
{
  "success": true,
  "message": "Login successful",
  "email": "juan.perez@empresa.com",
  "fullName": "Juan Pérez",
  "expiresAt": "2026-05-27T15:00:00Z"
}
```

### Core Business Entities

Each entity follows the same endpoint pattern:

| Method | Route | Auth Policy | Description |
|--------|-------|-------------|-------------|
| `GET` | `/{entity}` | `{entity}.View` | Paginated list (query params: `pageNumber`, `pageSize`) |
| `GET` | `/{entity}/{id}` | `{entity}.View` | Get by ID |
| `POST` | `/{entity}` | `{entity}.Create` | Create new entity |
| `PATCH` | `/{entity}/{id}` | `{entity}.Edit` | Partial update |
| `DELETE` | `/{entity}/{id}` | `{entity}.Delete` | Soft delete |

**Available entities:**

| Endpoint | DTO | Notes |
|----------|-----|-------|
| `/Accidents` | `AccidentDto` | Includes `EmployeeFullname`, `AccidentTypeName`, `EventCategoryName` |
| `/AccidentTypes` | `AccidentTypeDto` | — |
| `/Attachments` | `AttachmentDto` | — |
| `/Branches` | `BranchDto` | — |
| `/Businesses` | `BusinessDto` | — |
| `/CorrectiveActions` | `CorrectiveActionDto` | — |
| `/Departments` | `DepartmentDto` | — |
| `/DigitalEvidences` | `DigitalEvidenceDto` | — |
| `/Employees` | `EmployeeDto` | Full join: Business, Branch, Position, Department names |
| `/EventCategories` | `EventCategoryDto` | — |
| `/HealthPromotionEntities` | `HealthPromotionEntityDto` | EPS entities |
| `/OccupationalRiskAdministrators` | `OccupationalRiskAdministratorDto` | ARL entities |
| `/Positions` | `PositionDto` | — |
| `/RiskClasses` | `RiskClassDto` | — |
| `/Witnesses` | `WitnessDto` | — |

### User Management

| Method | Route | Auth Policy | Description |
|--------|-------|-------------|-------------|
| `GET` | `/User` | `Users.View` | Paginated user list (excludes soft-deleted) |
| `GET` | `/User/{id}` | `Users.View` | Get user by ID |
| `GET` | `/User/email/{email}` | `Users.View` | Get user by email |
| `GET` | `/User/email-exists/{email}` | `Users.View` | Check if email exists |
| `GET` | `/User/roles/{id}` | `Users.View` | Get user's assigned roles |
| `POST` | `/User` | `Users.Create` | Create user (with optional role assignment) |
| `PATCH` | `/User/{id}` | `Users.Edit` | Update user fields |
| `PATCH` | `/User/assign-role/{id}` | `Users.Edit` | Assign role to user |
| `PATCH` | `/User/remove-role/{id}` | `Users.Edit` | Remove role from user |
| `PATCH` | `/User/change-password/{id}` | `Users.Edit` | Change user password (requires current password) |
| `DELETE` | `/User/{id}` | `Users.Delete` | Soft delete user |

---

## Security

### CSRF Protection

The API mitigates Cross-Site Request Forgery via ASP.NET Core's antiforgery framework:

```csharp
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.Name = "csrf_token";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// Auto-validate on all POST/PATCH/DELETE
services.AddControllers(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});
```

All mutating requests (`POST`, `PATCH`, `DELETE`) must include the `X-CSRF-TOKEN` header, validated against the `csrf_token` cookie.

### CORS Policy

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("StrictCors", policy =>
    {
        policy.WithOrigins("http://localhost:5000", "http://localhost:5001")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

Only `localhost:5000` and `localhost:5001` (the Blazor client) are allowed. `AllowCredentials` is required for cookie-based authentication.

### Password Policy

| Setting | Value |
|---------|-------|
| Minimum length | 8 characters |
| Requires digit | ✅ Yes |
| Requires uppercase | ✅ Yes |
| Requires lowercase | ✅ Yes |
| Requires non-alphanumeric | ✅ Yes |
| Unique email | ✅ Required |

### Account Lockout

| Setting | Value |
|---------|-------|
| Lockout duration | 15 minutes |
| Max failed attempts | 5 |
| Lockout for new users | Enabled |

### Static Code Analysis

Three analyzer packages enforce code quality and security at build time:

- **Meziantou.Analyzer** (`3.0.69`) — Best practices for .NET (async, disposal, threading)
- **SecurityCodeScan.VS2019** (`5.6.7`) — OWASP vulnerability detection (SQL injection, XSS, CSRF)
- **Microsoft.CodeAnalysis.NetAnalyzers** (`10.0.203`) — CA rules enforced via `<AnalysisMode>All</AnalysisMode>`

---

## Data Layer

### Database Context

`ApplicationDbContext` extends `IdentityDbContext<User, IdentityRole<int>, int>`, providing both Identity tables and business entity tables in a single database.

**Entity categories:**

```csharp
// Core Business
DbSet<Employee> Employees
DbSet<Accident> Accidents
DbSet<Witness> Witnesses
DbSet<CorrectiveAction> CorrectiveActions
DbSet<CorrectiveActionTracking> CorrectiveActionTrackings

// Files & Evidence
DbSet<Attachment> Attachments
DbSet<DigitalEvidence> DigitalEvidences

// Organizational Structure
DbSet<Business> Businesses
DbSet<Branch> Branches
DbSet<Department> Departments
DbSet<Position> Positions

// Catalogs
DbSet<EventCategory> EventCategories
DbSet<AccidentType> AccidentTypes
DbSet<RiskClass> RiskClasses

// External Entities
DbSet<OccupationalRiskAdministrator> OccupationalRiskAdministrators
DbSet<HealthPromotionEntity> HealthPromotionEntities

// Custom RBAC
DbSet<Roles> CustomRoles
DbSet<Permissions> Permissions
DbSet<RolePermissions> RolePermissions

// Normalized Contacts
DbSet<BusinessEmail> BusinessEmails
DbSet<BusinessPhone> BusinessPhones
DbSet<BranchEmail> BranchEmails
DbSet<BranchPhone> BranchPhones
DbSet<EmployeeEmail> EmployeeEmails
DbSet<EmployeePhone> EmployeePhones
DbSet<OccupationalRiskAdministratorEmail> OcupationalRiskAdministratorEmails
DbSet<OccupationalRiskAdministratorPhone> OcupationalRiskAdministratorPhones
DbSet<HealthPromotionEntityEmail> HealthPromotionEntityEmails
DbSet<HealthPromotionEntityPhone> HealthPromotionEntityPhones

// Auth
DbSet<RefreshToken> RefreshTokens
```

### Identity Table Mapping

ASP.NET Core Identity's default table names (`AspNetUsers`, `AspNetRoles`, etc.) are renamed to cleaner names:

| Default Name | Custom Name |
|-------------|-------------|
| `AspNetUsers` | `Users` |
| `AspNetRoles` | `Roles` |
| `AspNetUserRoles` | `UserRoles` |
| `AspNetUserClaims` | `UserClaims` |
| `AspNetUserLogins` | `UserLogins` |
| `AspNetUserTokens` | `UserTokens` |
| `AspNetRoleClaims` | `RoleClaims` |

### Seeders

Three seeders run at application startup:

```csharp
// 1. Create Identity roles if they don't exist
await RoleSeeder.SeedAsync(services);
// Roles: Admin, Investigator, Supervisor, Consultant

// 2. Synchronize Identity roles → CustomRoles table
await roleSyncService.SynchronizeRoleAsync();

// 3. Seed permissions from AppPermissions constants
await PermissionSeeder.SeedAsync(dbContext);
// Uses reflection to read all const string fields from AppPermissions
// Automatically derives Module ("Accidents") and Action ("View") from name

// 4. Assign permissions to roles
await RolePermissionSeeder.SeedAsync(dbContext);
// Admin → all permissions
// Investigator → Accidents.View/Create/Edit, Reports.View, Employees.View
// Supervisor → Accidents.View, Employees.View
// Consultant → Reports.View, Accidents.View
```

---

## Validation Layer

All incoming requests are validated via **FluentValidation** integrated into the ASP.NET Core pipeline:

```csharp
builder.Services.AddFluentValidationAutoValidation(options =>
{
    options.DisableDataAnnotationsValidation = true;  // FluentValidation takes priority
});
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
```

The validator directory mirrors the entity structure with specialized validators for each operation:

```
Validators/
├── Users/
│   ├── CreateUserRequestValidator.cs
│   ├── UpdateUserRequestValidator.cs
│   ├── ChangePasswordRequestValidator.cs
│   └── AssignOrRemoveRoleRequestValidator.cs
├── Accidents/
│   ├── CreateAccidentRequestValidator.cs
│   └── UpdateAccidentRequestValidator.cs
├── Employees/
│   ├── CreateEmployeeRequestValidator.cs
│   └── UpdateEmployeeRequestValidator.cs
├── Businesses/
│   ├── CreateBusinessRequestValidator.cs
│   └── UpdateBusinessRequestValidator.cs
├── Branches/
│   ├── CreateBranchRequestValidator.cs
│   └── UpdateBranchRequestValidator.cs
... (one directory per entity)
├── LoginRequestValidator.cs
├── BaseCreateEntityEmailRequestValidator.cs
└── BaseCreateEntityPhoneRequestValidator.cs
```

---

## Usage Examples

### Authentication Flow

**Login:**

```bash
curl -X POST https://localhost:7241/api/v1/Auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "admin@sicoain.net", "password": "SecurePass123!"}' \
  -c cookies.txt
```

**Authenticated request (cookie included):**

```bash
curl -X GET https://localhost:7241/api/v1/Accidents?pageNumber=1&pageSize=10 \
  -b cookies.txt \
  -H "X-CSRF-TOKEN: <csrf_token_value>"
```

**Refresh token:**

```bash
curl -X POST https://localhost:7241/api/v1/Auth/refresh \
  -b cookies.txt -c cookies.txt
```

**Logout:**

```bash
curl -X POST https://localhost:7241/api/v1/Auth/logout \
  -b cookies.txt \
  -H "X-CSRF-TOKEN: <csrf_token_value>"
```

### CRUD Operations

**Create an Accident:**

```bash
curl -X POST https://localhost:7241/api/v1/Accidents \
  -b cookies.txt \
  -H "Content-Type: application/json" \
  -H "X-CSRF-TOKEN: <csrf_token_value>" \
  -d '{
    "eventDate": "2026-05-27T10:30:00Z",
    "description": "Caída desde escalera durante mantenimiento",
    "employeeId": 42,
    "accidentTypeId": 3,
    "eventCategoryId": 1
  }'
```

**Response (201 Created):**

```json
{
  "id": 128,
  "eventDate": "2026-05-27T10:30:00Z",
  "description": "Caída desde escalera durante mantenimiento",
  "employeeId": 42,
  "employeeFullname": "Juan Pérez",
  "accidentTypeId": 3,
  "accidentTypeName": "Caída desde altura",
  "eventCategoryId": 1,
  "eventCategoryName": "Accidente Laboral",
  "createdAt": "2026-05-27T14:32:00Z",
  "createdBy": 1
}
```

**Update (PATCH):**

```bash
curl -X PATCH https://localhost:7241/api/v1/Accidents/128 \
  -b cookies.txt \
  -H "Content-Type: application/json" \
  -H "X-CSRF-TOKEN: <csrf_token_value>" \
  -d '{
    "description": "Caída desde escalera durante mantenimiento de luminarias - actualizado"
  }'
```

**Soft Delete:**

```bash
curl -X DELETE https://localhost:7241/api/v1/Accidents/128 \
  -b cookies.txt \
  -H "X-CSRF-TOKEN: <csrf_token_value>"
# Response: 204 No Content
```

### Pagination

All `GET` list endpoints support pagination:

```bash
curl -X GET "https://localhost:7241/api/v1/Employees?pageNumber=2&pageSize=25" \
  -b cookies.txt
```

**Response:**

```json
{
  "items": [ ... ],
  "totalCount": 142,
  "pageNumber": 2,
  "pageSize": 25,
  "totalPages": 6,
  "hasPreviousPage": true,
  "hasNextPage": true
}
```

---

## Design Decisions

### 1. Generic CRUD Template

`BaseService<TEntity, TDto, TCreateRequest, TUpdateRequest>` + `BaseCrudController<TDto, TCreateRequest, TUpdateRequest>` eliminates repetitive code. 15 entity endpoints are implemented with 0 duplicated CRUD logic. The pattern handles soft delete via reflection — if an entity has an `IsDeleted` property, `DeleteAsync` sets it to `true` instead of removing the row.

**Tradeoff:** Generic `GetAllAsync` does not `Include()` related entities — concrete services override it when join data is needed (e.g., `AccidentService`, `EmployeeService`).

### 2. JWT in HttpOnly Cookies

Standard practice sends JWTs via `Authorization: Bearer`. This API uses HttpOnly cookies to eliminate XSS-based token exfiltration (the JS running in the browser never sees the token). The tradeoff is that the API must implement CSRF protection (antiforgery tokens), which it does via the `X-CSRF-TOKEN` header.

### 3. Refresh Token in Database (not JWT)

Refresh tokens are stored in the `RefreshTokens` SQL Server table, not encoded as JWTs. This enables:

- Server-side revocation on demand
- Token family tracking (rotation chain via `ReplacedByTokenId`)
- Reuse detection (if a revoked token is used, revoke the entire family)
- Unlimited token size (JWT refresh tokens are limited by HTTP header size)

**Tradeoff:** Every refresh requires a database round-trip to look up the token.

### 4. Dynamic Permission Policies at Startup

Policies are loaded from the database once at startup. This means adding a new permission (e.g., `"Reports.Export"`) only requires adding the constant to `AppPermissions` — the seeder creates it, the startup registers the policy, and controllers can immediately use `[Authorize(Policy = "Reports.Export")]`.

**Tradeoff:** New permissions require an application restart to register. An alternative would be policy-based authorization with a custom `IAuthorizationPolicyProvider`, but that adds complexity without meaningful benefit for this use case.

### 5. Soft Delete via Reflection

`BaseService.DeleteAsync` uses reflection to check for an `IsDeleted` property:

```csharp
var property = entity.GetType().GetProperty("IsDeleted");
if (property != null && property.PropertyType == typeof(bool))
    property.SetValue(entity, true);
```

This allows the generic service to handle soft delete without requiring all entities to implement a specific interface. The performance cost of reflection is negligible for single-entity operations.

---

## Configuration

### appsettings.json

```json
{
  "JwtSettings": {
    "SecretKey": "your-256-bit-secret-key-here",
    "Issuer": "SICOAIN-API",
    "Audience": "SICOAIN-Frontend",
    "ExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  }
}
```

| Setting | Description | Default |
|---------|-------------|---------|
| `SecretKey` | HMAC-SHA256 signing key (min 32 chars) | Required |
| `Issuer` | JWT `iss` claim | `SICOAIN-API` |
| `Audience` | JWT `aud` claim | `SICOAIN-Frontend` |
| `ExpirationMinutes` | Access token lifetime | 60 |
| `RefreshTokenExpirationDays` | Refresh token lifetime | 7 |

### Launch Profiles

| Profile | URLs | Environment |
|---------|------|-------------|
| `http` | `http://localhost:5078` | Development |
| `https` | `https://localhost:7241;http://localhost:5078` | Development |

---

## Contributing

Contributions are welcome. This project follows a **feature branch workflow**:

1. Fork the repository.
2. Create a feature branch: `git checkout -b feat/your-feature`.
3. Make your changes — maintain the existing patterns (generic CRUD, cookie auth, FluentValidation).
4. Ensure all tests pass: `dotnet test`.
5. Push and open a Pull Request.

**Guidelines:**

- Do NOT add `Co-Authored-By` or AI attribution to commits.
- Use [conventional commits](https://www.conventionalcommits.org/) (`feat:`, `fix:`, `refactor:`, `docs:`, `test:`, `chore:`).
- New entity endpoints should follow the `BaseCrudController<T>` pattern.
- Always add FluentValidation validators for new request DTOs.
- Add `[Authorize(Policy = ...)]` with the appropriate permission constant.

---

## License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE) file for details.

---

<div align="center">

**Built with 🔥 by [sicoain.net](https://sicoain.net)**

*Comprometidos con la seguridad y salud en el trabajo en Colombia 🇨🇴*

</div>
