using Identity.Domain.Entities.Base;
using Identity.Domain.Enums;

namespace Identity.Domain.Entities.Identity;

public class Credential : BaseEntity<Guid>
{
    public Guid UserId { get; set; }
    public CredentialType Type { get; set; } = CredentialType.Password;
    public CredentialStatus Status { get; set; } = CredentialStatus.Active;

    public string SecretHash { get; set; } = null!;
    public string? Algorithm { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? LastUsedAt { get; set; }

    public virtual User User { get; set; } = null!;
}