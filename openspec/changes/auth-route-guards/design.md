# Design: Auth Route Guards — Blazor WASM Route-Level Authentication

## 1. Resumen de la arquitectura

```
Usuario → URL → Blazor Router
                   │
        ┌──────────┴──────────┐
        │  <CascadingAuthState>│
        └──────────┬──────────┘
                   │
        ┌──────────┴──────────┐
        ▼                     ▼
   <Found>              <NotFound>
        │              NotFound.razor (público)
        │
   ┌────┴────┐
   ▼         ▼
  [Authorize]  Sin [Authorize]
     │              │
     ▼              ▼
  Authorize     RouteView
  RouteView        │
     │           Home.razor
     │           (login)
  ┌──┴──┐
  ▼     ▼
 Auth  No Auth
  │      │
  │      ▼
  │   NotAuthorized → RedirectToLogin → /
  ▼
 Página protegida
 (Dashboard, Empresas, etc.)
```

### Proveedor de autenticación

```
CustomAuthenticationStateProvider
  └─ GetAuthenticationStateAsync()
       └─ GET /auth/me → UserDto | 401 → TryRefresh → GET /auth/me | fail → anonymous
  └─ MarkUserAsAuthenticated(user) → NotifyAuthenticationStateChanged()
  └─ MarkUserAsLoggedOut() → NotifyAuthenticationStateChanged()
```

El `CascadingAuthenticationState` suscribe automáticamente a `NotifyAuthenticationStateChanged` y propaga el estado a todos los descendientes.

---

## 2. Cambios por archivo

### 2.1 `src/sicoain.client/App.razor` — Modificar

**Qué hacer**: Envolver el `<Router>` en `<CascadingAuthenticationState>`. Reemplazar `<RouteView>` por `<AuthorizeRouteView>`. Agregar `<NotAuthorized>` con `<RedirectToLogin />`.

```razor
<CascadingAuthenticationState>
    <Router AppAssembly="@typeof(App).Assembly" NotFoundPage="typeof(Pages.NotFound)">
        <Found Context="routeData">
            <AuthorizeRouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)">
                <NotAuthorized>
                    <RedirectToLogin />
                </NotAuthorized>
            </AuthorizeRouteView>
            <FocusOnNavigate RouteData="@routeData" Selector="h1" />
        </Found>
    </Router>
</CascadingAuthenticationState>
```

**Consideraciones**:
- `NotFoundPage` queda FUERA de `AuthorizeRouteView` — es resuelto por el Router directamente, sigue siendo público.
- El `FocusOnNavigate` se mantiene dentro del `<Found>` como antes.
- El `CascadingAuthenticationState` DEBE envolver al Router completo, no solo al `<Found>`, para que esté disponible en toda la aplicación (incluyendo layouts).

### 2.2 `src/sicoain.client/Components/RedirectToLogin.razor` — Crear

**Qué hacer**: Componente lógico que redirige a `/` cuando `AuthorizeRouteView` detecta un estado no autorizado.

```razor
@inject NavigationManager Navigation

@code {
    protected override void OnInitialized()
    {
        Navigation.NavigateTo("/", forceLoad: false);
    }
}
```

**Consideraciones**:
- No renderiza HTML — es puramente un componente de navegación.
- `forceLoad: false` porque el estado de auth ya está disponible en el proveedor.
- Sin `OnInitializedAsync` — no hay necesidad de async, la navegación es síncrona.
- Ubicación: `Components/` siguiendo la convención del proyecto (todos los componentes compartidos están ahí).

### 2.3 `src/sicoain.client/Pages/Home.razor` — Modificar

**Qué hacer**: Inyectar `AuthenticationStateProvider` y `NavigationManager`. En `OnInitializedAsync`, verificar si el usuario está autenticado y redirigir a `/dashboard`.

```razor
@page "/"
@inject AuthenticationStateProvider AuthProvider
@inject NavigationManager Navigation

<PageTitle>SICOAIN | Sistema de Control de Accidentes e Incidentes | Ingresar</PageTitle>
<MudCard
    Outlined="true"
    Class="w-100 mx-auto mt-12"
    Style="max-width: 500px;">
    <MudCardContent>
        <LoginComponent />
    </MudCardContent>
</MudCard>

@code {
    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthProvider.GetAuthenticationStateAsync();
        if (authState.User.Identity?.IsAuthenticated == true)
        {
            Navigation.NavigateTo("/dashboard", forceLoad: false);
        }
    }
}
```

