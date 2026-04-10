using Identity.Domain.Entities.Identity;
using Identity.Domain.Enums;

namespace Identity.Domain.Ports.Repositories;

public interface ICredentialRepository
{
    Task<Credential?> GetActiveByUserIdAndTypeAsync(
        Guid userId,
        CredentialType type,
        CancellationToken cancellationToken = default);

    Task AddAsync(Credential credential, CancellationToken cancellationToken = default);
}