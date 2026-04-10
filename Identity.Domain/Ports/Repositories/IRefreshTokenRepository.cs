using Identity.Domain.Entities.Identity;

namespace Identity.Domain.Ports.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task RevokeBySessionIdAsync(Guid sessionId, DateTime revokedAt, CancellationToken cancellationToken = default);
}
