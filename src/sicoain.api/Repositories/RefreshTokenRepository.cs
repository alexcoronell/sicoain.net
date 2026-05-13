using Microsoft.EntityFrameworkCore;
using sicoain.api.Abstractions;
using sicoain.api.Data;
using sicoain.shared.Entities;

namespace sicoain.api.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public RefreshTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken).ConfigureAwait(false);
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token).ConfigureAwait(false);
        }

        public async Task RevokeAsync(RefreshToken token, string revokedByIp, string? reason = null)
        {
            token.Revoke(revokedByIp, reason);
            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task<int> RevokeAllForUserAsync(int userId, string revokedByIp, string? reason = null)
        {
            var tokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
                .ToListAsync().ConfigureAwait(false);

            foreach (var token in tokens)
            {
                token.Revoke(revokedByIp, reason);
            }

            return tokens.Count;
        }

        public async Task UpdateAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
