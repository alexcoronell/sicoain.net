# SICOAIN — Sistema de Control de Accidentes e Incidentes

> **A full-stack platform for occupational accident and incident control, investigation, and regulatory compliance in Colombia**
>
> Monorepo containing a .NET 10 Web API, Blazor WebAssembly client, shared domain library, and comprehensive test suites (unit + integration) for managing workplace accidents, digital evidence, corrective actions, and regulatory compliance with ARL/EPS entities.

[![.NET](https://img.shields.io/badge/.NET-10.0.7-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com)
[![C#](https://img.shields.io/badge/C%23-13-239120?style=flat-square&logo=csharp)](https://learn.microsoft.com/en-us/dotnet/csharp)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-10.0.7-512BD4?style=flat-square&logo=dotnet)](https://learn.microsoft.com/en-us/aspnet/core)
[![Blazor](https://img.shields.io/badge/Blazor%20WASM-10.0.4-512BD4?style=flat-square&logo=blazor)](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor)
[![EF Core](https://img.shields.io/badge/EF%20Core-10.0.7-512BD4?style=flat-square&logo=dotnet)](https://learn.microsoft.com/en-us/ef/core)
[![JWT](https://img.shields.io/badge/Auth-JWT%20Bearer-000000?style=flat-square&logo=jsonwebtokens)](https://jwt.io)
[![xUnit](https://img.shields.io/badge/xUnit-2.9.3-1E8E3E?style=flat-square&logo=celery)](https://xunit.net)
[![FluentValidation](https://img.shields.io/badge/Validation-FluentValidation%2012.1.1-E83533?style=flat-square)](https://docs.fluentvalidation.net)
[![Swagger](https://img.shields.io/badge/API-Swagger%207.0.0-85EA2D?style=flat-square&logo=swagger)](https://swashbuckle.AspNetCore)
[![SQL Server](https://img.shields.io/badge/Database-SQL%20Server%202022-CC2927?style=flat-square&logo=microsoftsqlserver)](https://www.microsoft.com/en-us/sql-server)
[![Docker](https://img.shields.io/badge/Infra-Docker%20Compose-2496ED?style=flat-square&logo=docker)](https://www.docker.com)
[![Testcontainers](https://img.shields.io/badge/Testcontainers-Integration-5063C5?style=flat-square&logo=docker)](https://testcontainers.com)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](LICENSE)

---

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Projects](#projects)
- [Technology Stack](#technology-stack)
- [Quick Start](#quick-start)
- [Development Workflow](#development-workflow)
- [Authentication & Authorization](#authentication--authorization)
- [Database](#database)
- [Testing](#testing)
- [Project Statistics](#project-statistics)
- [Project READMEs](#project-readmes)
- [Contributing](#contributing)
- [License](#license)

---

## Overview

**SICOAIN** (Sistema de Control de Accidentes e Incidentes) is a full-stack web application designed to digitize and streamline occupational accident and incident control in Colombian workplaces.

### What It Does

- **Accident & Incident Tracking** — Register, classify, and manage workplace accidents with full digital evidence chain of custody
- **Employee Management** — Maintain worker profiles with Colombian identity documents (CC, CE, NIT, PEP), medical history, EPS/ARL affiliations, and job positions
- **Corrective Actions** — Track remediation plans with status lifecycle, priority levels, and effectiveness verification
- **Digital Evidence** — Store investigative evidence (photos, documents) with SHA-256 hashing and chain of custody logging
- **Role-Based Access Control** — Granular permission system with dynamic policies per module/action (View, Create, Edit, Delete, Approve)
- **Regulatory Compliance** — Native support for Colombian entities: ARL (Occupational Risk Administrators), EPS (Health Promotion Entities), and Colombian document types

### Who It's For

- **Occupational Health & Safety departments** in Colombian companies
- **ARL investigators** managing workplace accident reports
- **Company supervisors** tracking incidents and corrective actions
- **External consultants** generating reports and analytics

---

## Architecture

```
┌──────────────────────────────────────────────────────────────────────────┐
│                            HTTP / HTTPS                                   │
└──────────────────────────────────────────────────────────────────────────┘
                                    │
                   ┌────────────────┴────────────────┐
                   │                                 │
                   ▼                                 ▼
┌──────────────────────────────┐    ┌──────────────────────────────────┐
│   sicoain.client             │    │   sicoain.api                   │
│   Blazor WebAssembly         │◀──▶│   ASP.NET Core Web API          │
│   .NET 10.0                  │    │   .NET 10.0                     │
│                              │    │                                  │
│   Pages/                     │    │   Controllers/  (19)             │
│   Layout/                    │    │   Services/     (24)             │
│                              │    │   Abstractions/ (26)             │
│                              │    │   Validators/   (40+)            │
│                              │    │   Data/ (EF Core + Identity)    │
│                              │    │   Repositories/                  │
└──────────────────────────────┘    └──────────────┬───────────────────┘
                                                    │
                                                    ▼
                              ┌─────────────────────────────────────────┐
                              │   sicoain.shared                       │
                              │   Shared Domain Layer                  │
                              │   .NET 10.0 Class Library              │
                              │                                         │
                              │   Entities/     (37)                    │
                              │   DTOs/         (~100)                  │
                              │   Enums/        (6)                     │
                              │   Constants/    (18 permissions)        │
                              └─────────────────────────────────────────┘
                                                    │
                                                    ▼
                              ┌─────────────────────────────────────────┐
                              │   SQL Server 2022                       │
                              │   (via Docker Compose)                  │
                              │                                         │
                              │   Identity Tables (7)                   │
                              │   Business Tables  (25+)                │
                              └─────────────────────────────────────────┘
```

### Architectural Highlights

| Pattern | Implementation |
|---------|---------------|
| **Monorepo** | Single solution (`Sicoain.net.slnx`) with 4 projects |
| **Clean Architecture** | Domain layer (`shared`) isolated from infrastructure (`api`, `client`) |
| **Generic CRUD** | `BaseService<T>` + `BaseCrudController<T>` pattern eliminates ~80% boilerplate |
| **Repository Pattern** | `RefreshTokenRepository` abstracts token persistence |
| **DTO Layer** | Immutable C# `record` types separate domain from API contracts |
| **Soft Delete** | `IsDeleted` flag on all entities with reflection-based detection |
| **Token Rotation** | OWASP-compliant refresh token rotation with revocation chain |
| **Dynamic Authorization** | Permission policies loaded from database at startup |

---

## Projects

### 1. `sicoain.api` — ASP.NET Core Web API

[![README](https://img.shields.io/badge/README-details-512BD4?style=flat-square)](src/sicoain.api/README.md)

The RESTful backend exposing versioned HTTP endpoints (`/api/v1/`). Handles authentication, authorization, business logic, and data persistence.

| Metric | Count |
|--------|-------|
| Controllers | 19 |
| Services | 24 |
| Abstractions (interfaces) | 26 |
| Validators (FluentValidation) | 40+ |
| Seeders | 3 |
| Source files | ~136 `.cs` |

**Key features:** JWT in HttpOnly cookies, refresh token rotation, dynamic permission-based authorization, CSRF protection, API versioning, generic CRUD, FluentValidation integration, Swagger documentation.

### 2. `sicoain.client` — Blazor WebAssembly

[![README](https://img.shields.io/badge/README-coming%20soon-gray?style=flat-square)](src/sicoain.client/README.md)

The frontend SPA consuming the API. Built with .NET 10 Blazor WebAssembly.

| Metric | Count |
|--------|-------|
| Pages | 5 |
| Layouts | 2 |
| Source files | 8 `.razor` + 1 `.cs` |

**Current state:** Scaffolding with sample pages (Home, Counter, Weather, Not Found). Ready for feature implementation.

### 3. `sicoain.shared` — Shared Domain Library

[![README](https://img.shields.io/badge/README-details-512BD4?style=flat-square)](src/sicoain.shared/README.md)

The shared domain layer serving as the single source of truth across all projects. Contains entities, DTOs, enums, and constants.

| Metric | Count |
|--------|-------|
| Entity classes | 37 |
| Enum types | 6 |
| DTO records | ~100 |
| Permission constants | 18 |
| Source files | ~145 `.cs` |

**Key entities:** `BaseEntity` (audit + soft delete), `User : IdentityUser<int>`, `Business`, `Branch`, `Employee`, `Accident`, `DigitalEvidence`, `CorrectiveAction`, `RefreshToken`, `Permissions`, `Roles`.

### 4. `sicoain.UnitTests` — xUnit Test Suite

[![README](https://img.shields.io/badge/README-details-512BD4?style=flat-square)](tests/sicoain.UnitTests/README.md)

Comprehensive unit test suite covering the entire service layer and all input validators.

| Metric | Count |
|--------|-------|
| Total tests | **766** |
| Test files | 78 |
| Service tests | 120 |
| Validator tests | 645 |
| Test framework | xUnit 2.9.3 |
| Coverage collector | coverlet 6.0.4 |

**Testing patterns:** Mock-based (Moq 4.20.72) for auth utilities, EF Core InMemory for integration-style service tests, FluentAssertions 10.0.0 for expressive assertions.

### 5. `sicoain.IntegrationTests` — xUnit Integration Test Suite

[![README](https://img.shields.io/badge/README-details-512BD4?style=flat-square)](tests/sicoain.IntegrationTests/README.md)

End-to-end integration test suite exercising all 17 API controllers against a real SQL Server database via Docker Compose.

| Metric | Count |
|--------|-------|
| Total tests | **347** |
| Passed | 306 |
| Skipped | 41 |
| Controller test files | 17 |
| Test framework | xUnit 2.9.3 |
| Database | SQL Server 2022 (Docker) |
| Infrastructure | Docker Compose (`docker-compose.test.yml`) |

**Scope:** Every API endpoint is exercised through its full HTTP lifecycle — validation, business logic, authorization, and error handling. Tests authenticate via real login endpoint, exercise CSRF protection, and validate against a per-class isolated test database.

**Key patterns:**
- Custom `WebApplicationFactory<Program>` with mocked `IAntiforgery` (bypasses CSRF while keeping the filter pipeline)
- Cookie-based auth via real `/api/auth/login` with a custom `CookieHandler` DelegatingHandler (works around `CookieContainer` Secure-flag bug over HTTP)
- `BuildCreateForm()` / `BuildUpdateForm()` helpers for reusable authenticated request setup
- `CreateUnauthenticatedClientAsync()` for authorization policy enforcement tests
- `IAsyncLifetime` for per-class database creation/destruction via EF Core migrations

**Controllers covered:** Accidents, AccidentTypes, Attachments, Auth, Branches, Businesses, CorrectiveActions, Departments, DigitalEvidences, Employees, EventCategories, HealthPromotionEntities, OccupationalRiskAdministrators, Positions, RiskClasses, User, Witnesses.

---

## Technology Stack

| Category | Technology | Version | Project |
|----------|-----------|---------|---------|
| **Runtime** | .NET | `10.0` | All |
| **Language** | C# | `13` | All |
| **Web Framework** | ASP.NET Core | `10.0.7` | API |
| **Frontend** | Blazor WebAssembly | `10.0.4` | Client |
| **ORM** | Entity Framework Core | `10.0.7` | API |
| **Database** | SQL Server 2022 | Developer | API |
| **Identity** | ASP.NET Core Identity | `10.0.7` | API |
| **Auth Tokens** | JWT Bearer | `10.0.7` | API |
| **Validation** | FluentValidation | `12.1.1` | API |
| **Object Mapping** | AutoMapper | `16.1.1` | API |
| **API Docs** | Swashbuckle (Swagger) | `7.0.0` | API |
| **API Versioning** | Asp.Versioning.Mvc | `10.0.0` | API |
| **Test Framework** | xUnit | `2.9.3` | Tests |
| **Assertions** | FluentAssertions | `8.10.0` | Tests |
| **Mocking** | Moq | `4.20.72` | Unit Tests |
| **Code Coverage** | coverlet | `6.0.4` | Tests |
| **Integration Test Framework** | WebApplicationFactory | `10.0.7` | Integration Tests |
| **Integration DB** | Docker Compose + SQL Server | `2022-latest` | Integration Tests |
| **Auth Bypass** | Custom DelegatingHandler (CookieHandler) | — | Integration Tests |
| **Static Analysis** | Meziantou.Analyzer | `3.0.69` | API |
| **Security Analysis** | SecurityCodeScan | `5.6.7` | API |
| **Infrastructure** | Docker Compose | — | Root |
| **Code Style** | EditorConfig | — | Root |

---

## Quick Start

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for SQL Server)
- [Visual Studio 2025](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/) + [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)

### 1. Start SQL Server

```bash
docker compose up -d
```

This starts SQL Server 2022 Developer on port `1433` with persistent storage. The container includes SQL Server Agent for scheduled tasks.

### 2. Update the database

```bash
dotnet ef database update --project src/sicoain.api
```

The application will also run seeders automatically on startup:
- Create Identity roles (Admin, Investigator, Supervisor, Consultant)
- Synchronize Identity roles with the custom `Roles` table
- Seed 18 permissions from `AppPermissions` constants
- Assign permissions to roles

### 3. Run the API

```bash
dotnet run --project src/sicoain.api --launch-profile https
```

The API starts at:
- **HTTPS:** `https://localhost:7241`
- **HTTP:** `http://localhost:5078`
- **Swagger UI:** `https://localhost:7241/swagger`

### 4. Run the Client (optional)

```bash
dotnet run --project src/sicoain.client
```

### 5. Run the Tests

```bash
dotnet test
```

All **1,113 tests** (766 unit + 347 integration) should pass.  

> **Note:** Integration tests require the test SQL Server container to be running:
> ```bash
> docker compose -f docker-compose.test.yml up -d
> dotnet test tests/sicoain.IntegrationTests
> ```

### Docker Compose Services

```yaml
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: sicoain-sqlserver
    environment:
      SA_PASSWORD: '51c04in!2024'
      MSSQL_PID: 'Developer'
    ports:
      - '1433:1433'
    volumes:
      - sqlserver_data:/var/opt/mssql
```

---

## Development Workflow

### Branch Convention

```
master                    → Production-ready code
feat/feature-name          → New features
fix/issue-description      → Bug fixes
docs/project-readmes       → Documentation
test/test-scope            → Test additions/changes
```

### Commit Convention

This project uses [Conventional Commits](https://www.conventionalcommits.org/):

```
feat: add accident severity classification
fix: resolve race condition in file evidence upload
docs: add comprehensive README for API project
test: add ChangePasswordAsync tests to UserServiceTests
refactor: convert DTOs to records for immutability
chore: update FluentValidation to 12.1.1
```

### Build Verification

```bash
dotnet build                                    # Build all projects
dotnet build --no-restore                       # Skip restore for speed
dotnet test tests/sicoain.UnitTests             # Run all 766 unit tests
dotnet test tests/sicoain.IntegrationTests      # Run all 347 integration tests (requires Docker)
dotnet format                                   # Enforce code style (pre-commit hook)
```

The repository uses **Husky** for Git hooks, running `dotnet format` and `dotnet test` before every commit.

---

## Authentication & Authorization

```
┌──────────────┐     ┌──────────────────┐     ┌──────────────────┐
│   Login      │────▶│   JWT (15 min)   │────▶│   HttpOnly       │
│   (Email +   │     │   + Refresh      │     │   Cookie          │
│    Password) │     │   Token (7 days) │     │   + CSRF Token   │
└──────────────┘     └──────────────────┘     └──────────────────┘
                            │
                            ▼
                    ┌──────────────────┐
                    │   Permission     │
                    │   Claims in JWT  │
                    │   "Permission":  │
                    │   "Accidents.View│
                    └──────────────────┘
                            │
                            ▼
                    ┌──────────────────┐
                    │   Authorization  │
                    │   Policy check   │
                    │   [Authorize(    │
                    │    Policy =      │
                    │    "Accidents.   │
                    │    Create")]     │
                    └──────────────────┘
```

### Auth Flow

1. **Login:** User provides email + password → server validates via `SignInManager` → generates JWT (15 min) + refresh token (7 days, 64-byte random) → stores both as **HttpOnly cookies**
2. **Authenticated Request:** Browser sends `access_token` cookie automatically → JWT middleware reads from cookie (not `Authorization` header)
3. **Token Refresh:** When JWT expires → client calls `/Auth/refresh` → server revokes old refresh token → issues new pair (rotation)
4. **Authorization:** Each endpoint checks a specific permission via `[Authorize(Policy = "Accidents.View")]` → JWT contains `"Permission"` claims
5. **Logout:** Revokes refresh token in database → clears cookies

### Security Measures

| Measure | Implementation |
|---------|---------------|
| **Token Storage** | HttpOnly cookies (inaccessible to JavaScript) |
| **CSRF Protection** | Antiforgery token via `X-CSRF-TOKEN` header |
| **Refresh Rotation** | OWASP pattern — old token revoked on each refresh |
| **Reuse Detection** | Revoked token presented → possible theft → revoke entire family |
| **Account Lockout** | 5 failed attempts → 15 min lockout |
| **Password Policy** | 8+ chars, digit, upper, lower, non-alphanumeric |
| **CORS** | Restricted to `localhost:5000`, `localhost:5001` |
| **Static Analysis** | SecurityCodeScan, Meziantou.Analyzer, .NET Analyzers |

---

## Database

### Schema Overview

The database is managed by **EF Core Migrations** on SQL Server 2022.

**Identity tables** (renamed from `AspNet*` prefix):

| Table | Purpose |
|-------|---------|
| `Users` | Application users (`IdentityUser<int>`) |
| `Roles` | Identity roles |
| `UserRoles` | User-role assignments |
| `UserClaims` | User claim values |
| `UserLogins` | External login providers |
| `UserTokens` | Authentication tokens |
| `RoleClaims` | Role claim values |

**Business tables** (25+ tables):

| Category | Tables |
|----------|--------|
| **Organizational** | `Businesses`, `Branches`, `Departments`, `Positions`, `RiskClasses` |
| **Employee** | `Employees`, `EmployeePhones`, `EmployeeEmails`, `EmployeeContacts`, contact tables |
| **Accident** | `Accidents`, `AccidentTypes`, `EventCategories`, `DigitalEvidences`, `Witnesses` |
| **Remediation** | `CorrectiveActions`, `CorrectiveActionTrackings` |
| **External Entities** | `HealthPromotionEntities`, `OccupationalRiskAdministrators` + contact tables |
| **Auth** | `RefreshTokens`, `Permissions`, `CustomRoles`, `RolePermissions` |
| **Files** | `Attachments` |

### Entity Relationships

```
Business 1──N──▶ Branch 1──N──▶ Employee 1──N──▶ Accident
                                    │                ├──N── DigitalEvidence
                                    │                ├──N── Witness
                                    │                └──N── CorrectiveAction
                                    │                        └──N── CorrectiveActionTracking
                                    │
                                    N──1──▶ HealthPromotionEntity (EPS)
                                    N──1──▶ OccupationalRiskAdministrator (ARL)
                                    N──1──▶ Position N──1──▶ Department
                                                         N──1──▶ RiskClass
```

---

## Testing

[![Unit Tests](https://img.shields.io/badge/Unit-766%20tests-1E8E3E?style=flat-square)](tests/sicoain.UnitTests/README.md)
[![Integration Tests](https://img.shields.io/badge/Integration-347%20tests-5063C5?style=flat-square)](tests/sicoain.IntegrationTests/README.md)

The project contains **two test suites** totaling **1,113 tests** across 95 test files.

### Unit Tests (`sicoain.UnitTests`)

[![README](https://img.shields.io/badge/README-details-512BD4?style=flat-square)](tests/sicoain.UnitTests/README.md)

Covers **100% of service interfaces** (23 services × 23 test files) and **100% of validators** (55 validator test files).

```bash
# Run unit tests
dotnet test tests/sicoain.UnitTests

# Run with detailed output
dotnet test tests/sicoain.UnitTests --verbosity detailed

# Run specific test class
dotnet test tests/sicoain.UnitTests --filter "FullyQualifiedName~UserServiceTests"

# Generate coverage report
dotnet test tests/sicoain.UnitTests --collect:"XPlat Code Coverage"
```

### Integration Tests (`sicoain.IntegrationTests`)

[![README](https://img.shields.io/badge/README-details-512BD4?style=flat-square)](tests/sicoain.IntegrationTests/README.md)

End-to-end HTTP-level tests exercising all 17 API controllers against a real SQL Server instance.

```bash
# Start the test database
docker compose -f docker-compose.test.yml up -d

# Run integration tests
dotnet test tests/sicoain.IntegrationTests

# Run tests for a specific controller
dotnet test tests/sicoain.IntegrationTests --filter "FullyQualifiedName~AccidentsControllerTests"
```

### Testing Patterns

| Suite | Pattern | Tools | Used For |
|-------|---------|-------|----------|
| Unit | **Mock-based** | Moq | Auth utilities (CookieManager, JwtTokenGenerator, IpAddressProvider, RefreshTokenGenerator) |
| Unit | **EF InMemory** | EF Core InMemory | Service layer with full CRUD (UserService, AccidentService, BaseService entities) |
| Unit | **Testable inner classes** | Virtual methods | File path generation, date-dependent logic |
| Unit | **Direct validation** | FluentValidation test | All 40+ validators with valid/invalid cases |
| Integration | **Real HTTP + DB** | WebApplicationFactory, Docker Compose | Full HTTP lifecycle (validation, auth, business logic, error handling) |
| Integration | **Cookie auth** | Custom DelegatingHandler | Authentication via real `/api/auth/login` endpoint |
| Integration | **CSRF bypass** | Mocked `IAntiforgery` | Maintains filter pipeline while bypassing token challenge |
| Integration | **Per-class DB** | `IAsyncLifetime` + EF migrations | Isolated test databases created/destroyed per test class |

---

## Project Statistics

```
┌─────────────────────────────────────────────────────────────────────┐
│                     SICOAIN.net — Codebase Stats                      │
├─────────────────────────────────────────────────────────────────────┤
│                                                                      │
│  Total source files:          ~361  (.cs + .razor)                   │
│  Solution projects:           5                                      │
│  Test count:                  1,113 (95 files)                       │
│    ├─ Unit tests:             766   (78 files)                       │
│    └─ Integration tests:      347   (17 files)                       │
│  AutoMapper profiles:         16                                     │
│  Entity classes:              37                                     │
│  API controllers:             19                                     │
│  API endpoints:               ~80                                    │
│  Permission constants:        18                                     │
│  NuGet packages:              ~26                                    │
│  Git branches:                5+                                     │
│  Commits:                     60+                                    │
│                                                                      │
└─────────────────────────────────────────────────────────────────────┘
```

---

## Project READMEs

Each project has a detailed README with full architecture documentation, usage examples, and design decisions:

| Project | README | Pages |
|---------|--------|-------|
| **sicoain.api** | [📄 src/sicoain.api/README.md](src/sicoain.api/README.md) | ~700 lines |
| **sicoain.client** | [📄 src/sicoain.client/README.md](src/sicoain.client/README.md) | *(coming soon)* |
| **sicoain.shared** | [📄 src/sicoain.shared/README.md](src/sicoain.shared/README.md) | ~734 lines |
| **sicoain.UnitTests** | [📄 tests/sicoain.UnitTests/README.md](tests/sicoain.UnitTests/README.md) | ~416 lines |
| **sicoain.IntegrationTests** | [📄 tests/sicoain.IntegrationTests/README.md](tests/sicoain.IntegrationTests/README.md) | ~688 lines |

---

## Contributing

This project follows a **feature branch workflow** with conventional commits and pre-commit validation.

### Workflow

1. Create a branch from `master`: `git checkout -b feat/your-feature`
2. Make your changes following the established patterns
3. Run tests:
   - Unit: `dotnet test tests/sicoain.UnitTests` (all 766 must pass)
   - Integration: `dotnet test tests/sicoain.IntegrationTests` (all 347 must pass, requires Docker)
4. Commit using conventional commits: `git commit -m "feat: description"`
5. Push and open a Pull Request

### Guidelines

- **No AI attribution** — never add `Co-Authored-By` lines to commits
- **Patterns** — new entities → inherit `BaseEntity`; new DTOs → `record` types inheriting `BaseDto`; new endpoints → extend `BaseCrudController<T>`
- **Validation** — always add FluentValidation validators for new request DTOs
- **Authorization** — add `[Authorize(Policy = ...)]` with the appropriate `AppPermissions` constant
- **Contacts** — use `BaseEntityEmail`/`BaseEntityPhone` for polymorphic contact entities
- **Enums** — include `[Display(Name = "...")]` in Spanish for UI binding

---

## License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE) file for details.

---

<div align="center">

**Built with 🔥 by [sicoain.net](https://sicoain.net)**

*Comprometidos con la seguridad y salud en el trabajo en Colombia 🇨🇴*

</div>
