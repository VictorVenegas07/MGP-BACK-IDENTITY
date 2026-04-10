namespace Identity.Application.UseCase.Auth.DTOs;

public class SessionTerminationDto
{
    public bool Success { get; set; }
    public bool AlreadyRevoked { get; set; }
    public string Message { get; set; } = null!;
}
