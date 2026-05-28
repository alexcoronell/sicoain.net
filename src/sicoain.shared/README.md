# sicoain.shared

> **Domain-driven shared library for SICOAIN — Sistema de Control de Accidentes e Incidentes**
> A .NET class library providing entities, DTOs, enumerations, and constants for building occupational accident and incident control systems compliant with **Colombian labor regulations**.

[![.NET](https://img.shields.io/badge/.NET-10.0.7-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com)
[![C#](https://img.shields.io/badge/C%23-13-239120?style=flat-square&logo=csharp)](https://learn.microsoft.com/en-us/dotnet/csharp)
[![ASP.NET Core Identity](https://img.shields.io/badge/Identity-10.0.7-512BD4?style=flat-square&logo=dotnet)](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity)
[![EF Core](https://img.shields.io/badge/EF%20Core-10.0.7-512BD4?style=flat-square&logo=dotnet)](https://learn.microsoft.com/en-us/ef/core)
[![Platform](https://img.shields.io/badge/Platform-.NET%20Standard%202.0%20%7C%20.NET%2010-blue?style=flat-square)](https://dotnet.microsoft.com)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](LICENSE)

---

## Table of Contents

- [Overview](#overview)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Domain Model Architecture](#domain-model-architecture)
  - [Base Entity Hierarchy](#base-entity-hierarchy)
  - [Organizational Core](#organizational-core)
  - [Accident & Incident Management](#accident--incident-management)
  - [RBAC Security Model](#rbac-security-model)
  - [Authentication & Token Management](#authentication--token-management)
  - [Contact Information Pattern](#contact-information-pattern)
  - [Entity Relationship Diagram](#entity-relationship-diagram)
- [Enumerations](#enumerations)
- [DTO Layer](#dto-layer)
- [Security Constants](#security-constants)
- [Usage Examples](#usage-examples)
  - [Installing the Package](#installing-the-package)
  - [Defining a DbContext](#defining-a-dbcontext)
  - [Creating an Employee with Full Relationships](#creating-an-employee-with-full-relationships)
  - [Registering an Accident with Digital Evidence](#registering-an-accident-with-digital-evidence)
  - [Soft Delete & Audit Trail](#soft-delete--audit-trail)
  - [Querying with PagedResponse](#querying-with-pagedresponse)
  - [Authentication Flow](#authentication-flow)
  - [RBAC Permission Checking](#rbac-permission-checking)
- [Design Decisions & Patterns](#design-decisions--patterns)
  - [Why IdentityUser\<int\> Instead of string?](#why-identityuserint-instead-of-string)
  - [Polymorphic Contact Inheritance](#polymorphic-contact-inheritance)
  - [BaseDto as Record Type](#basedto-as-record-type)
  - [RefreshToken Rotation Design](#refreshtoken-rotation-design)
  - [Column Mapping Convention](#column-mapping-convention)
- [Contributing](#contributing)
- [License](#license)

---

## Overview

**sicoain.shared** is the shared domain layer of the sicoain.net ecosystem, designed to serve as the single source of truth for the occupational health and safety domain across multiple application tiers — API (`sicoain.api`), Blazor WebAssembly client (`sicoain.client`), and unit tests (`sicoain.UnitTests`).

The library encapsulates:

- **37 entity classes** covering the complete SG-SST domain model
- **6 enums** with Spanish-language `[Display]` annotations for direct UI binding
- **~100 DTO records** for request/response contracts
- **A complete RBAC system** with fine-grained module/action permissions
- **JWT refresh token rotation** infrastructure
- **Soft delete & audit trail** baked into every persistent entity

The domain model is tailored for Colombian regulatory entities — ARL (Administradoras de Riesgos Laborales), EPS (Entidades Promotoras de Salud), and Colombian identity document types — making it production-ready for the local market out of the box.

---

## Technology Stack

| Category | Technology | Version |
|----------|-----------|---------|
| **Runtime** | .NET | `net10.0` |
| **Language** | C# | `13` |
| **Identity Framework** | Microsoft.AspNetCore.Identity | `10.0.7` |
| **Identity Stores** | Microsoft.Extensions.Identity.Stores | `10.0.7` |
| **Annotations** | System.ComponentModel.Annotations | `5.0.0` |
| **Nullable analysis** | Enabled (`<Nullable>enable</Nullable>`) | — |
| **Implicit usings** | Enabled (`<ImplicitUsings>enable</ImplicitUsings>`) | — |

---

## Project Structure

```
src/sicoain.shared/
├── Constants/
│   └── AppPermissions.cs              # RBAC permission string constants
├── DTOs/                               # Request/Response contracts (records)
│   ├── Accidents/
│   ├── AccidentTypes/
│   ├── Attachments/
│   ├── Branches/
│   ├── Business/
│   ├── CorrectiveActions/
│   ├── CorrectiveActionTrackings/
│   ├── Departments/
│   ├── DigitalEvidences/
│   ├── EmployeeContacts/
│   ├── Employees/
│   ├── EventCategories/
│   ├── HealthPromotionEntities/
│   ├── OccupationalRiskAdministrators/
│   ├── Positions/
│   ├── RiskClasses/
│   ├── Users/
│   ├── Witnesses/
│   ├── AuthResponse.cs
│   ├── BaseDto.cs
│   ├── CreateEntityEmailRequest.cs
│   ├── CreateEntityPhoneRequest.cs
│   ├── LoginRequest.cs
│   ├── PagedResponse.cs
│   ├── RefreshTokenRequest.cs
│   ├── RevokeTokenRequest.cs
│   ├── UpdateEntityEmailRequest.cs
│   └── UpdateEntityPhoneRequest.cs
├── Entities/                           # Domain entities
│   ├── BaseEntity.cs                   # Abstract base — audit + soft delete
│   ├── BaseEntityEmail.cs             # Abstract — email contact template
│   ├── BaseEntityPhone.cs             # Abstract — phone contact template
│   ├── Accident.cs
│   ├── AccidentType.cs
│   ├── Attachment.cs
│   ├── Branch.cs
│   ├── BranchEmail.cs
│   ├── BranchPhone.cs
│   ├── Business.cs
│   ├── BusinessEmail.cs
│   ├── BusinessPhone.cs
│   ├── CorrectiveAction.cs
│   ├── CorrectiveActionTracking.cs
│   ├── Department.cs
│   ├── DigitalEvidence.cs
│   ├── Employee.cs
│   ├── EmployeeContact.cs
│   ├── EmployeeContactEmail.cs
│   ├── EmployeeContactPhone.cs
│   ├── EmployeeEmail.cs
│   ├── EmployeePhone.cs
│   ├── EventCategory.cs
│   ├── HealthPromotionEntity.cs
│   ├── HealthPromotionEntityEmail.cs
│   ├── HealthPromotionEntityPhone.cs
│   ├── OccupationalRiskAdministrator.cs
│   ├── OccupationalRiskAdministratorEmail.cs
│   ├── OccupationalRiskAdministratorPhone.cs
│   ├── Permissions.cs
│   ├── Position.cs
│   ├── RefreshToken.cs
│   ├── RiskClass.cs
│   ├── RolePermissions.cs
│   ├── Roles.cs
│   ├── User.cs
│   └── Witness.cs
├── Enums/                              # Enumerations (Spanish Display names)
│   ├── AccidentSeverity.cs
│   ├── AttachmentEntityType.cs
│   ├── DocumentType.cs
│   ├── PhoneType.cs
│   ├── Priority.cs
│   └── StatusAction.cs
├── Interfaces/                         # (Planned) Repository interfaces
└── sicoain.shared.csproj               # Project file
```

**78 source files** across 4 top-level directories, organized by architectural concern.

---

## Domain Model Architecture

### Base Entity Hierarchy

Every persistent entity (except `RefreshToken`) inherits from `BaseEntity`, which provides:

```
BaseEntity (abstract)
├── Id : int                           — Primary key (consistent int throughout)
├── CreatedAt : DateTime               — Auto-set to UtcNow
├── UpdatedAt : DateTime               — Updated on every modification
├── DeletedAt : DateTime?              — Set on soft delete
├── CreatedBy : int                    — User ID who created
├── UpdatedBy : int                    — User ID who last modified
├── DeletedBy : int?                   — User ID who deleted
├── IsDeleted : bool                   — Soft delete flag
├── UpdateTimestamps(userId)           — Sets UpdatedBy + UpdatedAt
├── MarkAsDeleted(userId)              — Sets DeletedBy, DeletedAt, IsDeleted
└── Restore()                          — Clears delete fields
```

All entities use `int` as the key type, consistent with `IdentityUser<int>`.

### Organizational Core

```
Business ──has many──▶ Branch ──has many──▶ Employee
  │                     │                     │
  │                     │               ┌─────┴──────────┬──────────────────┐
  │                     │               │                │                  │
  ▼                     ▼               ▼                ▼                  ▼
 BusinessPhone    BranchPhone       EmployeePhone   EmployeeContact    Accident
 BusinessEmail    BranchEmail       EmployeeEmail   ├─ EmployeeContactPhone
                                                    └─ EmployeeContactEmail
```

- **Business**: A company or organization. Root aggregate for organizational data.
- **Branch**: A physical location belonging to a Business.
- **Employee**: A worker with full Colombian identity support (`DocumentType`, `DocumentNumber`), medical info (`Diseases`, `Medications`, `Allergies`), and links to EPS/ARL.

**Regulatory entities** associated with each Employee:

| Entity | Colombian Role | Properties |
|--------|---------------|------------|
| `HealthPromotionEntity` | **EPS** (Entidad Promotora de Salud) | Name, Address, Phones, Emails |
| `OccupationalRiskAdministrator` | **ARL** (Administradora de Riesgos Laborales) | Name, Address, Phones, Emails |
| `Department` | **Departamento** (organizational unit) | Name, Description, Contact |
| `Position` | **Cargo** (job position) | Name, Department, RiskClass |
| `RiskClass` | **Clase de Riesgo** (OSHA risk class) | Name, Code, ContributionRate |

**RiskClass** includes a `ContributionRate` (`decimal(5,4)`) — a percentage used to calculate ARL premiums based on occupational risk level, a critical requirement for Colombian payroll compliance.

### Accident & Incident Management

```
Accident ──belongs to──▶ Employee
  │
  ├──has many──▶ DigitalEvidence    — File evidence with chain of custody
  ├──has many──▶ Witness            — People present at the incident
  │               └── optional link to Employee (if witness is an employee)
  └──has many──▶ CorrectiveAction
                   └──has many──▶ CorrectiveActionTracking  — Status history log
```

- **Accident**: Central incident record with `EventDate`, `Description`, linked to `Employee`, `AccidentType`, `EventCategory`.
- **AccidentType**: Classification with severity level.
- **EventCategory**: Categorization with severity threshold and hospitalization flag.
- **DigitalEvidence**: File tracking with `FileHash` (SHA-256), `MimeType`, `FileSize` (long), and `ChainOfCustody` text — forensically sound evidence chain.
- **Witness**: Can be an existing `Employee` (via `EmployeeId`) or an external person (via `WitnessName`/`WitnessContact`).
- **CorrectiveAction**: Remediation with `Status`, `Priority`, `DueDate`, `CompletionDate`, effectiveness tracking.
- **CorrectiveActionTracking**: Immutable status change log recording `OldStatus` → `NewStatus` transitions.

### RBAC Security Model

The authorization system follows a modular permission-based architecture:

```
Roles ──────◀── RolePermissions ──▶ Permissions
  │                                     │
  │ (via IdentityRoleId)                ├── Module : string
  │                                     ├── Action : string
  ▼                                     └── Description : string?
 Users
```

- **Permissions**: Granular `Module.Action` pairs (e.g., `"Accidents.View"`, `"Users.AssignRoles"`).
- **Roles**: Application roles with a link to `IdentityRole<int>` via `IdentityRoleId`.
- **RolePermissions**: Junction table — many-to-many between Roles and Permissions.

The `AppPermissions` constant class (see [Security Constants](#security-constants)) provides strongly-typed strings for runtime permission checks, preventing magic strings.

### Authentication & Token Management

```
User : IdentityUser<int>
 └──has many──▶ RefreshToken     — JWT refresh token with rotation chain
                   ├── Token        — Unique token value
                   ├── ExpiresAt    — Expiration date
                   ├── RevokedAt?   — Null if active
                   ├── RevokedByIp? — IP that triggered revocation
                   ├── ReplacedByTokenId? — Token rotation chain
                   └── IsActive (computed) — !IsRevoked && !IsExpired
```

The `RefreshToken` entity implements a **token rotation** pattern:
1. When a refresh token is used, the old token is revoked and a new one is issued.
2. `ReplacedByTokenId` tracks the replacement chain for forensic analysis.
3. `IsActive`, `IsExpired`, `IsRevoked` are `[NotMapped]` computed properties.
4. `Revoke(ipAddress, reason?)` encapsulates the revocation logic.

This design prevents replay attacks — a stolen refresh token can only be used once before rotation invalidates it.

### Contact Information Pattern

The library uses an **abstract base class inheritance** pattern for contact information, avoiding duplicating the same properties across every entity:

```
BaseEntityEmail (abstract)      BaseEntityPhone (abstract)
└── Email : string              └── Phone : string
                                └── PhoneType : PhoneType

Concrete implementations:
├── BusinessEmail               ├── BusinessPhone
├── BranchEmail                 ├── BranchPhone
├── EmployeeEmail               ├── EmployeePhone
├── EmployeeContactEmail        ├── EmployeeContactPhone
├── HealthPromotionEntityEmail  ├── HealthPromotionEntityPhone
└── OcupationalRiskAdmEmail     └── OcupationalRiskAdmPhone
```

This gives us **12 contact entity types** generated from just **2 abstract templates** — a textbook example of the Template Method pattern in domain modeling.

### Entity Relationship Diagram

```
┌───────────────┐       ┌──────────────────┐       ┌──────────────────────┐
│   Business    │1────N▶│     Branch        │1────N▶│     Employee         │
│               │       │                  │       │                      │
│ Phones (N)    │       │ Phones (N)       │       │ Phones (N)           │
│ Emails (N)    │       │ Emails (N)       │       │ Emails (N)           │
└───────────────┘       └──────────────────┘       │ Contacts (N)         │
                                                    │ Accidents (N)        │
       1                                             └──────┬───────────────┘
       │                                                    │
       │                                                    │ 1
       ▼                                                    ▼
┌───────────────┐                                  ┌──────────────────┐
│ Department    │1────N▶     Position              │Accident          │
│               │            │                     │                  │
└───────────────┘            │ RiskClass (N:1)     │ DigitalEvidence  │
                             │                     │ Witnesses (N)    │
                             │                     │ CorrectiveAction │
                             ▼                     └──────────────────┘
                      ┌──────────────┐
                      │  RiskClass   │              ┌──────────────────────┐
                      │              │              │  CorrectiveAction    │
                      │ Contribution │1────N▶        │                      │
                      │ Rate         │              │ Trackings (N)        │
                      └──────────────┘              │ Status / Priority    │
                                                    └──────────────────────┘
Employee ─N:1──▶ HealthPromotionEntity (EPS)
Employee ─N:1──▶ OccupationalRiskAdministrator (ARL)

RBAC:
Roles ──N:M──▶ Permissions  (via RolePermissions)
Users ──N:1──▶ Roles        (via IdentityRoleId)
User  ──1:N──▶ RefreshToken (token rotation chain)
```

---

## Enumerations

All enums use `System.ComponentModel.DataAnnotations.Display` with Spanish-language names for direct UI binding (e.g., in Blazor components or Swagger documentation).

| Enum | Values | Business Context |
|------|--------|-----------------|
| **DocumentType** | `TarjetaDeIdentidad` (1), `CedulaDeCiudadania` (2), `CedulaDeExtranjeria` (3), `Pasaporte` (4), `NumeroDeIdentificacionTributaria` (5), `PermisoEspecialDePermanencia` (6) | Colombian identity documents — covers CC, CE, NIT, PEP |
| **AccidentSeverity** | `Incident` (0), `Mild` (1), `Moderate` (2), `Severe` (3), `Critico` (4) | Severity classification per Colombian resolution |
| **Priority** | `Low` (0), `Medium` (1), `High` (2), `Critical` (3) | Corrective action urgency |
| **StatusAction** | `Rejected` (0), `Proposal` (1), `Approved` (2), `InProcess` (3), `Completed` (4) | Corrective action lifecycle |
| **AttachmentEntityType** | `Accident` (1), `CorrectiveAction` (2), `Witness` (3), `Employee` (4) | Polymorphic attachment association |
| **PhoneType** | `Mobile` (0), `Home` (1), `Work` (2), `Other` (3) | Phone number classification |

The `[Display(Name = "...")]` values are in Spanish because SG-SST is a Colombian regulatory system — all user-facing labels should render in Spanish by default.

---

## DTO Layer

The DTO (Data Transfer Object) layer provides **separation between the internal domain model and external API contracts**. All DTOs are implemented as C# `record` types, giving them value equality, immutability (`init` setters), and concise syntax.

| Category | Files | Key Types |
|----------|-------|-----------|
| **Base** | `BaseDto.cs` | `abstract record BaseDto` — Id, audit timestamps |
| **Pagination** | `PagedResponse.cs` | `PagedResponse<T>` — Items, TotalCount, PageNumber, PageSize, computed TotalPages |
| **Auth** | `LoginRequest.cs`, `AuthResponse.cs`, `RefreshTokenRequest.cs`, `RevokeTokenRequest.cs` | Authentication contracts |
| **Users** | `Users/*.cs` | `UserDto`, `CreateUserRequest`, `UpdateUserRequest`, `AssignOrRemoveRoleRequest`, `ChangePasswordRequest` |
| **Accidents** | `Accidents/*.cs` | `AccidentDto`, `CreateAccidentRequest`, `UpdateAccidentRequest` |
| **Employees** | `Employees/*.cs` | `EmployeeDto`, `CreateEmployeeRequest`, `UpdateEmployeeRequest`, plus email/phone variants |
| **Entity CRUD** | 18 entity directories | Mirror the entity model with Create/Update/Dto per entity |

**Key design characteristics:**

- **`BaseDto`** mirrors `BaseEntity` but as an immutable record — API consumers receive audit data without being able to modify it.
- **`PagedResponse<T>`** includes computed properties (`HasPreviousPage`, `HasNextPage`, `TotalPages`) — the server computes pagination state so clients don't have to.
- **Request DTOs** carry `[Required]`, `[EmailAddress]`, `[MinLength]`, `[MaxLength]` annotations used by FluentValidation on the API side.
- **Nested namespace organization** mirrors the entity structure (`sicoain.shared.DTOs.Accidents`, `sicoain.shared.DTOs.Employees`, etc.).

---

## Security Constants

```csharp
public static class AppPermissions
{
    // Accidents module
    public const string AccidentsView    = "Accidents.View";
    public const string AccidentsCreate  = "Accidents.Create";
    public const string AccidentsEdit    = "Accidents.Edit";
    public const string AccidentsDelete  = "Accidents.Delete";
    public const string AccidentsApprove = "Accidents.Approve";

    // Employees module
    public const string EmployeesView   = "Employees.View";
    public const string EmployeesCreate = "Employees.Create";
    public const string EmployeesEdit   = "Employees.Edit";
    public const string EmployeesDelete = "Employees.Delete";

    // Reports module
    public const string ReportsView   = "Reports.View";
    public const string ReportsExport = "Reports.Export";

    // Users module
    public const string UsersView       = "Users.View";
    public const string UsersCreate     = "Users.Create";
    public const string UsersEdit       = "Users.Edit";
    public const string UsersDelete     = "Users.Delete";
    public const string UsersAssignRoles = "Users.AssignRoles";

    // Settings module
    public const string SettingsView    = "Settings.View";
    public const string SettingsEdit    = "Settings.Edit";

    // Superadmin only
    public const string PermissionsManage = "Settings.Manage";
}
```

These constants prevent magic strings throughout the application. Authorization policies reference `AppPermissions.AccidentsCreate`, not `"Accidents.Create"` — if a permission name ever changes, the compiler catches every reference.

---

## Usage Examples

### Installing the Package

```bash
dotnet add project src/sicoain.api/sicoain.api.csproj reference src/sicoain.shared/sicoain.shared.csproj
```

### Defining a DbContext

```csharp
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using sicoain.shared.Entities;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public DbSet<Business> Businesses => Set<Business>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Accident> Accidents => Set<Accident>();
    public DbSet<AccidentType> AccidentTypes => Set<AccidentType>();
    public DbSet<EventCategory> EventCategories => Set<EventCategory>();
    public DbSet<CorrectiveAction> CorrectiveActions => Set<CorrectiveAction>();
    public DbSet<DigitalEvidence> DigitalEvidences => Set<DigitalEvidence>();
    public DbSet<Witness> Witnesses => Set<Witness>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<RiskClass> RiskClasses => Set<RiskClass>();
    public DbSet<HealthPromotionEntity> HealthPromotionEntities => Set<HealthPromotionEntity>();
    public DbSet<OccupationalRiskAdministrator> OccupationalRiskAdministrators => Set<OccupationalRiskAdministrator>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Permissions> Permissions => Set<Permissions>();
    public DbSet<Roles> Roles => Set<Roles>();
    public DbSet<RolePermissions> RolePermissions => Set<RolePermissions>();
}
```

Note the `IdentityDbContext<User, IdentityRole<int>, int>` — all three generic parameters use `int` keys, consistent with the `User : IdentityUser<int>` declaration.

### Creating an Employee with Full Relationships

```csharp
using sicoain.shared.Entities;
using sicoain.shared.Enums;

var employee = new Employee
{
    DocumentType = DocumentType.CedulaDeCiudadania,
    DocumentNumber = "1234567890",
    FirstName = "Juan",
    SecondName = "Carlos",
    Surname = "Pérez",
    SecondSurname = "González",
    State = "Cundinamarca",
    Municipality = "Bogotá",
    Neighborhood = "Chapinero",
    AddressStreet = "Carrera 7 # 71 - 21",
    HiringDate = DateTime.UtcNow,

    // Foreign keys — required relationships
    BusinessId = businessId,
    BranchId = branchId,
    PositionId = positionId,
    DepartmentId = departmentId,
    HealthPromotionEntityId = epsId,
    OccupationalRiskAdministratorId = arlId,

    // Optional medical info
    Diseases = "Ninguna",
    Medications = "Ninguno",
    Allergies = "Ninguna"
};

// EF Core will auto-set CreatedAt and CreatedBy via the service layer
context.Employees.Add(employee);
await context.SaveChangesAsync();
```

### Registering an Accident with Digital Evidence

```csharp
var accident = new Accident
{
    EventDate = DateTime.UtcNow,
    Description = "Caída desde escalera portátil durante mantenimiento de luminarias",
    EmployeeId = employeeId,
    AccidentTypeId = accidentTypeId,
    EventCategoryId = eventCategoryId
};

context.Accidents.Add(accident);
await context.SaveChangesAsync();

// Attach digital evidence with chain of custody
var evidence = new DigitalEvidence
{
    FileName = "foto_luminaria_01.jpg",
    FilePath = "/evidence/2026/05/accident_142/foto_luminaria_01.jpg",
    FileSize = 2_457_600,  // bytes
    MimeType = "image/jpeg",
    FileHash = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855",
    Description = "Fotografía del área donde ocurrió el incidente",
    TakenAt = DateTime.UtcNow,
    TakenByName = "Carlos Martínez — Inspector de Seguridad",
    ChainOfCustody = $"Recolectado por: Inspector Martínez | Fecha: {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC | Dispositivo: Cámara Samsung Galaxy S24",
    AccidentId = accident.Id
};

context.DigitalEvidences.Add(evidence);
await context.SaveChangesAsync();
```

### Soft Delete & Audit Trail

```csharp
// Get active employees (IsDeleted == false is the convention)
var activeEmployees = await context.Employees
    .Where(e => !e.IsDeleted)
    .Include(e => e.Business)
    .Include(e => e.Position)
    .ToListAsync();

// Soft delete — preserves data integrity
employee.MarkAsDeleted(currentUserId);
await context.SaveChangesAsync();

// Restore if needed
employee.Restore();
await context.SaveChangesAsync();

// Manual audit update
employee.UpdateTimestamps(currentUserId);
```

### Querying with PagedResponse

```csharp
using sicoain.shared.DTOs;

var pageNumber = 1;
var pageSize = 10;

var query = context.Employees
    .Where(e => !e.IsDeleted && e.BusinessId == businessId);

var totalCount = await query.CountAsync();

var items = await query
    .OrderBy(e => e.FirstName)
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .Select(e => new EmployeeDto
    {
        Id = e.Id,
        FirstName = e.FirstName,
        Surname = e.Surname,
        DocumentType = e.DocumentType,
        DocumentNumber = e.DocumentNumber,
        BusinessName = e.Business!.Name,
        PositionName = e.Position!.Name,
        HiringDate = e.HiringDate
    })
    .ToListAsync();

var response = new PagedResponse<EmployeeDto>
{
    Items = items,
    TotalCount = totalCount,
    PageNumber = pageNumber,
    PageSize = pageSize
};

// response.TotalPages => computed
// response.HasPreviousPage => false (page 1)
// response.HasNextPage => true if more pages
```

### Authentication Flow

```csharp
// 1. User logs in
var login = new LoginRequest("juan.perez@empresa.com", "SecurePass123!");

// 2. Validate credentials, generate JWT + refresh token
var refreshToken = new RefreshToken
{
    Token = GenerateCryptographicToken(),  // 256-bit random
    UserId = user.Id,
    ExpiresAt = DateTime.UtcNow.AddDays(7),
    CreatedByIp = httpContext.Connection.RemoteIpAddress?.ToString()
};

// 3. Token rotation: when refreshing, revoke old + issue new
oldRefreshToken.Revoke(ipAddress, "Replaced by new token");
newRefreshToken.ReplacedByTokenId = oldRefreshToken.Id;

// 4. Check token state
if (storedToken.IsExpired) { /* reject */ }
if (storedToken.IsRevoked) { /* possible token reuse — revoke family */ }
if (storedToken.IsActive)  { /* valid for use */ }
```

### RBAC Permission Checking

```csharp
using sicoain.shared.Constants;

// In an authorization handler:
bool canCreateAccident = await context.RolePermissions
    .Include(rp => rp.Permission)
    .Include(rp => rp.Role)
    .AnyAsync(rp =>
        rp.Permission!.Module == "Accidents" &&
        rp.Permission.Action == "Create" &&
        rp.Role!.IdentityRoleId == userRoleId
    );

// Using the constant:
if (!canCreateAccident)
    throw new UnauthorizedAccessException($"Missing: {AppPermissions.AccidentsCreate}");
```

---

## Design Decisions & Patterns

### Why IdentityUser\<int\> Instead of string?

ASP.NET Core Identity defaults to `string` (GUID) keys. This project uses `IdentityUser<int>` (and consequently `IdentityRole<int>`, `IdentityDbContext<User, IdentityRole<int>, int>`) because:

1. **Consistency with BaseEntity**: All domain entities use `int Id` — having `User` use `string` would break the pattern.
2. **Performance**: `int` keys are faster in database joins and index seeks than `string` GUIDs.
3. **Simplicity**: Auto-incrementing integer keys are easier to debug, reference in logs, and use in URLs (`/api/users/42`).

The tradeoff is that user IDs become predictable (sequential integers), but this is mitigated by the JWT authentication layer — the ID alone carries no authorization.

### Polymorphic Contact Inheritance

Instead of repeating `Email`/`Phone`/`PhoneType` properties on every entity that needs contact info, the library uses **abstract base classes**:

```
BaseEntityEmail  →  BusinessEmail, BranchEmail, EmployeeEmail, ...
BaseEntityPhone  →  BusinessPhone, BranchPhone, EmployeePhone, ...
```

This avoids the "magic number" antipattern where a single `PhoneType` column discriminates between business phones and employee phones. Each contact type is its own table with proper foreign keys, making joins explicit and queries efficient.

### BaseDto as Record Type

`BaseDto` is declared as `abstract record` rather than `abstract class` because:

1. **Immutability**: `init` setters ensure DTOs cannot be modified after creation.
2. **Value equality**: Two DTOs with the same values are equal — critical for testing assertions.
3. **Deconstruction**: Records support positional deconstruction for pattern matching.
4. **Non-destructive mutation**: `with` expressions allow creating modified copies.

### RefreshToken Rotation Design

The `RefreshToken` entity implements the **token rotation** pattern recommended by OWASP:

- When a refresh token is used, it is **revoked** and a **new token** is issued in its place.
- `ReplacedByTokenId` creates an auditable chain of token usage.
- If a revoked token is presented, it indicates possible token theft — the entire token family should be revoked.
- `Revoke(ipAddress, reason)` encapsulates revocation in a single method rather than exposing setters.

### Column Mapping Convention

Entity properties use explicit `[Column]` attributes with SQL Server types (`varchar`, `datetime2`, `decimal(5,4)`):

```csharp
[Column("document_type", TypeName = "varchar(100)")]
public required DocumentType DocumentType { get; set; }
```

This gives the development team **explicit control** over the database schema rather than relying on EF Core conventions. The `snake_case` column names follow the database naming convention used throughout the project.

---

## Contributing

Contributions are welcome. This project follows a **feature branch workflow**:

1. Fork the repository.
2. Create a feature branch: `git checkout -b feat/your-feature`.
3. Make your changes — maintain the existing patterns (abstract contacts, `int` keys, records).
4. Ensure all tests pass: `dotnet test`.
5. Push and open a Pull Request.

**Guidelines:**

- Do NOT add `Co-Authored-By` or AI attribution to commits.
- Use [conventional commits](https://www.conventionalcommits.org/) (`feat:`, `fix:`, `refactor:`, `docs:`, `test:`, `chore:`).
- New entities should inherit from `BaseEntity` (or `BaseEntityEmail`/`BaseEntityPhone` for contacts).
- New DTOs should inherit from `BaseDto` as `record` types.
- All new enums must include `[Display(Name = "...")]` in Spanish.

---

## License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE) file for details.

---

<div align="center">

**Built with 🔥 by [sicoain.net](https://sicoain.net)**

*Comprometidos con la seguridad y salud en el trabajo en Colombia 🇨🇴*

</div>
