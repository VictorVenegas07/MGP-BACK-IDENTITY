using Identity.Domain.Entities.Identity;

namespace Identity.Domain.Ports.Repositories;

public interface ISessionRepository
{
    Task AddAsync(Session session, CancellationToken cancellationToken = default);
     Task<Session?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
}