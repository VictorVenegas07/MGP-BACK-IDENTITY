namespace Identity.Domain.Enums;

public enum CredentialType
{
    Password = 1,
    Totp = 2,
    WebAuthn = 3,
    RecoveryCode = 4
}