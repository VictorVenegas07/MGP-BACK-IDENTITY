namespace Identity.Application.UseCase.Auth.Commands.CreateUser;

public class CreateUserDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = null!;
    public string? UserName { get; set; }
    public string? DisplayName { get; set; }
    public bool EmailVerified { get; set; }
    public DateTime VerificationTokenExpiresAt { get; set; }
}
