using Identity.Domain.Entities.Base;

namespace Identity.Domain.Entities.Identity;

public class RefreshToken : BaseEntity<Guid>
{
    public Guid SessionId { get; set; }
    public Guid UserId { get; set; }
    public Guid? TenantId { get; set; }

    public string TokenHash { get; set; } = null!;
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public DateTime? ReuseDetectedAt { get; set; }

    public Guid? RotatedFromTokenId { get; set; }
    public Guid? ReplacedByTokenId { get; set; }

    public virtual Session Session { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}