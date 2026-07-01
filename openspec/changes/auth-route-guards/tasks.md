# Tasks: Auth Route Guards — Route-Level Authentication for Blazor WASM

## Review Workload Forecast

| Field | Value |
|-------|-------|
| Estimated changed lines | ~55 |
| 400-line budget risk | Low |
| Chained PRs recommended | No |
| Delivery strategy | single-pr |
| Chain strategy | pending |

Decision needed before apply: No
Chained PRs recommended: No
Chain strategy: pending
400-line budget risk: Low

### Dependency Graph

```
1.1 (_Imports.razor) ──→ 1.3 (App.razor)
1.2 (RedirectToLogin) ──→ 1.3 (App.razor)
                          │
1.3 (App.razor) ──────────┼──→ 3.1 (7 module pages)
                          └──→ 3.2 (9 admin pages)
                          
2.1 (Home.razor) ─────────→ 4.1 (LoginComponent.razor)

1.3 + 2.1 + 3.1/3.2 + 4.1 → 5.1 (Verification)
```

## Phase 1: Router Infrastructure

- [ ] 1.1 Add `@using Microsoft.AspNetCore.Components.Authorization` to `src/sicoain.client/_Imports.razor` — enables `CascadingAuthenticationState` and `AuthorizeRouteView` globally
- [ ] 1.2 Create `src/sicoain.client/Components/RedirectToLogin.razor` — logical component that navigates to `/` with `forceLoad: false` in `OnInitialized`
- [ ] 1.3 Modify `src/sicoain.client/App.razor` — wrap `<Router>` in `<CascadingAuthenticationState>`; replace `<RouteView>` with `<AuthorizeRouteView>` + `<NotAuthorized>` pointing to `<RedirectToLogin />`; keep `<FocusOnNavigate>` and `<NotFound>` untouched

## Phase 2: Home Auth Redirect

- [ ] 2.1 Modify `src/sicoain.client/Pages/Home.razor` — inject `AuthenticationStateProvider` and `NavigationManager`; in `OnInitializedAsync` check auth state and redirect to `/dashboard` with `forceLoad: false` if authenticated; login content renders only for unauthenticated users

## Phase 3: Protected Pages

- [ ] 3.1 Add `@attribute [Authorize]` to 7 module pages: `Dashboard.razor`, `Businesses.razor`, `BusinessesCreate.razor`, `BusinessesEdit.razor`, `Branches.razor`, `BranchesCreate.razor`, `BranchesEdit.razor`
- [ ] 3.2 Add `@attribute [Authorize]` to 9 admin pages: `Roles.razor`, `RolesCreate.razor`, `RolesEdit.razor`, `Users.razor`, `UsersCreate.razor`, `UsersEdit.razor`, `Employees.razor`, `HealthPromotionEntities.razor`, `OccupationalRiskAdministrators.razor`

## Phase 4: Login Navigation Optimisation

- [ ] 4.1 Modify `src/sicoain.client/Components/LoginComponent.razor` — change `Navigation.NavigateTo("/dashboard", true)` to `false` on line 75; keep logout `forceLoad: true` unchanged

## Phase 5: Verification

- [ ] 5.1 Walk-through all E01–E06 scenarios: auth at `/` → dashboard; unauth at protected route → `/`; unauth at `/` → login; auth at protected route → content; login → dashboard; session expiry → `/` with login (no loop)

### Implementation Order

```
1.1 → 1.2  (parallel)
  ↓
1.3 ──────→ 3.1, 3.2 (parallel)
  │
2.1 → 4.1
  │
  ↓
5.1 (full integration walk-through)
```

Phase 1 first (router infra), then Phase 2 (home redirect) can run parallel. Phase 3 (page auth) depends on 1.3. Phase 4 depends on 2.1. Phase 5 is final verification.
