using sicoain.shared.Entities;

namespace sicoain.api.Abstractions
{
    internal interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task RevokeAsync(RefreshToken token, string revokedByIp, string? reason = null);
        Task<int> RevokeAllForUserAsync(int userId, string revokedByIp, string? reason = null);
        Task UpdateAsync(RefreshToken refreshToken);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
