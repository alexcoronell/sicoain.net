using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using MudBlazor.Services;
using sicoain.client;
using sicoain.client.Abstractions;
using sicoain.client.Handlers;
using sicoain.client.Services;
using sicoain.client.Providers;
using sicoain.shared.Entities;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddMudServices();

// Register the CSRF handler (requires IJSRuntime, is injected automatically)
builder.Services.AddTransient<CsrfHandler>();
builder.Services.AddTransient<CredentialsHandler>();

// Configure an HttpClient with a name that includes the CSRF handler
builder.Services.AddHttpClient("SicoainApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5078");
})
.AddHttpMessageHandler<CredentialsHandler>()
.AddHttpMessageHandler<CsrfHandler>();

// Register a default HttpClient (unnamed) that uses the above configuration
builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient("SicoainApi");
});

//Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();

// Register the custom authentication state provider
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

await builder.Build().RunAsync();
