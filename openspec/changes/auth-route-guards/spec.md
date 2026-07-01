# Spec: Auth Route Guards — Blazor WASM Route-Level Authentication

**Change**: `auth-route-guards`
**Propósito**: Implementar guardias de autenticación a nivel de ruta en el cliente Blazor WASM, con redirección automática de usuarios autenticados desde `/` hacia `/dashboard` y de usuarios no autenticados desde rutas protegidas hacia `/`.

---

## 1. Requerimientos funcionales

| ID | Descripción | Prioridad |
|----|-------------|-----------|
| RF1 | Un usuario autenticado que navega a `/` DEBE ser redirigido automáticamente a `/dashboard` | Alta |
| RF2 | Un usuario no autenticado que navega a una ruta protegida DEBE ser redirigido automáticamente a `/` | Alta |
| RF3 | Un usuario no autenticado que navega a `/` DEBE ver la página de login normalmente | Alta |
| RF4 | Un usuario autenticado que navega a una ruta protegida DEBE ver el contenido de la página sin interferencias | Alta |
| RF5 | Cuando la sesión expira mientras el usuario está en una ruta protegida, DEBE ser redirigido a `/` | Alta |
| RF6 | Tras un login exitoso, el usuario DEBE ser redirigido a `/dashboard` | Alta |

---

## 2. Escenarios (Given-When-Then)

| ID | Descripción | Given | When | Then |
|----|-------------|-------|------|------|
| E01 | Auth true → `/` redirige a dashboard | El usuario está autenticado y en la ruta `/` | El componente `Home.razor` se inicializa (`OnInitializedAsync`) | El sistema redirige a `/dashboard` mediante navegación suave (`forceLoad: false`) |
| E02 | Auth false → ruta protegida redirige a `/` | El usuario NO está autenticado e intenta acceder a `/dashboard` | El Router resuelve la ruta y `AuthorizeRouteView` detecta que el usuario no está autorizado | El sistema redirige a `/` mediante `RedirectToLogin` sin mostrar el contenido de la ruta protegida |
| E03 | Auth false → `/` muestra login | El usuario NO está autenticado y navega a `/` | El Router resuelve la ruta y `Home.razor` se inicializa | El sistema muestra la página de login (`LoginComponent`) sin redirección |
| E04 | Auth true → ruta protegida muestra contenido | El usuario está autenticado y navega a `/empresas` | El Router resuelve la ruta y `AuthorizeRouteView` verifica que el usuario está autorizado | El sistema muestra el contenido completo de la página sin redirección |
| E05 | Sesión expira en ruta protegida | El usuario está autenticado en `/roles` y su sesión expira (refresh token inválido) | `SessionExpiredNotifier` dispara el evento `SessionExpired` | `MainLayout` redirige a `/` y el usuario ve la página de login (sin loop de redirección) |
| E06 | Login exitoso redirige a dashboard | El usuario no autenticado completa el formulario de login con credenciales válidas | El método `HandleLoginAsync` retorna `result.Success = true` | El sistema redirige a `/dashboard` (se evalúa `forceLoad: false` vs `true`) |

---

## 3. Reglas de negocio

| ID | Regla |
|----|-------|
| RN1 | La autorización es binaria (autenticado vs no autenticado). NO se requiere autorización por roles en este cambio. |
| RN2 | La redirección DEBE ser inmediata: el usuario NO DEBE ver un flash del contenido de la página no autorizada ni de la página de login si será redirigido. |
| RN3 | NO DEBE existir un loop de redirección. Específicamente: autenticado en `/` → `/dashboard` NO debe volver a `/`. El check en `Home.razor` DEBE ejecutarse UNA SOLA vez por navegación. |
| RN4 | La página `NotFound` (404) DEBE permanecer pública y accesible sin autenticación, ya que el Router la resuelve fuera de `AuthorizeRouteView`. |
| RN5 | `forceLoad: true` en el login y logout DEBE evaluarse. Como `CustomAuthenticationStateProvider.MarkUserAsAuthenticated` se invoca ANTES de la navegación, el estado de auth está disponible inmediatamente para `AuthorizeRouteView`, por lo que `forceLoad: false` (navegación suave) ES SUFICIENTE. |
| RN6 | Sin embargo, en el logout (sesión expirada o cierre manual), `forceLoad: true` PUEDE ser necesario para forzar una recarga limpia del proveedor de autenticación y evitar estados fantasma. Este es un tradeoff conocido: login puede ser suave, logout debe ser duro. |

---

## 4. Consideraciones técnicas

### 4.1 Modificaciones en `App.razor`

