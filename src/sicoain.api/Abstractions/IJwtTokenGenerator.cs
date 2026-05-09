using sicoain.shared.Entities;

namespace sicoain.api.Abstractions
{
    internal interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
