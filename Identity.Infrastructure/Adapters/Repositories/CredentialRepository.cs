using Identity.Domain.Entities.Identity;
using Identity.Domain.Enums;
using Identity.Domain.Ports.Repositories;
using Identity.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Adapters.Repositories;

public class CredentialRepository : ICredentialRepository
{
    private readonly PersistenceContext _context;

    public CredentialRepository(PersistenceContext context)
    {
        _context = context;
    }

    public async Task<Credential?> GetActiveByUserIdAndTypeAsync(
        Guid userId,
        CredentialType type,
        CancellationToken cancellationToken = default)
    {
        return await _context.Credentials.FirstOrDefaultAsync(
            x => x.UserId == userId
                 && x.Type == type
                 && x.Status == CredentialStatus.Active
                 && !x.IsDeleted,
            cancellationToken);
    }

    public async Task AddAsync(Credential credential, CancellationToken cancellationToken = default)
    {
        await _context.Credentials.AddAsync(credential, cancellationToken);
    }
}