using Identity.Domain.Entities.Base;
using Identity.Domain.Enums;

namespace Identity.Domain.Entities.Identity;

public class Session : BaseEntity<Guid>
{
    public Guid UserId { get; set; }
    public Guid? TenantId { get; set; }

    public string AuthMethod { get; set; } = null!;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? DeviceName { get; set; }

    public SessionStatus Status { get; set; } = SessionStatus.Active;
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastSeenAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();
}