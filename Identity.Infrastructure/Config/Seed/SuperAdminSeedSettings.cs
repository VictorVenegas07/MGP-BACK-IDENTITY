namespace Identity.Infrastructure.Config.Seed;

public class SuperAdminSeedSettings
{
    public const string SectionName = "SuperAdminSeed";

    public bool Enabled { get; set; }
    public string Email { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string Password { get; set; } = null!;
}