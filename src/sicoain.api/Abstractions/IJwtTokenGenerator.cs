using System.Security.Claims;
using sicoain.shared.Entities;

namespace sicoain.api.Abstractions
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user, List<Claim>? additionalClaims = null);
    }
}
