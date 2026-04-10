using Identity.Domain.Entities.Identity;
using Identity.Domain.Enums;
using Identity.Domain.Ports.Repositories;
using Identity.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Adapters.Repositories;

public class VerificationTokenRepository : IVerificationTokenRepository
{
    private readonly PersistenceContext _context;

    public VerificationTokenRepository(PersistenceContext context)
    {
        _context = context;
    }

    public async Task<VerificationToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        return await _context.VerificationTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash && !x.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(VerificationToken verificationToken, CancellationToken cancellationToken = default)
    {
        await _context.VerificationTokens.AddAsync(verificationToken, cancellationToken);
    }

    public async Task InvalidateActiveTokensAsync(
        Guid userId,
        VerificationTokenType type,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var tokens = await _context.VerificationTokens
            .Where(x =>
                x.UserId == userId &&
                x.Type == type &&
                !x.IsDeleted &&
                !x.UsedAt.HasValue &&
                x.ExpiresAt > now)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.MarkAsDeleted();
            token.SetDelete();
        }
    }
}
