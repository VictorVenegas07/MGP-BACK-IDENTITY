using Identity.Domain.Entities.Base;
using Identity.Domain.Enums;

namespace Identity.Domain.Entities.Identity;

public class ExternalIdentity : BaseEntity<Guid>
{
    public Guid UserId { get; set; }
    public ExternalProviderType ProviderType { get; set; }

    public string ProviderSubject { get; set; } = null!;
    public string? ProviderEmail { get; set; }
    public bool ProviderEmailVerified { get; set; }
    public DateTime LinkedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }

    public virtual User User { get; set; } = null!;
}