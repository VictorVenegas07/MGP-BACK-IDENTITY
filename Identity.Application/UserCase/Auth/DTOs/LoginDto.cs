namespace Identity.Application.UseCase.Auth.Commands.Login;

public class LoginDto
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime AccessTokenExpiresAt { get; set; }

    public Guid UserId { get; set; }
    public string Email { get; set; } = null!;
    public string? UserName { get; set; }
    public string? DisplayName { get; set; }
}