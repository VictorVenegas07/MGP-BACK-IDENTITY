using Identity.Domain.Entities.Base;
using Identity.Domain.Enums;

namespace Identity.Domain.Entities.Identity;

public class TenantMembership : BaseEntity<Guid>
{
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }

    public string RoleCode { get; set; } = null!;
    public MembershipStatus Status { get; set; } = MembershipStatus.Active;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public virtual User User { get; set; } = null!;
}