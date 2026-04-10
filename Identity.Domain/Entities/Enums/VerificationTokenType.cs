namespace Identity.Domain.Enums;

public enum VerificationTokenType
{
    EmailVerification = 1,
    PasswordReset = 2,
    MagicLink = 3,
    Invite = 4
}