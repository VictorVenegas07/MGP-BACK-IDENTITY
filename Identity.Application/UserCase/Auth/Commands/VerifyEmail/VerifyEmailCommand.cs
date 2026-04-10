using Identity.Application.UseCase.Auth.DTOs;
using Identity.Domain.Common.Exceptions;
using Identity.Domain.Enums;
using Identity.Domain.Ports;
using Identity.Domain.Ports.Repositories;
using Identity.Domain.Ports.Security;
using MediatR;

namespace Identity.Application.UseCase.Auth.Commands.VerifyEmail;

public class VerifyEmailCommand : IRequest<EmailVerificationResultDto>
{
    public string Token { get; set; } = null!;
}

public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, EmailVerificationResultDto>
{
    private readonly IVerificationTokenRepository _verificationTokenRepository;
    private readonly ITokenHashingService _tokenHashingService;
    private readonly IUnitOfWork _unitOfWork;

    public VerifyEmailCommandHandler(
        IVerificationTokenRepository verificationTokenRepository,
        ITokenHashingService tokenHashingService,
        IUnitOfWork unitOfWork)
    {
        _verificationTokenRepository = verificationTokenRepository;
        _tokenHashingService = tokenHashingService;
        _unitOfWork = unitOfWork;
    }

    public async Task<EmailVerificationResultDto> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = _tokenHashingService.Hash(request.Token.Trim());
        var verificationToken = await _verificationTokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);

        if (verificationToken is null || verificationToken.Type != VerificationTokenType.EmailVerification)
            throw new UnauthorizedException("The email verification token is invalid.");

        if (verificationToken.UsedAt.HasValue)
            throw new ConflictException("The email verification token has already been used.");

        var now = DateTime.UtcNow;

        if (verificationToken.ExpiresAt <= now)
            throw new ForbiddenException("The email verification token has expired.");

        var user = verificationToken.User;

        if (user.EmailVerified)
        {
            verificationToken.UsedAt = now;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new EmailVerificationResultDto
            {
                Success = true,
                AlreadyVerified = true,
                Message = "The email address is already verified."
            };
        }

        user.EmailVerified = true;
        user.Status = UserStatus.Active;
        verificationToken.UsedAt = now;

        await _verificationTokenRepository.InvalidateActiveTokensAsync(
            user.Id,
            VerificationTokenType.EmailVerification,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new EmailVerificationResultDto
        {
            Success = true,
            AlreadyVerified = false,
            Message = "The email address was verified successfully."
        };
    }
}
