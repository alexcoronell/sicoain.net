# Design: Enable IsMain field on Email/Phone entities

## Technical Approach

Layer-by-layer enhancement across 6 base DTOs, 8 parent requests, 4 API services, and 4 Blazor forms. `IsMain` exists on `BaseEntityEmail`/`BaseEntityPhone` but is dead code — this change wires it through the full CRUD stack by replacing `List<string>` with typed request DTOs and adding business rule enforcement at the service layer.

## Architecture Decisions

### Decision: Typed DTOs over strings in parent requests

| Option | Tradeoff | Decision |
|--------|----------|----------|
| Keep `List<string>` + separate `IsMain` index | Fragile — index breaks when items reorder | ❌ Rejected |
| Reuse base DTOs directly in parent request lists | Automatic inheritance of `IsMain`, `PhoneType`, `Id`; no per-entity DTO duplication | ✅ **Chosen** |
| New per-entity wrapper requests | Duplicate boilerplate across 4 entities × 2 types | ❌ Rejected |

Parent requests use `List<CreateEntityEmailRequest>?` / `List<UpdateEntityEmailRequest>?` etc. directly — same pattern for phones.

### Decision: Id-based diffing for update sync

**Choice**: Use `int? Id` on `UpdateEntityEmailRequest`/`UpdateEntityPhoneRequest` to match existing items during sync.
**Alternatives**: Continue value-based string diffing (breaks when same value changes IsMain).
**Rationale**: The current HashSet-based diffing can't represent "update IsMain on existing email without changing the address." Adding `Id` enables precise update semantics and matches the per-item endpoint pattern.

### Decision: Shared `EnsureSingleMain<T>` helper

**Choice**: Static helper method in `sicoain.api.Services` namespace, constrained via `where T : IHasIsMain`. Applies rules AFTER mapping request DTOs to entities — entities have `{ get; set; }` properties, request DTOs remain immutable `record` types.
**Alternatives**: Duplicate logic in each service, apply rules via expression on request DTOs.
**Rationale**: IsMain rule is identical across emails and phones — a single generic method enforces consistency on the mutable entity instances. Request DTOs stay immutable. `IHasIsMain` is implemented on `BaseEntityEmail`/`BaseEntityPhone`, inherited by all concrete entities.

### Decision: Inherited IsMain in AutoMapper

**Choice**: No explicit mapping changes needed for `IsMain` — property name matches between DTOs and entities, AutoMapper convention maps it automatically.
**Exception**: `HealthPromotionEntityPhone` mapping explicitly ignores `PhoneType` — this `.ForMember(opt => opt.Ignore())` MUST be removed when `CreateEntityPhoneRequest.PhoneType` is added.

## Data Flow

```
Client Form (typed items with IsMain)
    │
    ▼
Parent Request DTOs (List<CreateEntityEmailRequest> etc.)
    │
    ▼
Service.Create/Update
    ├── _mapper.Map<Entity>(request) — scalar fields only
    ├── EnsureSingleMain(emails) / EnsureSingleMain(phones)
    ├── Create items from typed DTOs with IsMain, PhoneType, Id
    └── SaveChanges()
    │
    ▼
API Response → Client reads IsMain from EntityEmailDto / EntityPhoneDto
```

For update sync with Id matching:
```
request.Emails items grouped:
  ├── has Id → match by Id → update existing entity
  ├── no Id → create new entity
  └── existing items not in request → delete
```

## File Changes

