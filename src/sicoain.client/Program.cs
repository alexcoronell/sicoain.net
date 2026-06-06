using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using sicoain.client;
using sicoain.client.Handlers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddMudServices();

// Register the CSRF handler (requires IJSRuntime, is injected automatically)
builder.Services.AddTransient<CsrfHandler>();

// Configure an HttpClient with a name that includes the CSRF handler
builder.Services.AddHttpClient("SicoainApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5078");
})
.AddHttpMessageHandler<CsrfHandler>();

// Register a default HttpClient (unnamed) that uses the above configuration
builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient("SicoainApi");
});

await builder.Build().RunAsync();
