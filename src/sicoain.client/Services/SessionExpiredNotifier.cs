namespace sicoain.client.Services;

/// <summary>
/// Fires an event when the user session has expired (refresh token also invalid).
/// Consumed by the layout to redirect to the login page.
/// Registered as a singleton — no dependencies, safe for injection into
/// DelegatingHandlers without circular references.
/// </summary>
public sealed class SessionExpiredNotifier
{
    public event Action? SessionExpired;

    public void Notify() => SessionExpired?.Invoke();
}
