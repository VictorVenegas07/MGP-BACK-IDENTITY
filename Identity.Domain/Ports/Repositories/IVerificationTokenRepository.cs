using Identity.Domain.Entities.Identity;
using Identity.Domain.Enums;

namespace Identity.Domain.Ports.Repositories;

public interface IVerificationTokenRepository
{
    Task<VerificationToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task AddAsync(VerificationToken verificationToken, CancellationToken cancellationToken = default);
    Task InvalidateActiveTokensAsync(
        Guid userId,
        VerificationTokenType type,
        CancellationToken cancellationToken = default);
}
