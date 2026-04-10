using Identity.Application.UseCase.Auth.DTOs;
using Identity.Application.UseCase.Auth.Services;
using MediatR;

namespace Identity.Application.UseCase.Auth.Commands.RevokeRefreshToken;

public class RevokeRefreshTokenCommand : IRequest<SessionTerminationDto>
{
    public string RefreshToken { get; set; } = null!;
}

public class RevokeRefreshTokenCommandHandler : IRequestHandler<RevokeRefreshTokenCommand, SessionTerminationDto>
{
    private readonly SessionRevocationService _sessionRevocationService;

    public RevokeRefreshTokenCommandHandler(SessionRevocationService sessionRevocationService)
    {
        _sessionRevocationService = sessionRevocationService;
    }

    public async Task<SessionTerminationDto> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var result = await _sessionRevocationService.RevokeByRefreshTokenAsync(request.RefreshToken, cancellationToken);

        return new SessionTerminationDto
        {
            Success = true,
            AlreadyRevoked = !result.Found || result.AlreadyRevoked,
            Message = !result.Found || result.AlreadyRevoked
                ? "The refresh token was already revoked."
                : "The refresh token was revoked successfully."
        };
    }
}
