using Microsoft.AspNetCore.Http;

namespace sicoain.api.Abstractions
{
    public interface ICookieManager
    {
        void SetTokenCookie(string key, string token, int minutes);
        string? GetCookieValue(string key);
        void DeleteCookie(string key);
        HttpContext? GetHttpContext();
    }
}
