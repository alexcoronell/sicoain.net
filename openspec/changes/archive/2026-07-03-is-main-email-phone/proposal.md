# Proposal: Enable IsMain field on Email/Phone entities

## Intent

`IsMain` exists on `BaseEntityEmail` and `BaseEntityPhone` entities but is dead
code — never set or read anywhere in the pipeline. Parent create/update requests
accept raw `List<string>` for emails/phones, losing the ability to designate a
primary contact. This change wires `IsMain` through DTOs, typed request objects,
and service business logic.

## Scope

### In Scope
- Add `IsMain` to all 6 base DTOs (read, create, update for email & phone)
- Replace `List<string>` in 8 parent requests with typed objects carrying `IsMain`
- Update 4 services (Business, Branch, ORA, HPE) to apply IsMain business logic
- AutoMapper convention picks up IsMain automatically — no mapping changes needed

### Out of Scope
- Creating endpoints for per-item DTOs (CreateBusinessEmailRequest, etc.) — existing
  but unimplemented, unrelated
- Employee inline Emails/Phones — Employee already uses separate DTOs; out of scope
- Extracting shared Email/Phone sync logic into BaseService — deferred to spec
- Adding `PhoneType` to parent create items — phones currently hardcoded to Mobile

## Capabilities

No capability-level spec changes — this enhances existing entity behavior, not
new capabilities. Existing workflows (CRUD for Business, Branch, ORA, HPE)
gain IsMain semantics without new endpoints or modules.

### New Capabilities
None

### Modified Capabilities
None

## Approach

### DTO layer
1. Add `bool IsMain` to 6 base DTOs:
   - `EntityEmailDto`, `EntityPhoneDto`
   - `CreateEntityEmailRequest`, `CreateEntityPhoneRequest`
   - `UpdateEntityEmailRequest`, `UpdateEntityPhoneRequest`
2. Per-entity DTOs (BusinessEmailDto, etc.) inherit — no changes needed.

### Request layer (parent create/update)
3. Reuse existing base DTOs directly in parent request lists:
   - `CreateEntityEmailRequest` / `CreateEntityPhoneRequest` in Create requests
   - `UpdateEntityEmailRequest` / `UpdateEntityPhoneRequest` (with `int? Id`) in Update requests
4. Replace `List<string>? Emails/Phones` with typed lists in all 8 parent requests.
5. Add `PhoneType` to `CreateEntityPhoneRequest`.

### Service layer
5. Replace raw-string entity creation with typed-item processing in all 4 services.
6. Business rules — enforced in each Create/Update method:
   - If emails/phones exist, exactly one per type must be `IsMain = true`
   - If no IsMain specified, first item auto-becomes IsMain
   - Toggling a new IsMain unsets all others of that type
   - If only one email/phone exists, it is always IsMain

### Persist & retrieve
7. Update public DTOs (`EntityEmailDto`, `EntityPhoneDto`) to expose IsMain — read
   clients see the flag.

## Affected Areas

| Area | Impact | Description |
|------|--------|-------------|
| `src/sicoain.shared/DTOs/EntityEmailDto.cs` | Modified | Add `IsMain` |
| `src/sicoain.shared/DTOs/EntityPhoneDto.cs` | Modified | Add `IsMain` |
| `src/sicoain.shared/DTOs/CreateEntityEmailRequest.cs` | Modified | Add `IsMain` |
| `src/sicoain.shared/DTOs/CreateEntityPhoneRequest.cs` | Modified | Add `IsMain` |
| `src/sicoain.shared/DTOs/UpdateEntityEmailRequest.cs` | Modified | Add `IsMain` |
| `src/sicoain.shared/DTOs/UpdateEntityPhoneRequest.cs` | Modified | Add `IsMain` |
| `src/sicoain.shared/DTOs/Business/CreateBusinessRequest.cs` | Modified | Typed items instead of strings |
| `src/sicoain.shared/DTOs/Business/UpdateBusinessRequest.cs` | Modified | Typed items instead of strings |
| `src/sicoain.shared/DTOs/Branches/CreateBranchRequest.cs` | Modified | Typed items instead of strings |
| `src/sicoain.shared/DTOs/Branches/UpdateBranchRequest.cs` | Modified | Typed items instead of strings |
| `src/sicoain.shared/DTOs/OccupationalRiskAdministrators/Create*.cs` | Modified | Typed items instead of strings |
| `src/sicoain.shared/DTOs/OccupationalRiskAdministrators/Update*.cs` | Modified | Typed items instead of strings |
| `src/sicoain.shared/DTOs/HealthPromotionEntities/Create*.cs` | Modified | Typed items instead of strings |
| `src/sicoain.shared/DTOs/HealthPromotionEntities/Update*.cs` | Modified | Typed items instead of strings |
| `src/sicoain.api/Services/BusinessService.cs` | Modified | IsMain business logic |
| `src/sicoain.api/Services/BranchService.cs` | Modified | IsMain business logic |
| `src/sicoain.api/Services/ORAService.cs` | Modified | IsMain business logic |
| `src/sicoain.api/Services/HPEService.cs` | Modified | IsMain business logic |

## Risks

| Risk | Likelihood | Mitigation |
|------|------------|------------|
| Breaking parent request API | High | Backward-compat not possible — `List<string>` → typed. All callers must update |
| Existing data has no IsMain | Medium | Migration: first email/phone per parent auto-assigned as IsMain on read |
| Missing rule enforcement | Medium | Add validation before service Create/Update, unit tests for edge cases |

## Rollback Plan

Revert to the last commit before the change. No data migration needed — IsMain
exists in the schema but was never populated. Rollback leaves IsMain as dead
code again (current state).

## Dependencies

None — all changes are within the existing solution.

## Success Criteria

- [ ] All 6 base DTOs expose `IsMain` and map correctly to entities via AutoMapper
- [ ] Parent requests accept typed items; services create entities with IsMain set
- [ ] Business rules enforced: exactly one IsMain per type, auto-assign first if missing
- [ ] Existing unit tests (if any) pass; new tests cover rule edge cases
- [ ] `dotnet build` succeeds with no warnings
