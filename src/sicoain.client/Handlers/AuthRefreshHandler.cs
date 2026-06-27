using System.Net;
using System.Net.Http.Headers;
using sicoain.client.Services;

namespace sicoain.client.Handlers;

/// <summary>
/// Intercepts HTTP 401 responses, attempts to refresh the access token
/// via the refresh endpoint, and retries the original request on success.
/// Prevents infinite loops by skipping the refresh endpoint itself.
/// When the refresh token itself has expired (POST /auth/refresh returns 401),
/// fires <see cref="SessionExpiredNotifier"/> so the layout can redirect to
/// the login page.
/// </summary>
public class AuthRefreshHandler : DelegatingHandler
{
    private const string RefreshEndpointPath = "/auth/refresh";

    private static int _isRefreshing = 0;
    private readonly SessionExpiredNotifier _sessionExpiredNotifier;

    public AuthRefreshHandler(SessionExpiredNotifier sessionExpiredNotifier)
    {
        _sessionExpiredNotifier = sessionExpiredNotifier;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        // Only intercept 401 responses, and never intercept the refresh call itself
        if (response.StatusCode != HttpStatusCode.Unauthorized)
            return response;

        if (request.RequestUri?.AbsolutePath.Contains(RefreshEndpointPath, StringComparison.OrdinalIgnoreCase) == true)
            return response;

        // Prevent concurrent refresh attempts — if another 401 is already
        // refreshing, return the original 401 for this one
        if (Interlocked.CompareExchange(ref _isRefreshing, 1, 0) != 0)
            return response;

        try
        {
            // Buffer the original request content so we can retry it
            byte[]? bufferedContent = null;
            if (request.Content != null)
            {
                bufferedContent = await request.Content.ReadAsByteArrayAsync(cancellationToken);
            }

            // Build the refresh request — it's relative to the same base address
            var refreshUri = new Uri(
                request.RequestUri!.GetLeftPart(UriPartial.Authority) + "/api/v1/auth/refresh");

            using var refreshRequest = new HttpRequestMessage(HttpMethod.Post, refreshUri);

            var refreshResponse = await base.SendAsync(refreshRequest, cancellationToken);

            if (refreshResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Refresh token is also expired — session is over.
                // The layout will redirect the user to the login page.
                _sessionExpiredNotifier.Notify();
                return response;
            }

            if (!refreshResponse.IsSuccessStatusCode)
            {
                // Some other server error (500, etc.) — return the original 401
                // without redirecting, so the caller can show a meaningful error.
                return response;
            }

            // Build a new request to retry the original call
            using var retryRequest = new HttpRequestMessage(request.Method, request.RequestUri);

            // Copy request headers
            foreach (var header in request.Headers)
            {
                retryRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            // Copy content with buffered data
            if (bufferedContent != null && bufferedContent.Length > 0)
            {
                retryRequest.Content = new ByteArrayContent(bufferedContent);
                if (request.Content?.Headers != null)
                {
                    foreach (var header in request.Content.Headers)
                    {
                        retryRequest.Content.Headers.TryAddWithoutValidation(
                            header.Key, header.Value);
                    }
                }
            }

            // Retry with the fresh token (cookies are updated by the refresh call)
            response.Dispose();
            return await base.SendAsync(retryRequest, cancellationToken);
        }
        finally
        {
            Interlocked.Exchange(ref _isRefreshing, 0);
        }
    }
}
