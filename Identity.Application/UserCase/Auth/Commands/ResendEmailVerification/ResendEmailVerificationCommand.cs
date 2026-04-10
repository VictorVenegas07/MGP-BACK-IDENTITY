using Identity.Application.UseCase.Auth.DTOs;
using Identity.Domain.Common.Exceptions;
using Identity.Domain.Enums;
using Identity.Domain.Ports.Repositories;
using Identity.Domain.Ports.Security;
using MediatR;

namespace Identity.Application.UseCase.Auth.Commands.ResendEmailVerification;

public class ResendEmailVerificationCommand : IRequest<ResendEmailVerificationDto>
{
    public string Email { get; set; } = null!;
}

public class ResendEmailVerificationCommandHandler : IRequestHandler<ResendEmailVerificationCommand, ResendEmailVerificationDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailVerificationService _emailVerificationService;

    public ResendEmailVerificationCommandHandler(
        IUserRepository userRepository,
        IEmailVerificationService emailVerificationService)
    {
        _userRepository = userRepository;
        _emailVerificationService = emailVerificationService;
    }

    public async Task<ResendEmailVerificationDto> Handle(ResendEmailVerificationCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null)
            throw new NotFoundException("The user associated with the email address was not found.");

        if (user.Status != UserStatus.Active && user.Status != UserStatus.PendingVerification)
            throw new ForbiddenException("The user is not allowed to request email verification.");

        if (user.EmailVerified)
            throw new ConflictException("The email address is already verified.");

        var expiresAt = await _emailVerificationService.SendVerificationAsync(user, cancellationToken);

        return new ResendEmailVerificationDto
        {
            Email = user.PrimaryEmail,
            VerificationTokenExpiresAt = expiresAt,
            Message = "A new email verification message was sent."
        };
    }
}
