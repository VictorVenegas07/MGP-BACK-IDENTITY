namespace Identity.Infrastructure.Adapters.Security;

public class EmailVerificationSettings
{
    public const string SectionName = "EmailVerificationSettings";

    public int TokenExpirationHours { get; set; } = 24;
    public string VerificationUrl { get; set; } = null!;
}
