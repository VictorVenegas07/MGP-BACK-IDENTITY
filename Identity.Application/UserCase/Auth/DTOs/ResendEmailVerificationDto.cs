namespace Identity.Application.UseCase.Auth.DTOs;

public class ResendEmailVerificationDto
{
    public string Email { get; set; } = null!;
    public DateTime VerificationTokenExpiresAt { get; set; }
    public string Message { get; set; } = null!;
}