**Consideraciones**:
- `forceLoad: false` — el estado de auth ya está propagado, no necesita recarga dura.
- El check se ejecuta UNA vez por inicialización del componente. No hay loop porque la redirección va a `/dashboard`, no de vuelta a `/`.
- `OnInitializedAsync` se usa porque `GetAuthenticationStateAsync()` es async (hace llamada HTTP a `/auth/me`).
- El contenido del login solo se renderiza si el usuario NO está autenticado.
- Si el usuario está autenticado y navega a `/`, la redirección ocurre ANTES de que el componente termine de renderizar, por lo que no hay flash del login.

### 2.4 Páginas protegidas (16 archivos) — Modificar

**Qué hacer**: Agregar `@attribute [Authorize]` inmediatamente después de la directiva `@page` en cada una.

```razor
@page "/dashboard"
@attribute [Authorize]
<PageTitle>...</PageTitle>
...
```

**Consideraciones**:
- La directiva `@attribute [Authorize]` debe ir DESPUÉS de `@page` y ANTES de cualquier otra directiva.
- No se requiere `@using Microsoft.AspNetCore.Authorization` explícito porque `_Imports.razor` no lo tiene — pero `@attribute [Authorize]` es reconocido automáticamente por el compilador de Blazor cuando `AddAuthorizationCore()` está registrado en DI (ya está en `Program.cs`).
- Verificar que `_Imports.razor` no necesite `@using Microsoft.AspNetCore.Authorization` — en Blazor WASM con .NET 10, `[Authorize]` se resuelve sin import explícito porque el compilador lo conoce.

### 2.5 `src/sicoain.client/Components/LoginComponent.razor` — Modificar

**Qué hacer**: Cambiar `forceLoad: true` a `false` en la navegación post-login.

```razor
// Línea 75 actual:
Navigation.NavigateTo("/dashboard", true);
// Cambiar a:
Navigation.NavigateTo("/dashboard", false);
```

**Consideraciones**:
- `MarkUserAsAuthenticated` (líneas 63-73) se ejecuta ANTES de la navegación, invocando `NotifyAuthenticationStateChanged`. Cuando `AuthorizeRouteView` evalúa `/dashboard`, el `CascadingAuthenticationState` ya tiene el estado actualizado.
- `forceLoad: false` evita la recarga completa de WASM, lo que es más rápido y evita el flash de recarga.
- **Logout NO se modifica**: `StatusSessionComponent.HandleLogoutAsync` mantiene `forceLoad: true` porque necesita limpiar estado fantasma con recarga dura.

### 2.6 `src/sicoain.client/Layout/MainLayout.razor` — Sin cambios

No requiere modificaciones. El `MainLayout` ya maneja:
- Suscripción a `SessionExpiredNotifier`
- Redirección a `/` con `forceLoad: false` en expiración de sesión
- Estado `_isAuthenticated` manejado vía `StatusSessionComponent`
- El layout es envuelto automáticamente por `CascadingAuthenticationState` gracias a `AuthorizeRouteView.DefaultLayout`

---

## 3. Flujo de navegación

### Escenario A: Usuario no auth → `/empresas`

```
1. Usuario escribe /empresas
2. Router resuelve → Businesses.razor (tiene @attribute [Authorize])
3. AuthorizeRouteView verifica auth → NO autenticado
4. Renderiza <NotAuthorized> → <RedirectToLogin />
5. RedirectToLogin.OnInitialized → Navigation.NavigateTo("/", forceLoad: false)
6. Router resuelve → Home.razor
7. Home.razor.OnInitializedAsync → AuthProvider.GetAuthenticationStateAsync()
8. AuthState.IsAuthenticated = false → muestra LoginComponent
9. Usuario ve formulario de login. Sin flash de /empresas.
```

### Escenario B: Usuario auth → `/`

```
1. Usuario autenticado navega a /
2. Router resuelve → Home.razor (sin [Authorize])
3. RouteView renderiza Home.razor
4. Home.razor.OnInitializedAsync → AuthProvider.GetAuthenticationStateAsync()
5. AuthState.IsAuthenticated = true
6. Navigation.NavigateTo("/dashboard", forceLoad: false)
7. Router resuelve → Dashboard.razor (tiene @attribute [Authorize])
8. AuthorizeRouteView verifica auth → autenticado → muestra Dashboard
9. Usuario ve Dashboard. Sin flash del login.
```

### Escenario C: Usuario no auth → `/`

