using Identity.Domain.Entities.Identity;

namespace Identity.Domain.Ports.Repositories;

public interface ITenantMembershipRepository
{
    Task<TenantMembership?> GetByUserIdAndTenantIdAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default);
}
