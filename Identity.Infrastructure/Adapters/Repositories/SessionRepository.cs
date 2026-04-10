using Identity.Domain.Entities.Identity;
using Identity.Domain.Ports.Repositories;
using Identity.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Adapters.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly PersistenceContext _context;

    public SessionRepository(PersistenceContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Session session, CancellationToken cancellationToken = default)
    {
        await _context.Sessions.AddAsync(session, cancellationToken);
    }

    public async Task<Session?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await _context.Sessions
            .FirstOrDefaultAsync(x => x.Id == sessionId && !x.IsDeleted, cancellationToken);
    }
}