- Envolver `<Router>` en `<CascadingAuthenticationState>`.
- Reemplazar `<RouteView>` con `<AuthorizeRouteView>`.
- Agregar `<NotAuthorized>` que renderice un componente `<RedirectToLogin />`.
- El parámetro `NotFoundPage` del Router NO DEBE ser envuelto por `AuthorizeRouteView` (se mantiene público).

### 4.2 Nuevo componente `RedirectToLogin.razor` (en `Shared/` o `Components/`)

- Hereda de `ComponentBase`.
- En `OnInitializedAsync` llama a `Navigation.NavigateTo("/", forceLoad: false)`.
- No renderiza ningún marcado visible (es puramente lógico).
- `forceLoad: false` permite navegación suave preservando el estado de auth del proveedor.

### 4.3 Modificaciones en `Home.razor`

- Inyectar `AuthenticationStateProvider` y `NavigationManager`.
- En `OnInitializedAsync`: obtener estado de autenticación. Si el usuario está autenticado, redirigir a `/dashboard` con `forceLoad: false`.
- El resto del contenido (login) solo se renderiza si NO está autenticado.

### 4.4 Páginas protegidas (16 archivos)

- Agregar `@attribute [Authorize]` en cada página protegida, inmediatamente después de la directiva `@page`.
- Páginas a modificar:
  - Dashboard, Businesses, BusinessesCreate, BusinessesEdit
  - Branches, BranchesCreate, BranchesEdit
  - Roles, RolesCreate, RolesEdit
  - Users, UsersCreate, UsersEdit
  - Employees, HealthPromotionEntities, OccupationalRiskAdministrators

### 4.5 Evaluación de `forceLoad` en `LoginComponent.razor`

- Actualmente: `Navigation.NavigateTo("/dashboard", true)`.
- Propuesto: cambiar a `forceLoad: false`.
- **Fundamento**: `MarkUserAsAuthenticated` (línea 73-74 en `LoginComponent.razor`) notifica el cambio de estado ANTES de la navegación. Cuando `AuthorizeRouteView` evalúa la ruta `/dashboard`, el proveedor ya tiene el estado actualizado. No necesita recarga dura.
- **Tradeoff**: si existiera algún middleware o handler que dependa del ciclo de vida completo de WASM (re-inicialización de DI), `forceLoad: false` podría omitir ese paso. Hasta ahora no hay tal dependencia.
- **Logout**: `StatusSessionComponent.HandleLogoutAsync` usa `forceLoad: true`. Esto es correcto porque `MarkUserAsLoggedOut` + recarga dura garantiza que no queden residuos de estado.

### 4.6 Flujo de sesión expirada

- `SessionExpiredNotifier.Notify()` → `MainLayout.OnSessionExpired()` → `Navigation.NavigateTo("/", forceLoad: false)`.
- Al llegar a `/`, `Home.razor` verifica auth state → el usuario NO está autenticado → muestra login. Sin loop porque el usuario autenticado es el que redirige, y el check es una sola vez.

### 4.7 Orden de ejecución para evitar flash

1. Usuario no autenticado escribe `/dashboard` en URL.
2. Blazor Router resuelve `Dashboard.razor`.
3. `AuthorizeRouteView` detecta `[Authorize]` y que el usuario no está autenticado.
4. Renderiza `<RedirectToLogin />`.
5. `RedirectToLogin.OnInitializedAsync` navega a `/`.
6. `Home.razor.OnInitializedAsync` verifica auth → no autenticado → muestra login.
7. Usuario ve login. Sin flash de Dashboard.

---

## 5. Criterios de aceptación

- [ ] RF1: Autenticado en `/` → redirige a `/dashboard` (navegación suave)
- [ ] RF2: No autenticado en `/dashboard` → redirige a `/` (sin flash de Dashboard)
- [ ] RF3: No autenticado en `/` → ve el formulario de login
- [ ] RF4: Autenticado en cualquier ruta protegida → ve el contenido de la página
- [ ] RF5: Sesión expirada → usuario redirigido a `/` y ve login (sin loop)
- [ ] RF6: Login exitoso → redirige a `/dashboard` (con `forceLoad` evaluado)
- [ ] No hay loop de redirección en ningún escenario
- [ ] 16 páginas protegidas tienen `@attribute [Authorize]`
- [ ] `App.razor` usa `CascadingAuthenticationState` + `AuthorizeRouteView` + `NotAuthorized`
- [ ] `RedirectToLogin.razor` existe y funciona
- [ ] `NotFound.razor` sigue siendo accesible sin autenticación
- [ ] Logout mantiene `forceLoad: true`
