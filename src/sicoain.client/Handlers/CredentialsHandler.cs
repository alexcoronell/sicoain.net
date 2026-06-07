using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace sicoain.client.Handlers
{
    public class CredentialsHandler: DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
