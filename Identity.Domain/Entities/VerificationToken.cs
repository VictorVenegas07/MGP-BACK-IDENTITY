using Identity.Domain.Entities.Base;
using Identity.Domain.Enums;

namespace Identity.Domain.Entities.Identity;

public class VerificationToken : BaseEntity<Guid>
{
    public Guid UserId { get; set; }
    public VerificationTokenType Type { get; set; }

    public string TokenHash { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }

    public virtual User User { get; set; } = null!;
}