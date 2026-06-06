using Microsoft.JSInterop;
using System.Net.Http;

namespace sicoain.client.Handlers
{
    public class CsrfHandler : DelegatingHandler
    {
        private readonly IJSRuntime _jsRuntime;

        public CsrfHandler(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Post ||
                request.Method == HttpMethod.Put ||
                request.Method == HttpMethod.Patch ||
                request.Method == HttpMethod.Delete)
            {
                var csrfToken = await _jsRuntime.InvokeAsync<string>("eval", "document.cookie.split('; ').find(row => row.startsWith('csrf_token='))?.split('=')[1]");

                if (!string.IsNullOrEmpty(csrfToken))
                {
                    request.Headers.Add("X-CSRF-TOKEN", csrfToken);
                }
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
