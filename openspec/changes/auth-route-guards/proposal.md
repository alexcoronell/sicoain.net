# Proposal: Auth Route Guards

## Intent

All Blazor WASM routes are currently unprotected — any user can navigate to any page without authentication. Authenticated users landing on `/` see the login form; unauthenticated users can reach Dashboard, Empresas, and all other protected pages (though the menu is hidden). This change enforces proper route-level authentication guards.

## Scope

### In Scope
- Router-level auth enforcement via `CascadingAuthenticationState` + `AuthorizeRouteView`
- `[Authorize]` attribute on all 16 protected pages (Dashboard, Businesses, Branches, Roles, Users, Employees, EPS, ARL + create/edit variants)
- Authenticated redirect: `/` → `/dashboard`
- Unauthenticated redirect: any protected route → `/`
- New `RedirectToLogin` component for unauthorized fallback
- Evaluation of `forceLoad: true` removal on login → dashboard flow

### Out of Scope
- Role or permission-based authorization (deferred)
- Changes to `CustomAuthenticationStateProvider`, `AuthService`, or token refresh logic
- Auth state caching or performance optimization

## Capabilities

### New Capabilities
- `auth-route-guards`: Blazor WASM route-level authentication enforcement with login/home auto-redirect

### Modified Capabilities
- None (no existing specs)

## Approach

1. **App.razor**: Wrap `<Router>` in `<CascadingAuthenticationState>`; replace `<RouteView>` with `<AuthorizeRouteView>` + `<NotAuthorized>` pointing to `<RedirectToLogin />`
2. **RedirectToLogin.razor** (new — `Shared/`): calls `Navigation.NavigateTo("/")` via `OnInitializedAsync` when `AuthorizeRouteView` detects unauthorized state
3. **protected pages** (16 files): add `@attribute [Authorize]` at top, after `@page`
4. **Home.razor**: inject `AuthenticationStateProvider` in `OnInitializedAsync`; if authenticated, redirect to `/dashboard` (soft navigation)
5. **LoginComponent.razor**: evaluate switching `forceLoad: true` to `false` — `AuthenticationStateProvider` is already notified before navigation, so the auth state is available immediately

## Affected Areas

| Area | Impact | Description |
|------|--------|-------------|
| `App.razor` | Modified | Wrap Router in `CascadingAuthenticationState`, use `AuthorizeRouteView` |
| `Shared/RedirectToLogin.razor` | **New** | Redirects unauthorized users to `/` |
| `Pages/Home.razor` | Modified | Auth check on load → redirect to `/dashboard` |
| `Pages/Dashboard.razor` | Modified | Add `@attribute [Authorize]` |
| `Pages/Businesses.razor` | Modified | Add `@attribute [Authorize]` |
| `Pages/BusinessesCreate.razor` | Modified | Add `@attribute [Authorize]` |
| `Pages/BusinessesEdit.razor` | Modified | Add `@attribute [Authorize]` |
| `Pages/Branches.razor` | Modified | Add `@attribute [Authorize]` |
| `Pages/BranchesCreate.razor` | Modified | Add `@attribute [Authorize]` |
| `Pages/BranchesEdit.razor` | Modified | Add `@attribute [Authorize]` |
| `Pages/Roles.razor` | Modified | Add `@attribute [Authorize]` |
| `Pages/RolesCreate.razor` | Modified | Add `@attribute [Authorize]` |
| `Pages/RolesEdit.razor` | Modified | Add `@attribute [Authorize]` |
| `Pages/Users.razor` | Modified | Add `@attribute [Authorize]` |
| `Pages/UsersCreate.razor` | Modified | Add `@attribute [Authorize]` |
| `Pages/UsersEdit.razor` | Modified | Add `@attribute [Authorize]` |
| `Pages/Employees.razor` | Modified | Add `@attribute [Authorize]` |
| `Pages/HealthPromotionEntities.razor` | Modified | Add `@attribute [Authorize]` |
| `Pages/OccupationalRiskAdministrators.razor` | Modified | Add `@attribute [Authorize]` |
| `Components/LoginComponent.razor` | Modified | Evaluate `forceLoad` removal |

## Risks

| Risk | Likelihood | Mitigation |
|------|------------|------------|
| Redirect loop on session expiry | Low | `SessionExpiredNotifier` already redirects to `/`; `RedirectToLogin` respects real auth state from provider |
| Auth state race on cold/hard load | Low | `CustomAuthenticationStateProvider` calls `GET /auth/me` before declaring state — no local-only assumption |
| `NotFound.razor` with `@layout MainLayout` shows login UI for unauthenticated users | Low | The Router's `NotFound` parameter bypasses `AuthorizeRouteView`; `NotFound.razor` stays public |

## Rollback Plan

Revert `App.razor` to plain `<RouteView>`, remove `@attribute [Authorize]` from all pages, revert `Home.razor` auth check, delete `RedirectToLogin.razor`, revert `LoginComponent.razor` `forceLoad` change.

## Dependencies

None — `AddAuthorizationCore()` and `CustomAuthenticationStateProvider` are already registered.

## Success Criteria

- [ ] Unauthenticated user accessing `/dashboard` → redirected to `/` (login form)
- [ ] Authenticated user accessing `/` → redirected to `/dashboard`
- [ ] Authenticated user accessing any protected page (e.g., `/empresas`, `/roles`) → sees the page content
- [ ] Unauthenticated user accessing any protected page → redirected to `/`
- [ ] `LoginComponent` navigates to `/dashboard` without hard reload (`forceLoad: false`)
- [ ] Session expiry → user lands on `/` and sees login form (no redirect loop)
