# sicoain.shared — Domain Catalog

> **Technical reference for the shared domain layer of SICOAIN (Sistema de Control de Accidentes e Incidentes)**
> Comprehensive catalog of all entities, DTOs, enums, constants, and interfaces in the `sicoain.shared` class library.

---

## Table of Contents

- [Project Configuration](#project-configuration)
- [Packages & Dependencies](#packages--dependencies)
- [Entity Inheritance Hierarchy](#entity-inheritance-hierarchy)
- [Entity Catalog](#entity-catalog)
  - [BaseEntity (abstract)](#baseentity-abstract)
  - [BaseEntityEmail (abstract)](#baseentityemail-abstract)
  - [BaseEntityPhone (abstract)](#baseentityphone-abstract)
  - [User](#user)
  - [Business](#business)
  - [BusinessPhone](#businessphone)
  - [BusinessEmail](#businessemail)
  - [Branch](#branch)
  - [BranchPhone](#branchphone)
  - [BranchEmail](#branchemail)
  - [Employee](#employee)
  - [EmployeePhone](#employeephone)
  - [EmployeeEmail](#employeeemail)
  - [EmployeeContact](#employeecontact)
  - [EmployeeContactPhone](#employeecontactphone)
  - [EmployeeContactEmail](#employeecontactemail)
  - [Accident](#accident)
  - [AccidentType](#accidenttype)
  - [EventCategory](#eventcategory)
  - [DigitalEvidence](#digitalevidence)
  - [Witness](#witness)
  - [Attachment](#attachment)
  - [CorrectiveAction](#correctiveaction)
  - [CorrectiveActionTracking](#correctiveactiontracking)
  - [Position](#position)
  - [Department](#department)
  - [RiskClass](#riskclass)
  - [HealthPromotionEntity](#healthpromotionentity)
  - [HealthPromotionEntityPhone](#healthpromotionentityphone)
  - [HealthPromotionEntityEmail](#healthpromotionentityemail)
  - [OccupationalRiskAdministrator](#occupationalriskadministrator)
  - [OccupationalRiskAdministratorPhone](#occupationalriskadministratorphone)
  - [OccupationalRiskAdministratorEmail](#occupationalriskadministratoremail)
  - [Permissions](#permissions)
  - [Roles](#roles)
  - [RolePermissions](#rolepermissions)
  - [RefreshToken](#refreshtoken)
- [Enum Catalog](#enum-catalog)
  - [DocumentType](#documenttype)
  - [AccidentSeverity](#accidentseverity)
  - [Priority](#priority)
  - [StatusAction](#statusaction)
  - [AttachmentEntityType](#attachmententitytype)
  - [PhoneType](#phonetype)
- [DTO Catalog](#dto-catalog)
  - [BaseDto (abstract record)](#basedto-abstract-record)
  - [PagedResponse\<T\>](#pagedresponset)
  - [Auth DTOs](#auth-dtos)
  - [User DTOs](#user-dtos)
  - [Accident DTOs](#accident-dtos)
  - [Entity DTOs (by namespace)](#entity-dtos-by-namespace)
  - [Shared Base DTOs](#shared-base-dtos)
- [Constants Catalog](#constants-catalog)
  - [AppPermissions](#apppermissions)
- [Interfaces](#interfaces)
- [Namespace Map](#namespace-map)

---

## Project Configuration

| Property | Value |
|----------|-------|
| **Project file** | `src/sicoain.shared/sicoain.shared.csproj` |
| **SDK** | `Microsoft.NET.Sdk` (class library) |
| **Target framework** | `net10.0` |
| **Nullable** | Enabled (`<Nullable>enable</Nullable>`) |
| **Implicit usings** | Enabled (`<ImplicitUsings>enable</ImplicitUsings>`) |
| **Assembly** | `sicoain.shared` |
| **Root namespace** | `sicoain.shared` |

---

## Packages & Dependencies

| Package | Version | Purpose | Dependency Type |
|---------|---------|---------|-----------------|
| `Microsoft.Extensions.Identity.Stores` | `10.0.7` | Provides `IdentityUser<TKey>` base class for `User : IdentityUser<int>` | NuGet (direct) |
| `System.ComponentModel.Annotations` | `5.0.0` | `[Required]`, `[MaxLength]`, `[Key]`, `[Display]` attributes on entities and DTOs | NuGet (direct) |

**Transitive dependencies** (pulled by `Identity.Stores`):

- `Microsoft.Extensions.Identity.Core` (10.0.7)
- `Microsoft.AspNetCore.Cryptography.Internal` (10.0.7)
- `Microsoft.AspNetCore.Cryptography.KeyDerivation` (10.0.7)
- `Microsoft.Extensions.Logging.Abstractions` (10.0.x)
- `Microsoft.Extensions.Options` (10.0.x)

---

## Entity Inheritance Hierarchy

```
Object
├── RefreshToken                        (standalone — no BaseEntity inheritance)
│
└── BaseEntity (abstract)              (audit + soft delete)
    ├── BaseEntityEmail (abstract)      (adds Email property)
    │   ├── BusinessEmail
    │   ├── BranchEmail
    │   ├── EmployeeEmail
    │   ├── EmployeeContactEmail
    │   ├── HealthPromotionEntityEmail
    │   └── OccupationalRiskAdministratorEmail
    │
    ├── BaseEntityPhone (abstract)      (adds Phone + PhoneType)
    │   ├── BusinessPhone
    │   ├── BranchPhone
    │   ├── EmployeePhone
    │   ├── EmployeeContactPhone
    │   ├── HealthPromotionEntityPhone
    │   └── OccupationalRiskAdministratorPhone
    │
    ├── Business
    ├── Branch
    ├── Employee
    ├── EmployeeContact
    ├── Accident
    ├── AccidentType
    ├── EventCategory
    ├── DigitalEvidence
    ├── Witness
    ├── Attachment
    ├── CorrectiveAction
    ├── CorrectiveActionTracking
    ├── Position
    ├── Department
    ├── RiskClass
    ├── HealthPromotionEntity
    ├── OccupationalRiskAdministrator
    ├── Permissions
    ├── Roles
    ├── RolePermissions
    └── User : IdentityUser<int>        (replicates BaseEntity audit pattern)
```

---

## Entity Catalog

### BaseEntity (abstract)

**File:** `Entities/BaseEntity.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** None (top of hierarchy)
**Attributes:** `abstract`

The foundational base class providing audit trail and soft delete for all persistent entities.

#### Properties

| Property | Type | Access | Default | Attributes | Description |
|----------|------|--------|---------|------------|-------------|
| `Id` | `int` | get; set | — | `[Key]` | Primary key |
| `CreatedAt` | `DateTime` | get; set | `DateTime.UtcNow` | — | Creation timestamp |
| `UpdatedAt` | `DateTime` | get; set | `DateTime.UtcNow` | — | Last modification timestamp |
| `DeletedAt` | `DateTime?` | get; set | `null` | — | Soft delete timestamp |
| `CreatedBy` | `int` | get; set | — | — | User ID who created |
| `UpdatedBy` | `int` | get; set | — | — | User ID who last modified |
| `DeletedBy` | `int?` | get; set | `null` | — | User ID who soft-deleted |
| `IsDeleted` | `bool` | get; set | `false` | — | Soft delete flag |

#### Methods

| Method | Parameters | Behavior |
|--------|-----------|----------|
| `UpdateTimestamps` | `int userId` | Sets `UpdatedBy = userId`, `UpdatedAt = DateTime.UtcNow` |
| `MarkAsDeleted` | `int userId` | Sets `DeletedBy = userId`, `DeletedAt = DateTime.UtcNow`, `IsDeleted = true` |
| `Restore` | *(none)* | Sets `DeletedBy = null`, `DeletedAt = null`, `IsDeleted = false` |

---

### BaseEntityEmail (abstract)

**File:** `Entities/BaseEntityEmail.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntity`
**Attributes:** `abstract`

Template for polymorphic email contact entities.

#### Properties

| Property | Type | Access | Attributes | Description |
|----------|------|--------|------------|-------------|
| *(inherited)* | — | — | — | All `BaseEntity` properties |
| `Email` | `string` | get; set | `[Required]` | Email address |

**Inherited by:** `BusinessEmail`, `BranchEmail`, `EmployeeEmail`, `EmployeeContactEmail`, `HealthPromotionEntityEmail`, `OccupationalRiskAdministratorEmail`

---

### BaseEntityPhone (abstract)

**File:** `Entities/BaseEntityPhone.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntity`
**Attributes:** `abstract`

Template for polymorphic phone contact entities with type classification.

#### Properties

| Property | Type | Access | Attributes | Description |
|----------|------|--------|------------|-------------|
| *(inherited)* | — | — | — | All `BaseEntity` properties |
| `Phone` | `string` | get; set | `[Required]` | Phone number |
| `PhoneType` | `PhoneType` | get; set | `[Column("phone_type")]`, `[Required]` | Classification (Mobile/Home/Work/Other) |

**Inherited by:** `BusinessPhone`, `BranchPhone`, `EmployeePhone`, `EmployeeContactPhone`, `HealthPromotionEntityPhone`, `OccupationalRiskAdministratorPhone`

---

### User

**File:** `Entities/User.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `IdentityUser<int>` (from `Microsoft.Extensions.Identity.Stores`)

ASP.NET Core Identity user with custom audit fields (mirrors `BaseEntity` pattern manually).

#### IdentityUser\<int\> inherited properties

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `int` | Primary key |
| `UserName` | `string?` | Login username |
| `NormalizedUserName` | `string?` | Upper-case username |
| `Email` | `string?` | Email address |
| `NormalizedEmail` | `string?` | Upper-case email |
| `EmailConfirmed` | `bool` | Email verification flag |
| `PasswordHash` | `string?` | Hashed password |
| `SecurityStamp` | `string?` | Security stamp for invalidation |
| `ConcurrencyStamp` | `string?` | Optimistic concurrency token |
| `PhoneNumber` | `string?` | Phone number |
| `PhoneNumberConfirmed` | `bool` | Phone verification flag |
| `TwoFactorEnabled` | `bool` | 2FA flag |
| `LockoutEnd` | `DateTimeOffset?` | Lockout expiration |
| `LockoutEnabled` | `bool` | Lockout feature flag |
| `AccessFailedCount` | `int` | Failed login attempts |

#### Custom Properties

| Property | Type | Access | Attributes | Description |
|----------|------|--------|------------|-------------|
| `FullName` | `string` | get; set | `[Required]`, `[MinLength(8)]`, `[MaxLength(100)]` | User's full name |
| `CreatedAt` | `DateTime` | get; set | — | Creation timestamp |
| `UpdatedAt` | `DateTime` | get; set | — | Last update timestamp |
| `DeletedAt` | `DateTime?` | get; set | — | Soft delete timestamp |
| `CreatedBy` | `int` | get; set | — | Creator user ID |
| `UpdatedBy` | `int` | get; set | — | Last modifier user ID |
| `DeletedBy` | `int?` | get; set | — | Deleter user ID |
| `IsDeleted` | `bool` | get; set | — | Soft delete flag |
| `IsActive` | `bool` | get; set | — | Active account flag |
| `RefreshTokens` | `ICollection<RefreshToken>` | get; set | — | Navigation collection |

#### Custom Methods

| Method | Parameters | Behavior |
|--------|-----------|----------|
| `UpdateTimestamps` | `int userId` | Mirrors `BaseEntity.UpdateTimestamps` |
| `MarkAsDeleted` | `int userId` | Mirrors `BaseEntity.MarkAsDeleted` |
| `Restore` | *(none)* | Mirrors `BaseEntity.Restore` |

**Note:** `User` does NOT inherit from `BaseEntity` because it must extend `IdentityUser<int>`. It manually replicates the same audit/soft-delete properties and methods for consistency.

---

### Business

**File:** `Entities/Business.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntity`

A company or organization — root aggregate for organizational data.

#### Properties

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntity` properties |
| `Name` | `string` | `[Required]` | Business name |
| `AddressStreet` | `string?` | `[Column("address_street", TypeName = "varchar(200)")]`, `[MaxLength(200)]` | Street address |
| `Phones` | `ICollection<BusinessPhone>?` | — | Navigation: phone numbers |
| `Emails` | `ICollection<BusinessEmail>?` | — | Navigation: email addresses |
| `Branches` | `ICollection<Branch>?` | — | Navigation: physical locations |
| `Employees` | `ICollection<Employee>?` | — | Navigation: workers |

---

### BusinessPhone

**File:** `Entities/BusinessPhone.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntityPhone`

#### Properties

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntityPhone` + `BaseEntity` properties |
| `BusinessId` | `int` | `[Required]` | FK to parent `Business` |
| `Business` | `Business` | `[Required]` | Navigation property |

---

### BusinessEmail

**File:** `Entities/BusinessEmail.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntityEmail`

#### Properties

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntityEmail` + `BaseEntity` properties |
| `BusinessId` | `int` | `[Required]` | FK to parent `Business` |
| `Business` | `Business` | `[Required]` | Navigation property |

---

### Branch

**File:** `Entities/Branch.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntity`

A physical location (sucursal) belonging to a Business.

#### Properties

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntity` properties |
| `Name` | `string` | `[Required]` | Branch name |
| `AddressStreet` | `string?` | `[Column("address_street", TypeName = "varchar(200)")]`, `[MaxLength(200)]` | Street address |
| `BusinessId` | `int` | `[Required]` | FK to parent `Business` |
| `Business` | `Business` | `[Required]` | Navigation property |
| `Phones` | `ICollection<BranchPhone>?` | — | Navigation: phone numbers |
| `Emails` | `ICollection<BranchEmail>?` | — | Navigation: email addresses |
| `Employees` | `ICollection<Employee>?` | — | Navigation: workers |

---

### BranchPhone

**File:** `Entities/BranchPhone.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntityPhone`

#### Properties

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntityPhone` + `BaseEntity` properties |
| `BranchId` | `int` | `[Required]` | FK to parent `Branch` |
| `Branch` | `Branch` | `[Required]` | Navigation property |

---

### BranchEmail

**File:** `Entities/BranchEmail.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntityEmail`

#### Properties

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntityEmail` + `BaseEntity` properties |
| `BranchId` | `int` | `[Required]` | FK to parent `Branch` |
| `Branch` | `Branch` | `[Required]` | Navigation property |

---

### Employee

**File:** `Entities/Employee.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntity`

A worker with full Colombian identity support, medical information, and regulatory entity affiliations.

#### Properties

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntity` properties |
| `DocumentType` | `DocumentType` | `[Column("document_type", TypeName = "varchar(100)")]`, `[Required]` | Colombian identity document type |
| `DocumentNumber` | `string` | `[Column("document_number", TypeName = "varchar(100)")]`, `[Required]` | Identity document number |
| `FirstName` | `string` | `[Column("first_name", TypeName = "varchar(100)")]`, `[Required]` | First name (primer nombre) |
| `SecondName` | `string` | `[Column("second_name", TypeName = "varchar(100)")]`, `[Required]` | Second name (segundo nombre) |
| `Surname` | `string` | `[Column("surname", TypeName = "varchar(100)")]`, `[Required]` | Last name (primer apellido) |
| `SecondSurname` | `string` | `[Column("second_surname", TypeName = "varchar(100)")]`, `[Required]` | Second last name (segundo apellido) |
| `State` | `string` | `[Column(TypeName = "varchar(100)")]`, `[Required]` | Department/State (departamento) |
| `Municipality` | `string` | `[Column(TypeName = "varchar(100)")]`, `[Required]` | Municipality (municipio) |
| `Neighborhood` | `string` | `[Column(TypeName = "varchar(100)")]`, `[Required]` | Neighborhood (barrio) |
| `AddressStreet` | `string` | `[Column("address_street", TypeName = "varchar(200)")]`, `[Required]` | Street address |
| `AlternativeAddressStreet` | `string?` | `[Column("alternative_address_street", TypeName = "varchar(200)")]` | Alternative address |
| `PostalCode` | `string?` | `[Column("postal_code", TypeName = "varchar(20)")]`, `[Required]` | Postal code |
| `HiringDate` | `DateTime` | `[Column("hiring_date", TypeName = "datetime2")]`, `[Required]` | Hire date |
| `TerminationDate` | `DateTime?` | `[Column("termination_date", TypeName = "datetime2")]` | Termination date |
| `Diseases` | `string?` | `[Column(TypeName = "varchar(200)")]` | Pre-existing diseases |
| `Medications` | `string?` | `[Column(TypeName = "varchar(200)")]` | Current medications |
| `Allergies` | `string?` | `[Column(TypeName = "varchar(200)")]` | Known allergies |
| `Notes` | `string?` | `[Column(TypeName = "varchar(255)")]` | General notes |

#### Foreign Keys & Navigation Properties

| Property | Type | Nullable | Related Entity | Description |
|----------|------|----------|---------------|-------------|
| `BusinessId` | `int` | No | `Business` | Employer company |
| `Business` | `Business?` | Yes | — | Navigation |
| `BranchId` | `int` | No | `Branch` | Physical location |
| `Branch` | `Branch?` | Yes | — | Navigation |
| `HealthPromotionEntityId` | `int` | No | `HealthPromotionEntity` | EPS affiliation |
| `HealthPromotionEntity` | `HealthPromotionEntity?` | Yes | — | Navigation |
| `OccupationalRiskAdministratorId` | `int` | No | `OccupationalRiskAdministrator` | ARL affiliation |
| `OccupationalRiskAdministrator` | `OccupationalRiskAdministrator?` | Yes | — | Navigation |
| `DepartmentId` | `int` | No | `Department` | Organizational department |
| `Department` | `Department?` | Yes | — | Navigation |
| `PositionId` | `int` | No | `Position` | Job position |
| `Position` | `Position?` | Yes | — | Navigation |

#### Collections

| Property | Type | Description |
|----------|------|-------------|
| `EmployeePhones` | `ICollection<EmployeePhone>?` | Contact phone numbers |
| `EmployeeEmails` | `ICollection<EmployeeEmail>?` | Contact email addresses |
| `EmployeeContacts` | `ICollection<EmployeeContact>?` | Emergency contacts |
| `Witnesses` | `ICollection<Witness>?` | Accidents where employee was witness |
| `Accidents` | `ICollection<Accident>?` | Accidents involving this employee |

---

### EmployeePhone

**File:** `Entities/EmployeePhone.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntityPhone`

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntityPhone` + `BaseEntity` properties |
| `EmployeeId` | `int` | `[Required]` | FK to parent `Employee` |
| `Employee` | `Employee?` | — | Navigation property |

---

### EmployeeEmail

**File:** `Entities/EmployeeEmail.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntityEmail`

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntityEmail` + `BaseEntity` properties |
| `EmployeeId` | `int` | `[Required]` | FK to parent `Employee` |
| `Employee` | `Employee?` | — | Navigation property |

---

### EmployeeContact

**File:** `Entities/EmployeeContact.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntity`

An emergency contact person linked to an employee.

#### Properties

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntity` properties |
| `Fullname` | `string` | `[Required]` | Contact's full name |
| `Relationship` | `string` | `[Required]` | Relationship to employee (e.g., "Spouse", "Parent") |
| `EmployeeId` | `int` | `[Required]` | FK to `Employee` |
| `Employee` | `Employee` | `[Required]` | Navigation property |
| `EmployeeContactPhones` | `ICollection<EmployeeContactPhone>?` | — | Contact phone numbers |
| `EmployeeContactEmails` | `ICollection<EmployeeContactEmail>?` | — | Contact email addresses |

---

### EmployeeContactPhone

**File:** `Entities/EmployeeContactPhone.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntityPhone`

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntityPhone` + `BaseEntity` properties |
| `EmployeeContactId` | `int` | `[Required]` | FK to parent `EmployeeContact` |
| `EmployeeContact` | `EmployeeContact` | `[Required]` | Navigation property |

---

### EmployeeContactEmail

**File:** `Entities/EmployeeContactEmail.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntityEmail`

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntityEmail` + `BaseEntity` properties |
| `EmployeeContactId` | `int` | `[Required]` | FK to parent `EmployeeContact` |
| `EmployeeContact` | `EmployeeContact` | `[Required]` | Navigation property |

---

### Accident

**File:** `Entities/Accident.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntity`

A workplace accident or incident — the central record in the SICOAIN system.

#### Properties

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntity` properties |
| `EventDate` | `DateTime` | `[Column("event_date", TypeName = "datetime2")]`, `[Required]` | Date and time of the incident |
| `Description` | `string` | `[Column("description", TypeName = "nvarchar(500)")]`, `[Required]` | Detailed description of what happened |
| `EmployeeId` | `int` | `[Required]` | FK to the affected `Employee` |
| `Employee` | `Employee` | `[Required]` | Navigation property |
| `AccidentTypeId` | `int` | `[Required]` | FK to `AccidentType` |
| `AccidentType` | `AccidentType` | `[Required]` | Navigation property |
| `EventCategoryId` | `int` | `[Required]` | FK to `EventCategory` |
| `EventCategory` | `EventCategory` | `[Required]` | Navigation property |

#### Collections

| Property | Type | Description |
|----------|------|-------------|
| `DigitalEvidences` | `ICollection<DigitalEvidence>?` | Photo/video/document evidence |
| `Witnesses` | `ICollection<Witness>?` | People who witnessed the accident |
| `CorrectiveActions` | `ICollection<CorrectiveAction>?` | Remediation actions taken |

---

### AccidentType

**File:** `Entities/AccidentType.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntity`

Classification of workplace accidents by type and severity level.

#### Properties

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntity` properties |
| `Name` | `string` | `[Required]` | Type name (e.g., "Caída desde altura") |
| `Description` | `string?` | `[Column(TypeName = "varchar(255)")]` | Optional description |
| `Severity` | `AccidentSeverity` | — | Default severity level for this type |

---

### EventCategory

**File:** `Entities/EventCategory.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntity`

Categorization of workplace events with severity threshold and hospitalization flag.

#### Properties

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntity` properties |
| `Name` | `string` | `[Required]` | Category name |
| `LevelOfSeverity` | `AccidentSeverity` | `[Required]` | Severity threshold |
| `RequiresHospitalization` | `bool` | — | Flag indicating if hospitalization is required |

---

### DigitalEvidence

**File:** `Entities/DigitalEvidence.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntity`

Forensic file evidence with chain of custody tracking.

#### Properties

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntity` properties |
| `FileName` | `string` | `[Required]` | Original file name |
| `FilePath` | `string` | `[Required]` | Storage path |
| `FileSize` | `long` | `[Required]` | File size in bytes |
| `MimeType` | `string` | `[Required]` | MIME type (e.g., "image/jpeg", "application/pdf") |
| `FileHash` | `string` | `[Required]` | SHA-256 hash for integrity verification |
| `Description` | `string` | `[Required]` | Description of the evidence |
| `TakenAt` | `DateTime` | `[Required]` | Date/time the evidence was collected |
| `TakenByName` | `string?` | — | Name of the person who collected it |
| `ChainOfCustody` | `string` | — | Chain of custody log text |
| `AccidentId` | `int?` | `[Required]` | FK to related `Accident` |
| `Accident` | `Accident?` | — | Navigation property |

---

### Witness

**File:** `Entities/Witness.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntity`

A person who witnessed an accident. Can be an existing `Employee` or an external person.

#### Properties

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntity` properties |
| `AccidentId` | `int` | `[Required]` | FK to related `Accident` |
| `Accident` | `Accident` | `[Required]` | Navigation property |
| `EmployeeId` | `int?` | — | FK to `Employee` (if witness is an employee) |
| `Employee` | `Employee?` | — | Navigation property |
| `WitnessName` | `string?` | — | Name (for external witnesses) |
| `WitnessContact` | `string?` | — | Contact info (for external witnesses) |
| `Statement` | `string` | `[Required]` | Witness statement text |

---

### Attachment

**File:** `Entities/Attachment.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntity`

Polymorphic file attachment that can be associated with any entity type.

#### Properties

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntity` properties |
| `FileName` | `string` | `[Required]` | Original file name |
| `FilePath` | `string` | `[Required]` | Storage path |
| `FileSize` | `long` | `[Required]` | File size in bytes |
| `MimeType` | `string` | `[Required]` | MIME type |
| `FileHash` | `string` | `[Required]` | SHA-256 hash |
| `Description` | `string` | `[Required]` | Description |
| `EntityType` | `AttachmentEntityType` | `[Required]` | Polymorphic discriminator (Accident, CorrectiveAction, Witness, Employee) |
| `EntityId` | `int` | `[Required]` | FK to the specific entity |

---

### CorrectiveAction

**File:** `Entities/CorrectiveAction.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntity`

A remediation action taken in response to an accident.

#### Properties

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntity` properties |
| `Title` | `string` | `[Column(TypeName = "varchar(255)")]`, `[Required]` | Action title |
| `Description` | `string` | `[Required]` | Detailed description |
| `DueDate` | `DateTime?` | `[Column("due_date", TypeName = "datetime2")]` | Deadline for completion |
| `Status` | `StatusAction` | `[Column(TypeName = "varchar(100)")]`, `[Required]` | Current status (Rejected → Proposal → Approved → InProcess → Completed) |
| `Priority` | `Priority` | `[Column(TypeName = "varchar(100)")]`, `[Required]` | Urgency level (Low → Medium → High → Critical) |
| `CompletionDate` | `DateTime?` | `[Column("completion_date", TypeName = "datetime2")]` | Actual completion date |
| `VerificationNotes` | `string?` | — | Notes from effectiveness verification |
| `IsEffective` | `bool` | `[Column("is_effective", TypeName = "bit")]` | Effectiveness flag |
| `AccidentId` | `int` | `[Required]` | FK to related `Accident` |
| `Accident` | `Accident?` | — | Navigation property |

#### Collections

| Property | Type | Description |
|----------|------|-------------|
| `Trackings` | `ICollection<CorrectiveActionTracking>?` | Status change history log |

---

### CorrectiveActionTracking

**File:** `Entities/CorrectiveActionTracking.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntity`

Immutable log entry recording status transitions for a corrective action.

#### Properties

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntity` properties |
| `CorrectiveActionId` | `int` | `[Required]` | FK to parent `CorrectiveAction` |
| `CorrectiveAction` | `CorrectiveAction?` | — | Navigation property |
| `OldStatus` | `string` | `[Column(TypeName = "varchar(100)")]`, `[Required]` | Previous status value |
| `NewStatus` | `string` | `[Column(TypeName = "varchar(100)")]`, `[Required]` | New status value |
| `TrackingDate` | `DateTime` | `[Required]` | When the transition occurred |
| `Comments` | `string` | `[Required]` | Notes about the transition |

---

### Position

**File:** `Entities/Position.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntity`

A job position within the organizational structure.

#### Properties

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntity` properties |
| `Name` | `string` | `[Required]` | Position title |
| `Description` | `string?` | — | Position description |
| `DepartmentId` | `int` | `[Required]` | FK to parent `Department` |
| `Department` | `Department` | `[Required]` | Navigation property |
| `RiskClassId` | `int` | `[Required]` | FK to `RiskClass` |
| `RiskClass` | `RiskClass?` | — | Navigation property |
| `Employees` | `ICollection<Employee>?` | — | Navigation: employees in this position |

---

### Department

**File:** `Entities/Department.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntity`

An organizational unit (departamento).

#### Properties

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntity` properties |
| `Name` | `string` | `[Required]` | Department name |
| `Description` | `string?` | — | Description |
| `Email` | `string?` | — | Department email |
| `Phone` | `string?` | — | Department phone |
| `Positions` | `ICollection<Position>?` | — | Navigation: job positions |

---

### RiskClass

**File:** `Entities/RiskClass.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntity`

OSHA/Colombian risk classification with contribution rate for ARL premium calculation.

#### Properties

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntity` properties |
| `Name` | `string` | `[Required]` | Risk class name |
| `Code` | `string` | `[Required]`, `[MaxLength(5)]` | Risk class code |
| `ContributionRate` | `decimal` | `[Column("contribution_rate", TypeName = "decimal(5,4)")]` | Premium rate (e.g., 0.0696 = 6.96%) |
| `IsActive` | `bool` | — | Active flag |
| `Positions` | `ICollection<Position>?` | — | Navigation: positions in this risk class |

---

### HealthPromotionEntity

**File:** `Entities/HealthPromotionEntity.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntity`

Colombian EPS (Entidad Promotora de Salud) — health insurance provider.

#### Properties

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntity` properties |
| `Name` | `string` | `[Required]` | EPS name |
| `AddressStreet` | `string?` | `[Column("address_street", TypeName = "varchar(200)")]`, `[MaxLength(200)]` | Address |
| `Notes` | `string?` | — | Additional notes |
| `Employees` | `ICollection<Employee>?` | — | Navigation: affiliated employees |

---

### HealthPromotionEntityPhone

**File:** `Entities/HealthPromotionEntityPhone.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntityPhone`

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntityPhone` + `BaseEntity` properties |
| `HealthPromotionEntityId` | `int` | `[Required]` | FK to parent `HealthPromotionEntity` |
| `HealthPromotionEntity` | `HealthPromotionEntity` | `[Required]` | Navigation property |

---

### HealthPromotionEntityEmail

**File:** `Entities/HealthPromotionEntityEmail.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntityEmail`

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntityEmail` + `BaseEntity` properties |
| `HealthPromotionEntityId` | `int` | `[Required]` | FK to parent `HealthPromotionEntity` |
| `HealthPromotionEntity` | `HealthPromotionEntity` | `[Required]` | Navigation property |

---

### OccupationalRiskAdministrator

**File:** `Entities/OccupationalRiskAdministrator.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntity`

Colombian ARL (Administradora de Riesgos Laborales) — occupational risk insurance administrator.

#### Properties

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntity` properties |
| `Name` | `string` | `[Required]` | ARL name |
| `AddressStreet` | `string?` | `[Column("address_street", TypeName = "varchar(200)")]`, `[MaxLength(200)]` | Address |
| `Phones` | `ICollection<OccupationalRiskAdministratorPhone>?` | — | Navigation: phone numbers |
| `Emails` | `ICollection<OccupationalRiskAdministratorEmail>?` | — | Navigation: email addresses |
| `Employees` | `ICollection<Employee>?` | — | Navigation: affiliated employees |

---

### OccupationalRiskAdministratorPhone

**File:** `Entities/OccupationalRiskAdministratorPhone.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntityPhone`

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntityPhone` + `BaseEntity` properties |
| `OccupationalRiskAdministratorId` | `int` | `[Required]` | FK to parent `OccupationalRiskAdministrator` |
| `OccupationalRiskAdministrator` | `OccupationalRiskAdministrator` | `[Required]` | Navigation property |

---

### OccupationalRiskAdministratorEmail

**File:** `Entities/OccupationalRiskAdministratorEmail.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntityEmail`

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntityEmail` + `BaseEntity` properties |
| `OccupationalRiskAdministratorId` | `int` | `[Required]` | FK to parent `OccupationalRiskAdministrator` |
| `OccupationalRiskAdministrator` | `OccupationalRiskAdministrator` | `[Required]` | Navigation property |

---

### Permissions

**File:** `Entities/Permissions.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntity`

Fine-grained permission definition for RBAC.

#### Properties

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntity` properties |
| `Name` | `string` | `[Required]`, `[MaxLength(100)]` | Permission name (e.g., "Accidents.View") |
| `Module` | `string` | `[Required]` | Module identifier (e.g., "Accidents", "Employees", "Reports") |
| `Action` | `string` | `[Required]` | Action verb (e.g., "View", "Create", "Edit", "Delete", "Approve") |
| `Description` | `string?` | — | Optional description |

---

### Roles

**File:** `Entities/Roles.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntity`

Application-level role with link to ASP.NET Core Identity role.

#### Properties

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntity` properties |
| `IdentityRoleId` | `int` | — | FK to `IdentityRole<int>` |
| `Name` | `string` | `[Required]` | Role name (e.g., "Admin", "Investigator") |
| `NormalizedName` | `string?` | — | Upper-case normalized name |
| `Description` | `string?` | — | Role description |
| `IsActive` | `bool` | — | Active flag |

---

### RolePermissions

**File:** `Entities/RolePermissions.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** `BaseEntity`

Many-to-many junction table between `Roles` and `Permissions`.

#### Properties

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| *(inherited)* | — | — | All `BaseEntity` properties |
| `RoleId` | `int` | `[Required]` | FK to `Roles` |
| `PermissionId` | `int` | `[Required]` | FK to `Permissions` |
| `Permission` | `Permissions?` | `[ForeignKey(nameof(PermissionId))]` | Navigation property |
| `Role` | `Roles?` | `[ForeignKey(nameof(RoleId))]` | Navigation property |

---

### RefreshToken

**File:** `Entities/RefreshToken.cs`
**Namespace:** `sicoain.shared.Entities`
**Base class:** *(none — standalone)*

JWT refresh token with rotation chain, revocation tracking, and audit.

This is the **only entity that does NOT inherit from `BaseEntity`**. It has its own audit and key structure.

#### Properties

| Category | Property | Type | Attributes | Description |
|----------|----------|------|------------|-------------|
| **Key** | `Id` | `int` | `[Key]` | Primary key |
| **Token Data** | `Token` | `string` | `[Required]`, `[MaxLength(200)]` | Unique token value |
| **Token Data** | `UserId` | `int` | `[Required]` | FK to `User` |
| **Token Data** | `User` | `User` | `[ForeignKey(nameof(UserId))]` | Navigation property |
| **Expiration** | `ExpiresAt` | `DateTime` | `[Required]` | Token expiration |
| **Revocation** | `RevokedAt` | `DateTime?` | — | Revocation timestamp (null = active) |
| **Revocation** | `RevokedByIp` | `string?` | `[MaxLength(45)]` | IP that triggered revocation |
| **Revocation** | `RevokedReason` | `string?` | `[MaxLength(200)]` | Reason for revocation |
| **Revocation** | `ReplacedByTokenId` | `int?` | — | Token rotation chain (new token ID) |
| **Audit** | `CreatedAt` | `DateTime` | — | Token creation timestamp |
| **Audit** | `CreatedByIp` | `string?` | `[MaxLength(45)]` | IP that requested the token |

#### Computed Properties (\[NotMapped\])

| Property | Type | Expression | Description |
|----------|------|------------|-------------|
| `IsRevoked` | `bool` | `RevokedAt != null` | Token has been revoked |
| `IsExpired` | `bool` | `DateTime.UtcNow >= ExpiresAt` | Token has expired |
| `IsActive` | `bool` | `!IsRevoked && !IsExpired` | Token is valid for use |

#### Methods

| Method | Parameters | Behavior |
|--------|-----------|----------|
| `Revoke` | `string ipAddress`, `string? reason = null` | Sets `RevokedAt = UtcNow`, `RevokedByIp`, `RevokedReason` |

---

## Enum Catalog

All enums use `System.ComponentModel.DataAnnotations.Display` attribute with Spanish-language display names for direct UI binding.

### DocumentType

**File:** `Enums/DocumentType.cs`
**Namespace:** `sicoain.shared.Enums`
**Storage:** Stored as `int` in database (via `[Column(TypeName = "varchar(100)")]` — stored as string name)

Colombian identity document types.

| Value | Name | Display Name | Description |
|-------|------|-------------|-------------|
| `1` | `TarjetaDeIdentidad` | Tarjeta de Identidad | ID card for minors |
| `2` | `CedulaDeCiudadania` | Cédula de Ciudadanía | National ID (most common) |
| `3` | `CedulaDeExtranjeria` | Cédula de Extranjería | Foreign resident ID |
| `4` | `Pasaporte` | Pasaporte | Passport |
| `5` | `NumeroDeIdentificacionTributaria` | NIT - Número de Identificación Tributaria | Tax ID number |
| `6` | `PermisoEspecialDePermanencia` | Permiso Especial de Permanencia | Special stay permit (PEP) |

---

### AccidentSeverity

**File:** `Enums/AccidentSeverity.cs`
**Namespace:** `sicoain.shared.Enums`
**Storage:** `int`

Severity classification for workplace accidents/incidents.

| Value | Name | Display Name | Description |
|-------|------|-------------|-------------|
| `0` | `Incident` | Incidente (Sin Lesión) | No injury occurred |
| `1` | `Mild` | Leve | Minor injury |
| `2` | `Moderate` | Moderado | Moderate injury |
| `3` | `Severe` | Grave | Serious injury |
| `4` | `Critico` | Muy Grave / Mortal | Critical or fatal |

---

### Priority

**File:** `Enums/Priority.cs`
**Namespace:** `sicoain.shared.Enums`
**Storage:** `int`

Urgency level for corrective actions.

| Value | Name | Display Name | Description |
|-------|------|-------------|-------------|
| `0` | `Low` | Baja | Low priority |
| `1` | `Medium` | Media | Medium priority |
| `2` | `High` | Alta | High priority |
| `3` | `Critical` | Crítica | Critical — immediate action required |

---

### StatusAction

**File:** `Enums/StatusAction.cs`
**Namespace:** `sicoain.shared.Enums`
**Storage:** `int`

Lifecycle status for corrective actions.

| Value | Name | Display Name | Description |
|-------|------|-------------|-------------|
| `0` | `Rejected` | Rechazada | Proposal rejected |
| `1` | `Proposal` | Propuesta | Proposed, pending approval |
| `2` | `Approved` | Aprobada | Approved for execution |
| `3` | `InProcess` | En proceso | Currently being executed |
| `4` | `Completed` | Completada | Completed |

---

### AttachmentEntityType

**File:** `Enums/AttachmentEntityType.cs`
**Namespace:** `sicoain.shared.Enums`
**Storage:** `int`

Polymorphic discriminator for the `Attachment` entity.

| Value | Name | Display Name | Description |
|-------|------|-------------|-------------|
| `1` | `Accident` | Accidente/Incidente | Attachment belongs to an Accident |
| `2` | `CorrectiveAction` | Acción correctiva | Attachment belongs to a CorrectiveAction |
| `3` | `Witness` | Testigo | Attachment belongs to a Witness |
| `4` | `Employee` | Empleado | Attachment belongs to an Employee |

---

### PhoneType

**File:** `Enums/PhoneType.cs`
**Namespace:** `sicoain.shared.Enums`
**Storage:** `int`

Classification of phone numbers in polymorphic contact entities.

| Value | Name | Display Name | Description |
|-------|------|-------------|-------------|
| `0` | `Mobile` | Celular | Mobile/cell phone |
| `1` | `Home` | Casa | Home phone |
| `2` | `Work` | Trabajo | Work phone |
| `3` | `Other` | Otro | Other type |

---

## DTO Catalog

All DTOs are C# `record` types with `init` setters (immutable by convention). They live under the `sicoain.shared.DTOs` namespace hierarchy.

### BaseDto (abstract record)

**File:** `DTOs/BaseDto.cs`
**Namespace:** `sicoain.shared.DTOs`
**Attributes:** `abstract record`

Foundation for all response DTOs — mirrors `BaseEntity` audit fields.

#### Properties

| Property | Type | Access |
|----------|------|--------|
| `Id` | `int` | `get; init` |
| `CreatedAt` | `DateTime` | `get; init` |
| `UpdatedAt` | `DateTime?` | `get; init` |
| `DeletedAt` | `DateTime?` | `get; init` |
| `CreatedBy` | `int` | `get; init` |
| `UpdatedBy` | `int` | `get; init` |
| `DeletedBy` | `int?` | `get; init` |

---

### PagedResponse\<T\>

**File:** `DTOs/PagedResponse.cs`
**Namespace:** `sicoain.shared.DTOs`
**Attributes:** `record`

Generic paginated response wrapper used by all list endpoints.

#### Properties

| Property | Type | Access | Description |
|----------|------|--------|-------------|
| `Items` | `List<T>` | `get; init` | Page items |
| `TotalCount` | `int` | `get; init` | Total items across all pages |
| `PageNumber` | `int` | `get; init` | Current page number |
| `PageSize` | `int` | `get; init` | Items per page |
| `TotalPages` | `int` | `get` (computed) | `Math.Ceiling(TotalCount / PageSize)` |
| `HasPreviousPage` | `bool` | `get` (computed) | `PageNumber > 1` |
| `HasNextPage` | `bool` | `get` (computed) | `PageNumber < TotalPages` |

---

### Auth DTOs

**Namespace:** `sicoain.shared.DTOs`

#### LoginRequest

**File:** `DTOs/LoginRequest.cs`
**Type:** `record`

| Property | Type | Attributes |
|----------|------|------------|
| `Email` | `string` | `[Required]`, `[EmailAddress]` |
| `Password` | `string` | `[Required]`, `[MinLength(8)]`, `[MaxLength(32)]` |

#### AuthResponse

**File:** `DTOs/AuthResponse.cs`
**Type:** `record`

| Property | Type |
|----------|------|
| `Success` | `bool` |
| `Message` | `string?` |
| `Email` | `string?` |
| `FullName` | `string?` |
| `ExpiresAt` | `DateTime?` |

#### RefreshTokenRequest

**File:** `DTOs/RefreshTokenRequest.cs`

#### RevokeTokenRequest

**File:** `DTOs/RevokeTokenRequest.cs`

---

### User DTOs

**Namespace:** `sicoain.shared.DTOs.Users`

#### UserDto

**File:** `DTOs/Users/UserDto.cs`
**Base:** `BaseDto`
**Type:** `record`

| Property | Type | Default |
|----------|------|---------|
| `Email` | `string` | `string.Empty` |
| `FullName` | `string` | `string.Empty` |
| `IsActive` | `bool` | `false` |

#### CreateUserRequest

**File:** `DTOs/Users/CreateUserRequest.cs`

#### UpdateUserRequest

**File:** `DTOs/Users/UpdateUserRequest.cs`

#### ChangePasswordRequest

**File:** `DTOs/Users/ChangePasswordRequest.cs`

#### AssignOrRemoveRoleRequest

**File:** `DTOs/Users/AssignOrRemoveRoleRequest.cs`

#### UserListResponse

**File:** `DTOs/Users/UserListResponse.cs`

---

### Accident DTOs

**Namespace:** `sicoain.shared.DTOs.Accident` *(note: singular namespace)*

#### AccidentDto

**File:** `DTOs/Accidents/AccidentDto.cs`
**Base:** `BaseDto`
**Type:** `record`

| Property | Type | Default |
|----------|------|---------|
| `EventDate` | `DateTime` | — |
| `Description` | `string` | `string.Empty` |
| `EmployeeId` | `int` | — |
| `EmployeeFullname` | `string` | `string.Empty` |
| `AccidentTypeId` | `int` | — |
| `AccidentTypeName` | `string` | `string.Empty` |
| `EventCategoryId` | `int` | — |
| `EventCategoryName` | `string` | `string.Empty` |

#### CreateAccidentRequest

**File:** `DTOs/Accidents/CreateAccidentRequest.cs`

#### UpdateAccidentRequest

**File:** `DTOs/Accidents/UpdateAccidentRequest.cs`

---

### Entity DTOs (by namespace)

Each entity in the domain model has a corresponding DTO directory under `DTOs/`. The pattern is:

```
DTOs/{EntityName}/
├── {EntityName}Dto.cs              (extends BaseDto)
├── Create{EntityName}Request.cs
└── Update{EntityName}Request.cs
```

Some entities also have email/phone sub-DTOs (e.g., `DTOs/Business/BusinessEmailDto.cs`, `DTOs/Business/BusinessPhoneDto.cs`).

The complete list of entity DTO directories:

| Directory | Files | Key DTO |
|-----------|-------|---------|
| `DTOs/Accidents/` | 3 | `AccidentDto` |
| `DTOs/AccidentTypes/` | 3 | `AccidentTypeDto` |
| `DTOs/Attachments/` | 3 | `AttachmentDto` |
| `DTOs/Branches/` | 5+ | `BranchDto` + email/phone DTOs |
| `DTOs/Business/` | 5+ | `BusinessDto` + email/phone DTOs |
| `DTOs/CorrectiveActions/` | 3 | `CorrectiveActionDto` |
| `DTOs/CorrectiveActionTrackings/` | 1+ | `CorrectiveActionTrackingDto` |
| `DTOs/Departments/` | 3 | `DepartmentDto` |
| `DTOs/DigitalEvidences/` | 3 | `DigitalEvidenceDto` |
| `DTOs/EmployeeContacts/` | 5+ | `EmployeeContactDto` + email/phone DTOs |
| `DTOs/Employees/` | 5+ | `EmployeeDto` + email/phone DTOs |
| `DTOs/EventCategories/` | 3 | `EventCategoryDto` |
| `DTOs/HealthPromotionEntities/` | 5+ | `HealthPromotionEntityDto` + email/phone DTOs |
| `DTOs/OccupationalRiskAdministrators/` | 5+ | `OccupationalRiskAdministratorDto` + email/phone DTOs |
| `DTOs/Positions/` | 3 | `PositionDto` |
| `DTOs/RiskClasses/` | 3 | `RiskClassDto` |
| `DTOs/Users/` | 7 | `UserDto` + request DTOs |
| `DTOs/Witnesses/` | 3 | `WitnessDto` |

---

### Shared Base DTOs

Base request/response types for the polymorphic email/phone pattern.

| File | Type | Properties |
|------|------|------------|
| `DTOs/CreateEntityEmailRequest.cs` | `record` | Base email creation |
| `DTOs/CreateEntityPhoneRequest.cs` | `record` | Base phone creation |
| `DTOs/UpdateEntityEmailRequest.cs` | `record` | Base email update |
| `DTOs/UpdateEntityPhoneRequest.cs` | `record` | Base phone update |
| `DTOs/EntityEmailDto.cs` | `record` | Base email DTO |
| `DTOs/EntityPhoneDto.cs` | `record` | Base phone DTO |

---

## Constants Catalog

### AppPermissions

**File:** `Constants/AppPermissions.cs`
**Namespace:** `sicoain.shared.Constants`
**Type:** `static class`

All permission strings are `public const string` fields, organized by module.

#### Accidents Module

| Constant | Value |
|----------|-------|
| `AccidentsView` | `"Accidents.View"` |
| `AccidentsCreate` | `"Accidents.Create"` |
| `AccidentsEdit` | `"Accidents.Edit"` |
| `AccidentsDelete` | `"Accidents.Delete"` |
| `AccidentsApprove` | `"Accidents.Approve"` |

#### Employees Module

| Constant | Value |
|----------|-------|
| `EmployeesView` | `"Employees.View"` |
| `EmployeesCreate` | `"Employees.Create"` |
| `EmployeesEdit` | `"Employees.Edit"` |
| `EmployeesDelete` | `"Employees.Delete"` |

#### Reports Module

| Constant | Value |
|----------|-------|
| `ReportsView` | `"Reports.View"` |
| `ReportsExport` | `"Reports.Export"` |

#### Users Module

| Constant | Value |
|----------|-------|
| `UsersView` | `"Users.View"` |
| `UsersCreate` | `"Users.Create"` |
| `UsersEdit` | `"Users.Edit"` |
| `UsersDelete` | `"Users.Delete"` |
| `UsersAssignRoles` | `"Users.AssignRoles"` |

#### Settings Module

| Constant | Value |
|----------|-------|
| `SettingsView` | `"Settings.View"` |
| `SettingsEdit` | `"Settings.Edit"` |

#### Superadmin

| Constant | Value |
|----------|-------|
| `PermissionsManage` | `"Settings.Manage"` |

**Total: 18 permission constants.**

---

## Interfaces

**Directory:** `Interfaces/` (currently empty)

The `Interfaces/` directory is reserved for future repository abstraction interfaces. At present, the API project (`sicoain.api`) directly uses EF Core for data access via `ApplicationDbContext`.

---

## Namespace Map

| Namespace | Location | Contents |
|-----------|----------|----------|
| `sicoain.shared` | Root | Project assembly |
| `sicoain.shared.Constants` | `Constants/` | `AppPermissions` |
| `sicoain.shared.DTOs` | `DTOs/` | `BaseDto`, `PagedResponse<T>`, auth DTOs, shared base DTOs |
| `sicoain.shared.DTOs.Accident` | `DTOs/Accidents/` | Accident DTOs |
| `sicoain.shared.DTOs.AccidentTypes` | `DTOs/AccidentTypes/` | AccidentType DTOs |
| `sicoain.shared.DTOs.Attachments` | `DTOs/Attachments/` | Attachment DTOs |
| `sicoain.shared.DTOs.Branches` | `DTOs/Branches/` | Branch DTOs |
| `sicoain.shared.DTOs.Business` | `DTOs/Business/` | Business DTOs |
| `sicoain.shared.DTOs.CorrectiveActions` | `DTOs/CorrectiveActions/` | CorrectiveAction DTOs |
| `sicoain.shared.DTOs.CorrectiveActionTrackings` | `DTOs/CorrectiveActionTrackings/` | CorrectiveActionTracking DTOs |
| `sicoain.shared.DTOs.Departments` | `DTOs/Departments/` | Department DTOs |
| `sicoain.shared.DTOs.DigitalEvidences` | `DTOs/DigitalEvidences/` | DigitalEvidence DTOs |
| `sicoain.shared.DTOs.EmployeeContacts` | `DTOs/EmployeeContacts/` | EmployeeContact DTOs |
| `sicoain.shared.DTOs.Employees` | `DTOs/Employees/` | Employee DTOs |
| `sicoain.shared.DTOs.EventCategories` | `DTOs/EventCategories/` | EventCategory DTOs |
| `sicoain.shared.DTOs.HealthPromotionEntities` | `DTOs/HealthPromotionEntities/` | HealthPromotionEntity DTOs |
| `sicoain.shared.DTOs.OccupationalRiskAdministrators` | `DTOs/OccupationalRiskAdministrators/` | OR Administrator DTOs |
| `sicoain.shared.DTOs.Positions` | `DTOs/Positions/` | Position DTOs |
| `sicoain.shared.DTOs.RiskClasses` | `DTOs/RiskClasses/` | RiskClass DTOs |
| `sicoain.shared.DTOs.Users` | `DTOs/Users/` | User DTOs |
| `sicoain.shared.DTOs.Witnesses` | `DTOs/Witnesses/` | Witness DTOs |
| `sicoain.shared.Entities` | `Entities/` | All 37 entity classes |
| `sicoain.shared.Enums` | `Enums/` | All 6 enum types |

---

*Generated from source code analysis. All entity properties, DTO records, enum values, and constants reflect the actual implementation in `src/sicoain.shared/`.*
