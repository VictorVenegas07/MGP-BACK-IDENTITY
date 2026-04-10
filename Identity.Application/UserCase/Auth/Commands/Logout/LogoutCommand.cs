using Identity.Application.UseCase.Auth.DTOs;
using Identity.Application.UseCase.Auth.Services;
using MediatR;

namespace Identity.Application.UseCase.Auth.Commands.Logout;

public class LogoutCommand : IRequest<SessionTerminationDto>
{
    public string RefreshToken { get; set; } = null!;
}

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, SessionTerminationDto>
{
    private readonly SessionRevocationService _sessionRevocationService;

    public LogoutCommandHandler(SessionRevocationService sessionRevocationService)
    {
        _sessionRevocationService = sessionRevocationService;
    }

    public async Task<SessionTerminationDto> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var result = await _sessionRevocationService.RevokeByRefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (!result.Found)
        {
            return new SessionTerminationDto
            {
                Success = true,
                AlreadyRevoked = true,
                Message = "Logout completed."
            };
        }

        return new SessionTerminationDto
        {
            Success = true,
            AlreadyRevoked = result.AlreadyRevoked,
            Message = result.AlreadyRevoked
                ? "The session was already revoked."
                : "Logout completed."
        };
    }
}
