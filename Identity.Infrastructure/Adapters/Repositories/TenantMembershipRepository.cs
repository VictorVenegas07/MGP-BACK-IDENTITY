using Identity.Domain.Entities.Identity;
using Identity.Domain.Ports.Repositories;
using Identity.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Adapters.Repositories;

public class TenantMembershipRepository : ITenantMembershipRepository
{
    private readonly PersistenceContext _context;

    public TenantMembershipRepository(PersistenceContext context)
    {
        _context = context;
    }

    public async Task<TenantMembership?> GetByUserIdAndTenantIdAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.TenantMemberships
            .FirstOrDefaultAsync(
                x => x.UserId == userId && x.TenantId == tenantId && !x.IsDeleted,
                cancellationToken);
    }
}
