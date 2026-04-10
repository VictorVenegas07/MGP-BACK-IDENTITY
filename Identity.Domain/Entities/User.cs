using Identity.Domain.Entities.Base;
using Identity.Domain.Enums;

namespace Identity.Domain.Entities.Identity;

public class User : BaseEntity<Guid>
{
    public string PrimaryEmail { get; set; } = null!;
    public bool EmailVerified { get; set; }
    public string? UserName { get; set; }
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
    public UserStatus Status { get; set; } = UserStatus.PendingVerification;
    public DateTime? LastLoginAt { get; set; }
    public bool IsPlatformAdmin { get; set; }

    public virtual ICollection<Credential> Credentials { get; set; } = new HashSet<Credential>();
    public virtual ICollection<ExternalIdentity> ExternalIdentities { get; set; } = new HashSet<ExternalIdentity>();
    public virtual ICollection<Session> Sessions { get; set; } = new HashSet<Session>();
    public virtual ICollection<TenantMembership> TenantMemberships { get; set; } = new HashSet<TenantMembership>();
    public virtual ICollection<VerificationToken> VerificationTokens { get; set; } = new HashSet<VerificationToken>();
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();
}