| File | Action | Description |
|------|--------|-------------|
| `src/sicoain.shared/DTOs/EntityEmailDto.cs` | Modify | Add `bool IsMain` |
| `src/sicoain.shared/DTOs/EntityPhoneDto.cs` | Modify | Add `bool IsMain`, add `PhoneType` |
| `src/sicoain.shared/DTOs/CreateEntityEmailRequest.cs` | Modify | Add `bool IsMain` |
| `src/sicoain.shared/DTOs/CreateEntityPhoneRequest.cs` | Modify | Add `bool IsMain`, add `PhoneType` |
| `src/sicoain.shared/DTOs/UpdateEntityEmailRequest.cs` | Modify | Add `bool IsMain`, add `int? Id` |
| `src/sicoain.shared/DTOs/UpdateEntityPhoneRequest.cs` | Modify | Add `bool IsMain`, add `int? Id` |
| `src/sicoain.shared/DTOs/Business/CreateBusinessRequest.cs` | Modify | `List<CreateEntityEmailRequest>? Emails`, `List<CreateEntityPhoneRequest>? Phones` |
| `src/sicoain.shared/DTOs/Business/UpdateBusinessRequest.cs` | Modify | `List<UpdateEntityEmailRequest>? Emails`, `List<UpdateEntityPhoneRequest>? Phones` |
| `src/sicoain.shared/DTOs/Branches/CreateBranchRequest.cs` | Modify | Same typed swap |
| `src/sicoain.shared/DTOs/Branches/UpdateBranchRequest.cs` | Modify | Same typed swap |
| `src/sicoain.shared/DTOs/OccupationalRiskAdministrators/Create*.cs` | Modify | Same typed swap |
| `src/sicoain.shared/DTOs/OccupationalRiskAdministrators/Update*.cs` | Modify | Same typed swap |
| `src/sicoain.shared/DTOs/HealthPromotionEntities/Create*.cs` | Modify | Same typed swap |
| `src/sicoain.shared/DTOs/HealthPromotionEntities/Update*.cs` | Modify | Same typed swap |
| `src/sicoain.api/Services/BusinessService.cs` | Modify | Typed-item creation, Id-based sync, call EnsureSingleMain |
| `src/sicoain.api/Services/BranchService.cs` | Modify | Same |
| `src/sicoain.api/Services/OccupationalRiskAdministratorService.cs` | Modify | Same |
| `src/sicoain.api/Services/HealthPromotionEntityService.cs` | Modify | Same |
| `src/sicoain.api/Mappings/*Profile.cs` | Modify | Remove `PhoneType.Ignore()` from phone entity mappings |
| `src/sicoain.client/Components/BusinessForm.razor` | Modify | Coupled IsMain selector per email/phone row |
| `src/sicoain.client/Components/BranchForm.razor` | Modify | Same |
| `src/sicoain.client/Components/ORAForm.razor` | Modify | Same |
| `src/sicoain.client/Components/HPEForm.razor` | Modify | Same |

**New file**: `src/sicoain.api/Services/IsMainHelper.cs` — static `EnsureSingleMain<T>` generic method.

## Interfaces / Contracts

```csharp
// Shared interface for IsMain constraint helper
namespace sicoain.api.Services;

public interface IHasIsMain
{
    bool IsMain { get; set; }
}

public static class IsMainHelper
{
    /// Ensures exactly one item is marked IsMain per type.
    /// Rules (from spec):
    ///   - If none marked → first item becomes IsMain
    ///   - If multiple marked → first wins, others forced false
    ///   - If only one item → always IsMain
    public static void EnsureSingleMain<T>(List<T> items) where T : IHasIsMain
    {
        if (items.Count == 0) return;
        if (items.Count == 1) { items[0].IsMain = true; return; }

        var mains = items.Where(i => i.IsMain).ToList();
        if (mains.Count == 0)
            items[0].IsMain = true;
        else if (mains.Count > 1)
            foreach (var item in items.SkipWhile(i => !i.IsMain).Skip(1))
                item.IsMain = false;
    }
}
```

## Testing Strategy

| Layer | What to Test | Approach |
|-------|-------------|----------|
| Unit | `EnsureSingleMain` edge cases | xUnit — 0 items, 1 item, none marked, multiple marked, single-override |
| Unit | Service Create with typed items | Verify IsMain set, PhoneType passed through |
| Unit | Service Update Id-based sync | Match by Id, add new, remove omitted |
| Integration | Full create→read roundtrip | HTTP call → verify IsMain in response DTO |

## Migration / Rollout

No data migration required. `IsMain` already exists in the schema with default `false`. Existing data will have no `IsMain` set — the first email/phone per entity will auto-assign as main on next update.

## Open Questions

- [ ] The `IHasIsMain` interface approach constrains the generic — confirm alternative using expression-based property access if runtime overhead concerns exist.