```
1. Usuario no autenticado navega a /
2. Router resuelve → Home.razor (sin [Authorize])
3. RouteView renderiza Home.razor
4. Home.razor.OnInitializedAsync → AuthProvider.GetAuthenticationStateAsync()
5. AuthState.IsAuthenticated = false → NO redirige
6. Usuario ve LoginComponent
```

### Escenario D: Usuario auth → `/empresas`

```
1. Usuario autenticado navega a /empresas
2. Router resuelve → Businesses.razor (tiene @attribute [Authorize])
3. AuthorizeRouteView verifica auth → autenticado
4. Renderiza Businesses.razor con MainLayout
5. Usuario ve el contenido de Empresas
```

### Escenario E: Sesión expira en `/empresas`

```
1. Usuario autenticado ve /empresas
2. Llamada API falla con 401 → AuthRefreshHandler intenta refresh
3. Refresh falla → SessionExpiredNotifier.Notify()
4. MainLayout.OnSessionExpired → Navigation.NavigateTo("/", forceLoad: false)
5. Router resuelve → Home.razor
6. Home.razor.OnInitializedAsync → AuthProvider.GetAuthenticationStateAsync()
7. AuthProvider → GET /auth/me → 401 → TryRefresh → falla → anonymous
8. AuthState.IsAuthenticated = false → muestra LoginComponent
9. Usuario ve login. Sin loop de redirección.
```

---

## 4. Páginas que requieren `@attribute [Authorize]`

| # | Archivo | Ruta `@page` |
|---|---------|--------------|
| 1 | `Pages/Dashboard.razor` | `/dashboard` |
| 2 | `Pages/Businesses.razor` | `/empresas` |
| 3 | `Pages/BusinessesCreate.razor` | `/empresas/crear` |
| 4 | `Pages/BusinessesEdit.razor` | `/empresas/editar/{Id}` |
| 5 | `Pages/Branches.razor` | `/sucursales` |
| 6 | `Pages/BranchesCreate.razor` | `/sucursales/crear` |
| 7 | `Pages/BranchesEdit.razor` | `/sucursales/editar/{Id}` |
| 8 | `Pages/Roles.razor` | `/roles` |
| 9 | `Pages/RolesCreate.razor` | `/roles/crear` |
| 10 | `Pages/RolesEdit.razor` | `/roles/editar/{Id}` |
| 11 | `Pages/Users.razor` | `/usuarios` |
| 12 | `Pages/UsersCreate.razor` | `/usuarios/crear` |
| 13 | `Pages/UsersEdit.razor` | `/usuarios/editar/{Id}` |
| 14 | `Pages/Employees.razor` | `/empleados` |
| 15 | `Pages/HealthPromotionEntities.razor` | `/EPS` |
| 16 | `Pages/OccupationalRiskAdministrators.razor` | `/ARL` |

**Total: 16 páginas protegidas.** Las páginas públicas (sin `[Authorize]`) son `Home.razor` (login) y `NotFound.razor` (404).

---

## 5. Decisiones técnicas

### 5.1 `forceLoad: false` en todas las navegaciones internas

| Navegación | forceLoad | Razón |
|------------|-----------|-------|
| Login → `/dashboard` | `false` | `MarkUserAsAuthenticated` notifica estado ANTES de navegar. `AuthorizeRouteView` lo ve inmediatamente. |
| `/` → `/dashboard` (auth redirect) | `false` | Auth state ya está disponible del proveedor. No necesita recarga. |
| `RedirectToLogin` → `/` | `false` | Estado no-auth ya está en el proveedor. Navegación suave es suficiente. |
| Sesión expirada → `/` | `false` | Ya está en `MainLayout`. Mantener. |
| Logout → `/` | `true` | NO se cambia. Recarga dura necesaria para limpiar estado fantasma del proveedor y handlers HTTP. |

### 5.2 `CascadingAuthenticationState` es necesario

`AuthorizeRouteView` depende de `CascadingAuthenticationState` para recibir el estado de autenticación actualizado. Sin él, `AuthorizeRouteView` no sabe cuándo el usuario inicia sesión o cierra sesión, y no puede re-evaluar las rutas. `CascadingAuthenticationState` se suscribe a `NotifyAuthenticationStateChanged` del `AuthenticationStateProvider` registrado.

### 5.3 Cómo evitar el loop de redirección

El loop potencial sería: `/` → `/dashboard` (auth check) → `/` (algo invalida auth) → `/dashboard` → ...

