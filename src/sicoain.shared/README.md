# sicoain.shared

> **Shared domain model library for SG-SST (Sistema de Gestión de Seguridad y Salud en el Trabajo)**
> A robust, production-ready .NET class library providing comprehensive entity models for occupational health and safety management, tailored for Colombian labor regulations.

[![.NET](https://img.shields.io/badge/.NET-8.0-purple?style=flat-square&logo=.net)](https://dotnet.microsoft.com)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?style=flat-square&logo=csharp)](https://learn.microsoft.com/en-us/dotnet/csharp)
[![Entity Framework Core](https://img.shields.io/badge/EF%20Core-8.0-purple?style=flat-square&logo=microsoft)](https://learn.microsoft.com/en-us/ef/core)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](LICENSE)
[![Platform](https://img.shields.io/badge/Platform-.NET%20Core-blue?style=flat-square)](https://dotnet.microsoft.com)

## Overview

`sicoain.shared` is a domain-driven design (DDD) library that provides the core entities, value objects, and enumerations for building **SG-SST compliant applications** in Colombia. It implements industry-standard patterns including soft-delete, audit trails, and comprehensive relationship modeling between organizations, employees, accidents, and regulatory entities.

This library is designed to be the shared foundation across multiple applications (API, Blazor WebAssembly, Mobile) in the sicoain.net ecosystem.

## Key Features

| Feature | Description |
|---------|-------------|
| **Soft Delete Pattern** | Built-in `IsDeleted` flag and `MarkAsDeleted()` method for safe data removal |
| **Audit Trail** | Automatic tracking of `CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy` |
| **Colombian Documents** | Native support for `Cédula`, `Cédula de Extranjería`, `NIT`, `Pasaporte`, etc. |
| **Domain Relationships** | Full entity navigation properties for Business → Branch → Employee |
| **Digital Evidence** | Chain of custody, file hashing, metadata for accident documentation |
| **Regulatory Compliance** | Ready for ARL (Administradora de Riesgos Laborales) reporting |

## Technology Stack

<div align="center">

| Category | Technology |
|----------|------------|
| **Framework** | [.NET 8.0](https://dotnet.microsoft.com) |
| **Language** | [C# 12.0](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-12) |
| **ORM** | [Entity Framework Core 8.0](https://learn.microsoft.com/en-us/ef/core) |
| **Validation** | [DataAnnotations](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation) |
| **Identity** | [ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity) |

</div>

## Installation

```bash
dotnet add package sicoain.shared
```

Or via the .csproj reference:

```xml
<PackageReference Include="sicoain.shared" Version="1.0.0" />
```

## Domain Model Architecture

### Core Entities

```
┌─────────────────────────────────────────────────────────────────────────────────────────────┐
│                        BASE ENTITY HIERARCHY                              │
├─────────────────────────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│   ┌─────────────┐     ┌─────────────┐     ┌��────────────┐              │
│   │  Business  │────▶│   Branch    │────▶│  Employee  │              │
│   └─────────────┘     └─────────────┘     └─────────────┘              │
│         │                                       │                        │
│         │                                       ▼                        │
│         │                              ┌─────────────────────┐             │
│         └───────────────────────────▶│     Accident         │             │
│                                        └─────────────────────┘             │
│                                              │                           │
│                    ┌───────────────────────────┼──────────────┐             │
│                    │                           │              │             │
│                    ▼                           ▼              ▼             │
│          ┌─────────────────┐    ┌────────────────┐ ┌──────────┐│
│          │ DigitalEvidence  │    │ CorrectiveAction│ │ Witness  ││
│          └─────────────────┘    └────────────────┘ └──────────┘│
│                                                                         │
└─────────────────────────────────────────────────────────────────────────────┘
```

### Entity Overview

| Entity | Description | Key Properties |
|--------|-------------|---------------|
| `BaseEntity` | Abstract base with audit & soft-delete | Id, CreatedAt, UpdatedAt, DeletedAt, IsDeleted |
| `User` | ASP.NET Core Identity user | FullName, Role |
| `Business` | Company/organization | Name, AddressStreet, Phones, Emails, Branches |
| `Branch` | Physical location | Name, AddressStreet, BusinessId, Employees |
| `Employee` | Worker with Colombian documents | DocumentType, DocumentNumber, FullName, Position, MedicalInfo |
| `Accident` | Workplace incident | EventDate, Description, EmployeeId, Severity |
| `AccidentType` | Classification of accident | Name, Description, Severity |
| `EventCategory` | Category of workplace event | Name, Description |
| `CorrectiveAction` | Remediation action | Description, Status, DueDate |
| `DigitalEvidence` | File with chain of custody | FileName, FileHash, MimeType, TakenAt |
| `Witness` | Accident witness | Name, Contact, Statement |
| `Position` | Job position | Name, Description |
| `Department` | Organizational unit | Name, Description |
| `RiskClass` | OSHA risk classification | Name, Level, Description |
| `HealthPromotionEntity` | ARP (Colombian EPS) | Name, Code, Address |
| `OccupationalRiskAdministrator` | ARL administrator | Name, Code, Address |

### Contact Information Pattern

The library uses a normalized contact pattern for multiple entity types:

```
┌─────────────────┐       ┌─────────────────┐       ┌─────────────────┐
│   Business      │       │   Branch        │       │   Employee      │
│   (Empresa)     │       │   (Sucursal)    │       │   (Empleado)    │
└────────┬────────┘       └────────┬───��────┘       └────────┬────────┘
         │                          │                          │
         ▼                        ▼                          ▼
┌─────────────────┐       ┌─────────────────┐       ┌─────────────────┐
│ BusinessPhone    │       │ BranchPhone     │       │ EmployeePhone   │
│ BusinessEmail    │       │ BranchEmail    │       │ EmployeeEmail   │
└─────────────────┘       └─────────────────┘       └─────────────────┘
```

### Enumerations

| Enum | Description | Values |
|------|------------|--------|
| `DocumentType` | Colombian identity documents | CC, CE, TI, NIT, PEP, Pasaporte |
| `AccidentSeverity` | Incident severity level | F只al, Grave, Leve |
| `Priority` | Action priority | Alta, Media, Baja |
| `StatusAction` | Corrective action status | Pendiente, EnProceso, Completada, Verificada |
| `AttachmentEntityType` | Attachment association | Accident, Employee, Evidence, Report |

## Usage Examples

### Defining a DbContext

```csharp
using Microsoft.EntityFrameworkCore;
using sicoain.Shared.Entities;

public class SicoainDbContext : DbContext
{
    public DbSet<Business> Businesses => Set<Business>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Accident> Accidents => Set<Accident>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure BaseEntity as the abstract base
        modelBuilder.Entity<Business>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
        });
        
        // Configure relationships
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Business)
            .WithMany(b => b.Employees)
            .HasForeignKey(e => e.BusinessId);
    }
}
```

### Creating an Employee

```csharp
using sicoain.Shared.Entities;
using sicoain.Shared.Enums;

var employee = new Employee
{
    DocumentType = DocumentType.CedulaDeCiudadania,
    DocumentNumber = "1234567890",
    FirstName = "Juan",
    Surname = "Pérez",
    State = "Cundinamarca",
    Municipality = "Bogotá",
    Neighborhood = "Chapinero",
    AddressStreet = "Carrera 7 # 71 - 21",
    BusinessId = businessId,
    BranchId = branchId,
    PositionId = positionId,
    HealthPromotionEntityId = epsId,
    OccupationalRiskAdministratorId = arlId,
    HiringDate = DateTime.UtcNow
};

// EF Core will auto-set:
// - CreatedAt = DateTime.UtcNow
// - CreatedBy = currentUserId
```

### Querying with Soft Delete

```csharp
// Get all employees excluding soft-deleted
var activeEmployees = await dbContext.Employees
    .Where(e => !e.IsDeleted)
    .Include(e => e.Business)
    .Include(e => e.Position)
    .ToListAsync();

// Soft delete an employee
employee.MarkAsDeleted(currentUserId);
await dbContext.SaveChangesAsync();
```

### Adding Digital Evidence

```csharp
var evidence = new DigitalEvidence
{
    FileName = "accidente_2024_001.pdf",
    FilePath = "/storage/evidence/2024/001.pdf",
    FileSize = "2.5 MB",
    MimeType = "application/pdf",
    FileHash = ComputeSha256(FilePath),
    Description = "Fotografías del accidente laboral",
    TakenAt = DateTime.UtcNow,
    TakenByName = "Instructor Martínez",
    AccidentId = accidentId,
    ChainOfCustody = "Recolectado por: Instructor Martínez | Hora: 14:30"
};
```

## Project Structure

```
src/sicoain.shared/
├── Entities/
│   ├── BaseEntity.cs          # Abstract base with audit + soft-delete
│   ├── User.cs              # ASP.NET Core Identity user
│   ├── Business.cs          # Company organization
│   ├── Branch.cs           # Physical location
│   ├── Employee.cs         # Worker with medical info
│   ├── Accident.cs         # Workplace incident
│   ├── AccidentType.cs    # Accident classification
│   ├── EventCategory.cs   # Event categorization
│   ├── CorrectiveAction.cs # Remediation tracking
│   ├── DigitalEvidence.cs # File evidence with hash
│   ├── Witness.cs         # Accident witness
│   ├── Position.cs       # Job position
│   ├── Department.cs     # Organizational unit
│   ├── RiskClass.cs     # OSHA risk class
│   ├── HealthPromotionEntity.cs  # Colombian EPS
│   ├── OccupationalRiskAdministrator.cs  # Colombian ARL
│   └── Contact/*         # Contact entities (Phone, Email)
├── Enums/
│   ├── DocumentType.cs       # Colombian documents
│   ├── AccidentSeverity.cs   # Severity levels
│   ├── Priority.cs          # Action priority
│   ├── StatusAction.cs    # Action status
│   └── AttachmentEntityType.cs  # Attachment types
├── Interfaces/              # Repository interfaces (planned)
│   └── IEntityRepository.cs
└── sicoain.shared.csproj    # Project file
```

## Best Practices Implemented

### 1. Soft Delete Pattern
```csharp
// Instead of hard deletion, mark as deleted
employee.MarkAsDeleted(currentUserId);
// Query automatically filters: WHERE DeletedBy IS NULL
```

### 2. Audit Trail
```csharp
// Every entity tracks:
// - CreatedAt/UpdatedAt: automatic timestamps
// - CreatedBy/UpdatedBy: user who performed the action
employee.UpdateTimestamps(currentUserId);
```

### 3. Required vs Optional References
```csharp
// Use 'required' keyword for mandatory relationships
public required int BusinessId { get; set; }
public required Business Business { get; set; }

// Use nullable for optional relationships  
public string? AlternativeAddressStreet { get; set; }
```

### 4. Explicit Column Mapping
```csharp
// EF Core column configuration for SQL Server
[Column("event_date", TypeName = "datetime")]
[Column("address_street", TypeName = "varchar(200)")]
```

## Roadmap

- [ ] Add repository interfaces (`IEntityRepository<T>`)
- [ ] Add specification pattern support
- [ ] Add validation helpers
- [ ] Add domain events infrastructure
- [ ] Add migration scripts for SQL Server
- [ ] Add PostgreSQL provider support

## Contributing

Contributions are welcome! Please read our [contributing guidelines](CONTRIBUTING.md) before submitting PRs.

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

---

<div align="center">

**Built with 🔥 by [sicoain.net](https://sicoain.net)**

*Passionate about occupational health and safety in Colombia 🇨🇴*

</div>