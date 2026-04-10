using Identity.Domain.Entities.Identity;
using Identity.Domain.Enums;
using Identity.Domain.Ports;
using Identity.Domain.Ports.Repositories;
using Identity.Domain.Ports.Security;
using Identity.Domain.Common.Exceptions;
using MediatR;

namespace Identity.Application.UseCase.Auth.Commands.CreateUser;

public class CreateUserCommand : IRequest<CreateUserDto>
{
    public string Email { get; set; } = null!;
    public string? UserName { get; set; }
    public string? DisplayName { get; set; }
    public string Password { get; set; } = null!;
} 


public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ICredentialRepository _credentialRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailVerificationService _emailVerificationService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        ICredentialRepository credentialRepository,
        IPasswordHasher passwordHasher,
        IEmailVerificationService emailVerificationService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _credentialRepository = credentialRepository;
        _passwordHasher = passwordHasher;
        _emailVerificationService = emailVerificationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateUserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var normalizedUserName = string.IsNullOrWhiteSpace(request.UserName)
            ? null
            : request.UserName.Trim().ToLowerInvariant();

        if (await _userRepository.ExistsByEmailAsync(normalizedEmail, cancellationToken))
            throw new ConflictException("The email is already registered.");

        if (!string.IsNullOrWhiteSpace(normalizedUserName) &&
            await _userRepository.ExistsByUserNameAsync(normalizedUserName, cancellationToken))
            throw new ConflictException("The username is already registered.");

        var userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            PrimaryEmail = normalizedEmail,
            UserName = normalizedUserName,
            DisplayName = string.IsNullOrWhiteSpace(request.DisplayName) ? null : request.DisplayName.Trim(),
            EmailVerified = false,
            Status = UserStatus.PendingVerification
        };

        var passwordHash = _passwordHasher.Hash(request.Password);

        var credential = new Credential
        {
            UserId = userId,
            Type = CredentialType.Password,
            Status = CredentialStatus.Active,
            SecretHash = passwordHash,
            Algorithm = "Argon2id"
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _credentialRepository.AddAsync(credential, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var verificationTokenExpiresAt = await _emailVerificationService.SendVerificationAsync(user, cancellationToken);

        return new CreateUserDto
        {
            UserId = user.Id,
            Email = user.PrimaryEmail,
            UserName = user.UserName,
            DisplayName = user.DisplayName,
            EmailVerified = user.EmailVerified,
            VerificationTokenExpiresAt = verificationTokenExpiresAt
        };
    }
}
