namespace Identity.Infrastructure.Adapters.Security;

public class SmtpEmailSettings
{
    public const string SectionName = "SmtpEmailSettings";

    public bool Enabled { get; set; }
    public string Host { get; set; } = null!;
    public int Port { get; set; } = 25;
    public bool UseSsl { get; set; } = true;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string FromEmail { get; set; } = null!;
    public string? FromName { get; set; }
}
