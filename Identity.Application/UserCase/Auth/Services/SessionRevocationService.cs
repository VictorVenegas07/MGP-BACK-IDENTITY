using Identity.Domain.Enums;
using Identity.Domain.Ports;
using Identity.Domain.Ports.Repositories;
using Identity.Domain.Ports.Security;

namespace Identity.Application.UseCase.Auth.Services;

public class SessionRevocationService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenHashingService _tokenHashingService;
    private readonly IUnitOfWork _unitOfWork;

    public SessionRevocationService(
        IRefreshTokenRepository refreshTokenRepository,
        ITokenHashingService tokenHashingService,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _tokenHashingService = tokenHashingService;
        _unitOfWork = unitOfWork;
    }

    public async Task<SessionRevocationResult> RevokeByRefreshTokenAsync(
        string rawRefreshToken,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = _tokenHashingService.Hash(rawRefreshToken.Trim());
        var refreshToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);

        if (refreshToken is null || refreshToken.Session is null)
        {
            return new SessionRevocationResult
            {
                Found = false
            };
        }

        var now = DateTime.UtcNow;
        var session = refreshToken.Session;
        var alreadyRevoked = session.Status == SessionStatus.Revoked || refreshToken.RevokedAt.HasValue;

        session.Status = SessionStatus.Revoked;
        session.RevokedAt ??= now;
        session.LastSeenAt = now;

        await _refreshTokenRepository.RevokeBySessionIdAsync(session.Id, now, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SessionRevocationResult
        {
            Found = true,
            AlreadyRevoked = alreadyRevoked
        };
    }
}

public class SessionRevocationResult
{
    public bool Found { get; set; }
    public bool AlreadyRevoked { get; set; }
}
