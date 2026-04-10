namespace Identity.Application.UseCase.Auth.DTOs;

public class EmailVerificationResultDto
{
    public bool Success { get; set; }
    public bool AlreadyVerified { get; set; }
    public string Message { get; set; } = null!;
}
