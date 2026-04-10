using Identity.Domain.Entities.Identity;
using Identity.Domain.Ports.Repositories;
using Identity.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Adapters.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly PersistenceContext _context;

    public RefreshTokenRepository(PersistenceContext context)
    {
        _context = context;
    }

    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
    }

    public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        return await _context.RefreshTokens
            .Include(x => x.Session)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash && !x.IsDeleted, cancellationToken);
    }

    public async Task RevokeBySessionIdAsync(Guid sessionId, DateTime revokedAt, CancellationToken cancellationToken = default)
    {
        var refreshTokens = await _context.RefreshTokens
            .Where(x => x.SessionId == sessionId && !x.IsDeleted && !x.RevokedAt.HasValue)
            .ToListAsync(cancellationToken);

        foreach (var refreshToken in refreshTokens)
        {
            refreshToken.RevokedAt = revokedAt;
        }
    }
}
