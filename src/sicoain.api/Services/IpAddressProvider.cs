using sicoain.api.Abstractions;

namespace sicoain.api.Services
{
    internal class IpAddressProvider : IIpAddressProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IpAddressProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentIpAddress()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return "unknown";
            }

            // Check for forwarded IP (when behind a proxy or load balancer)
            var forwadedIp = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwadedIp))
            {
                return forwadedIp.Split(",").First().Trim();
            }

            // Fallback to remote IP address
            return httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }
}
