using MediatR;
using Identity.Domain.Common.Exceptions;
using Identity.Domain.Entities.Auth;
using Identity.Domain.Entities.Identity;
using Identity.Domain.Enums;
using Identity.Domain.Ports;
using Identity.Domain.Ports.Repositories;
using Identity.Domain.Ports.Security;

namespace Identity.Application.UseCase.Auth.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<RefreshTokenDto>
{
    public string RefreshToken { get; set; } = null!;
}

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenDto>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly ITokenHashingService _tokenHashingService;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        ISessionRepository sessionRepository,
        ITokenHashingService tokenHashingService,
        ITokenService tokenService,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _sessionRepository = sessionRepository;
        _tokenHashingService = tokenHashingService;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<RefreshTokenDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var incomingTokenHash = _tokenHashingService.Hash(request.RefreshToken);

        var currentRefreshToken = await _refreshTokenRepository.GetByTokenHashAsync(incomingTokenHash, cancellationToken);

        if (currentRefreshToken is null)
            throw new UnauthorizedException("Invalid refresh token.");

        var session = currentRefreshToken.Session;
        var user = currentRefreshToken.User;

        if (session is null || user is null)
            throw new UnauthorizedException("Invalid refresh token.");

        if (currentRefreshToken.RevokedAt.HasValue)
        {
            currentRefreshToken.ReuseDetectedAt = now;

            session.Status = SessionStatus.Revoked;
            session.RevokedAt = now;
            await _refreshTokenRepository.RevokeBySessionIdAsync(session.Id, now, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            throw new UnauthorizedException("Refresh token reuse detected. Session revoked.");
        }

        if (currentRefreshToken.ExpiresAt <= now)
            throw new UnauthorizedException("Refresh token expired.");

        if (session.Status != SessionStatus.Active)
            throw new UnauthorizedException("Session is not active.");

        if (session.ExpiresAt <= now)
            throw new UnauthorizedException("Session expired.");

        var newRawRefreshToken = _tokenService.GenerateRefreshToken();
        var newRefreshTokenHash = _tokenHashingService.Hash(newRawRefreshToken);
        var refreshTokenExpiresAt = _tokenService.GetRefreshTokenExpirationUtc();

        var newRefreshToken = new Domain.Entities.Identity.RefreshToken
        {
            SessionId = session.Id,
            UserId = user.Id,
            TenantId = currentRefreshToken.TenantId,
            TokenHash = newRefreshTokenHash,
            IssuedAt = now,
            ExpiresAt = refreshTokenExpiresAt,
            RotatedFromTokenId = currentRefreshToken.Id,
        };

        currentRefreshToken.RevokedAt = now;
        currentRefreshToken.ReplacedByTokenId = newRefreshToken.Id;

        session.LastSeenAt = now;

        await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

        var accessTokenExpiresAt = _tokenService.GetAccessTokenExpirationUtc();

        var payload = new JwtPayloadData
        {
            UserId = user.Id,
            SessionId = session.Id,
            TenantId = currentRefreshToken.TenantId,
            Email = user.PrimaryEmail,
            UserName = user.UserName
        };

        var newAccessToken = _tokenService.GenerateAccessToken(payload);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RefreshTokenDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRawRefreshToken,
            AccessTokenExpiresAt = accessTokenExpiresAt
        };
    }
}