Esto se evita porque:

1. **`Home.razor` solo redirige UNA dirección**: de `/` a `/dashboard`. Nunca redirige de vuelta a `/`.
2. **`AuthorizeRouteView` no redirige**: solo muestra `NotAuthorized` si no hay auth. No hay navegación desde `AuthorizeRouteView` mismo.
3. **`RedirectToLogin` solo va a `/`**: nunca a `/dashboard` ni otra ruta protegida.
4. **El flujo de sesión expirada**: `MainLayout` redirige a `/`, `Home.razor` verifica auth → como no está autenticado, NO redirige a dashboard. Se queda en `/` mostrando login.

### 5.4 Manejo de sesión expirada

El flujo existente es correcto y no se modifica:

```
API 401 → AuthRefreshHandler → refresh falla → SessionExpiredNotifier.Notify()
  → MainLayout.OnSessionExpired → NavigateTo("/", forceLoad: false)
  → Home.razor verifica auth → no auth → muestra login
```

La protección adicional que aporta este cambio: si por algún motivo el usuario llegara a una ruta protegida sin estar autenticado (ej: state race), `AuthorizeRouteView` + `RedirectToLogin` lo redirige a `/`, donde `Home.razor` confirma que no está autenticado.

---

## 6. Riesgos y mitigaciones

### 6.1 Flash de contenido no autorizado

**Riesgo**: Que `AuthorizeRouteView` renderice brevemente el contenido de la página protegida antes de determinar que no hay autorización, y luego muestre `NotAuthorized`.

**Mitigación**: `AuthorizeRouteView` en Blazor WASM NO renderiza el contenido de la ruta hasta que `GetAuthenticationStateAsync()` resuelve. El contenido protegido nunca se renderiza para usuarios no autenticados. El `NotAuthorized` se muestra inmediatamente cuando el estado no-auth está disponible.

### 6.2 Loop de redirección

**Riesgo**: Usuario auth en `/` → `/dashboard` → algo lo redirige de vuelta a `/` → loop infinito.

**Mitigación**: Diseño unidireccional explicado en 5.3. La única redirección automática desde Home a dashboard ocurre cuando auth=true. No hay código que redirija de dashboard a home automáticamente. La expiración de sesión va a `/`, pero ahí Home detecta no-auth y se queda.

### 6.3 Rendimiento de `GetAuthenticationStateAsync` en cada navegación

**Riesgo**: `GetAuthenticationStateAsync()` hace una llamada HTTP a `/auth/me` en cada invocación (ver `CustomAuthenticationStateProvider`). En cada navegación a una ruta protegida, `AuthorizeRouteView` llama a este método.

**Mitigación**:
- El proveedor ya maneja `UnauthorizedAccessException` con refresh token, minimizando fallos visibles.
- Este riesgo está documentado como fuera de alcance en la propuesta. Se abordará en un cambio futuro de caching de auth state si el rendimiento es problema.
- En la práctica, el servidor responde rápido a `/auth/me` porque la cookie HttpOnly se envía automáticamente y la validación JWT es ligera.

### 6.4 `NotFound.razor` con contenido sensible

**Riesgo**: La página 404 usa `@layout MainLayout`. Si el layout muestra información del usuario autenticado (nombre en AppBar, menú de navegación), un usuario no autenticado en `/not-found` vería elementos pensados para usuarios auth.

**Mitigación**: El `MainLayout` actual ya maneja `_isAuthenticated` mediante `StatusSessionComponent` y oculta condicionalmente el menú y otros elementos. No hay cambio necesario. El `NotFound` del Router (`NotFoundPage`) se renderiza fuera de `AuthorizeRouteView`, por lo que es accesible sin auth.

### 6.5 Auth state race en carga inicial

**Riesgo**: En la primera carga WASM, `CustomAuthenticationStateProvider.GetAuthenticationStateAsync()` se ejecuta concurrentemente con la inicialización de componentes. Podría haber un momento donde `AuthorizeRouteView` evalúa auth antes de que el proveedor complete su primera llamada HTTP.

**Mitigación**: `GetAuthenticationStateAsync()` es `async` y `AuthorizeRouteView` awaita su resultado antes de decidir. Durante la espera, Blazor muestra el contenido del `NotAuthorized` — que es `RedirectToLogin`. Si hay una demora, el usuario ve una redirección momentánea a `/`. Esto es aceptable y no muestra contenido protegido.
