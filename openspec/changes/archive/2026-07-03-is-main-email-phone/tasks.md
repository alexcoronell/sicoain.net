# Tasks: Enable IsMain field on Email/Phone entities

## Review Workload Forecast

| Field | Value |
|-------|-------|
| Estimated changed lines | 300–450 |
| 400-line budget risk | High |
| Chained PRs recommended | Yes |
| Suggested split | PR 1: DTOs + service logic (backend) → PR 2: Blazor forms + tests |
| Delivery strategy | ask-on-risk |
| Chain strategy | pending |

Decision needed before apply: No (size:exception approved)
Chained PRs recommended: Yes
Chain strategy: size-exception
400-line budget risk: High

## Phase 1: DTO Layer — Foundation types

- [x] 1.1 Create `IHasIsMain` interface with `bool IsMain { get; set; }` in `src/sicoain.api/Services/`
- [x] 1.2 Add `bool IsMain` to `EntityEmailDto` and `EntityPhoneDto`
- [x] 1.3 Add `bool IsMain` to `CreateEntityEmailRequest` and `CreateEntityPhoneRequest`; add `PhoneType` to phone request
- [x] 1.4 Add `bool IsMain` and `int? Id` to `UpdateEntityEmailRequest` and `UpdateEntityPhoneRequest`
- [x] 1.5 Swap `List<string>? Emails/Phones` to typed lists in all 8 parent create/update request DTOs (Business, Branch, ORA, HPE)

## Phase 2: Service Layer — Business logic

- [x] 2.1 Create `IsMainHelper.EnsureSingleMain<T>()` static method constrained by `IHasIsMain`
- [x] 2.2 Remove `PhoneType.Ignore()` from all 4 AutoMapper profiles
- [x] 2.3 Update `BusinessService` Create/Update to process typed items, Id-based sync, and call `EnsureSingleMain`
- [x] 2.4 Update `BranchService` Create/Update with same typed-item + IsMain logic
- [x] 2.5 Update `ORAService` Create/Update with same typed-item + IsMain logic
- [x] 2.6 Update `HPEService` Create/Update with same typed-item + IsMain logic

## Phase 3: UI Layer — Blazor forms

- [x] 3.1 Update `BusinessForm.razor` — show IsMain radio/badge per email/phone row, enforce one per type
- [x] 3.2 Update `BranchForm.razor` — same IsMain selector pattern
- [x] 3.3 Update `ORAForm.razor` — same IsMain selector pattern
- [x] 3.4 Update `HPEForm.razor` — same IsMain selector pattern

## Phase 4: Testing

- [x] 4.1 Unit tests for `EnsureSingleMain` — 0 items, 1 item, none marked, multiple marked, single-override
- [x] 4.2 Unit tests for service Create — IsMain set correctly, PhoneType passed through, auto-assign on missing
- [x] 4.3 Unit tests for service Update — Id-based sync: match by Id, add new, remove omitted
- [x] 4.4 Integration test — full create→read roundtrip verifying IsMain in response DTO
