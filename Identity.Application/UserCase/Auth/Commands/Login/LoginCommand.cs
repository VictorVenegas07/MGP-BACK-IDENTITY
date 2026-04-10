using MediatR;
using Identity.Domain.Common.Exceptions;
using Identity.Domain.Entities.Auth;
using Identity.Domain.Entities.Identity;
using Identity.Domain.Enums;
using Identity.Domain.Ports;
using Identity.Domain.Ports.Repositories;
using Identity.Domain.Ports.Security;

namespace Identity.Application.UseCase.Auth.Commands.Login;

public class LoginCommand : IRequest<LoginDto>
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public Guid? TenantId { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? DeviceName { get; set; }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantMembershipRepository _tenantMembershipRepository;
    private readonly ICredentialRepository _credentialRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenHashingService _tokenHashingService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        ITenantMembershipRepository tenantMembershipRepository,
        ICredentialRepository credentialRepository,
        ISessionRepository sessionRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IUnitOfWork unitOfWork,
        ITokenHashingService tokenHashingService)
    {
        _userRepository = userRepository;
        _tenantMembershipRepository = tenantMembershipRepository;
        _credentialRepository = credentialRepository;
        _sessionRepository = sessionRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
        _tokenHashingService = tokenHashingService;
    }

    public async Task<LoginDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (user is null)
            throw new UnauthorizedException("Invalid credentials.");

        if (user.Status != UserStatus.Active && user.Status != UserStatus.PendingVerification)
            throw new ForbiddenException("The user is not allowed to log in.");

        var credential = await _credentialRepository.GetActiveByUserIdAndTypeAsync(
            user.Id,
            CredentialType.Password,
            cancellationToken);

        if (credential is null)
            throw new UnauthorizedException("Invalid credentials.");

        var isValidPassword = _passwordHasher.Verify(request.Password, credential.SecretHash);
        if (!isValidPassword)
            throw new UnauthorizedException("Invalid credentials.");

        if (!user.EmailVerified || user.Status == UserStatus.PendingVerification)
            throw new ForbiddenException("You must verify your email address before logging in.");

        if (request.TenantId.HasValue)
        {
            var tenantMembership = await _tenantMembershipRepository.GetByUserIdAndTenantIdAsync(
                user.Id,
                request.TenantId.Value,
                cancellationToken);

            if (tenantMembership is null)
                throw new ForbiddenException("The user does not belong to the requested tenant.");

            if (tenantMembership.Status != MembershipStatus.Active)
            {
                throw tenantMembership.Status switch
                {
                    MembershipStatus.Pending => new ForbiddenException("The tenant membership is still pending approval."),
                    MembershipStatus.Suspended => new ForbiddenException("The tenant membership is suspended."),
                    MembershipStatus.Removed => new ForbiddenException("The tenant membership was removed."),
                    _ => new ForbiddenException("The user is not allowed to access the requested tenant.")
                };
            }
        }

        var now = DateTime.UtcNow;

        var session = new Session
        {
            UserId = user.Id,
            TenantId = request.TenantId,
            AuthMethod = "password",
            IpAddress = request.IpAddress,
            UserAgent = request.UserAgent,
            DeviceName = request.DeviceName,
            Status = SessionStatus.Active,
            StartedAt = now,
            LastSeenAt = now,
            ExpiresAt = _tokenService.GetRefreshTokenExpirationUtc(),
        };

        await _sessionRepository.AddAsync(session, cancellationToken);

        var rawRefreshToken = _tokenService.GenerateRefreshToken();

        var refreshToken = new Domain.Entities.Identity.RefreshToken
        {
            SessionId = session.Id,
            UserId = user.Id,
            TenantId = request.TenantId,
            TokenHash = _tokenHashingService.Hash(rawRefreshToken),
            IssuedAt = now,
            ExpiresAt = _tokenService.GetRefreshTokenExpirationUtc()
        };

        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        user.LastLoginAt = now;
        credential.LastUsedAt = now;

        var accessTokenExpiresAt = _tokenService.GetAccessTokenExpirationUtc();

        var payload = new JwtPayloadData
        {
            UserId = user.Id,
            Email = user.PrimaryEmail,
            SessionId = session.Id,
            TenantId = request.TenantId,
            UserName = user.UserName
        };

        var accessToken = _tokenService.GenerateAccessToken(payload);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new LoginDto
        {
            AccessToken = accessToken,
            RefreshToken = rawRefreshToken,
            AccessTokenExpiresAt = accessTokenExpiresAt,
            UserId = user.Id,
            Email = user.PrimaryEmail,
            UserName = user.UserName,
            DisplayName = user.DisplayName
        };
    }
}
