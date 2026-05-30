using System.Net;

namespace sicoain.IntegrationTests.Utilities;

/// <summary>
/// DelegatingHandler that manually manages cookies from Set-Cookie response
/// headers and attaches them to subsequent requests. Bypasses the default
/// CookieContainer behavior which would discard Secure cookies over HTTP
/// due to the CookieManager source-code bug (Secure flag inverted).
/// </summary>
public sealed class CookieHandler : DelegatingHandler
{
    private readonly Dictionary<string, string> _cookies = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, string> Cookies => _cookies;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Attach stored cookies to the request
        if (_cookies.Count > 0)
        {
            var cookieHeader = string.Join("; ", _cookies.Select(c => $"{c.Key}={c.Value}"));
            request.Headers.TryAddWithoutValidation("Cookie", cookieHeader);
        }

        var response = await base.SendAsync(request, cancellationToken);

        // Store cookies from the response's Set-Cookie headers
        if (response.Headers.TryGetValues("Set-Cookie", out var setCookies))
        {
            foreach (var cookieStr in setCookies)
            {
                var nameValue = cookieStr.Split(';')[0];
                var parts = nameValue.Split('=', 2);
                if (parts.Length == 2)
                {
                    if (parts[1].Length == 0)
                        _cookies.Remove(parts[0]);
                    else
                        _cookies[parts[0]] = parts[1];
                }
            }
        }

        return response;
    }

    public void Clear() => _cookies.Clear();
}
