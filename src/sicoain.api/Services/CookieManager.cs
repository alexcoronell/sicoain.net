using Microsoft.AspNetCore.Http;
using sicoain.api.Abstractions;

namespace sicoain.api.Services;

public class CookieManager : ICookieManager
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CookieManager(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void SetTokenCookie(string key, string token, int minutes)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.Response == null) return;

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = httpContext.Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(minutes)
        };
        httpContext.Response.Cookies.Append(key, token, cookieOptions);
    }

    public string? GetCookieValue(string key)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.Request.Cookies.TryGetValue(key, out var value) == true)
        {
            return value;
        }
        return null;
    }

    public void DeleteCookie(string key)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.Response == null) return;

        httpContext.Response.Cookies.Delete(key, new CookieOptions
        {
            HttpOnly = true,
            Secure = httpContext.Request.IsHttps,
            SameSite = SameSiteMode.Strict
        });
    }

    public HttpContext? GetHttpContext()
    {
        return _httpContextAccessor.HttpContext;
    }
}
