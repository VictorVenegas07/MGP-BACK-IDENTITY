using Identity.Domain.Common.Exceptions;
using Identity.Domain.Entities.Identity;
using Identity.Domain.Enums;
using Identity.Domain.Ports.Security;
using Identity.Infrastructure.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/identity")]
public class IdentityController : ControllerBase
{
    private readonly PersistenceContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public IdentityController(PersistenceContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
    {
        var users = await _context.Users
            .AsNoTracking()
            .OrderByDescending(x => x.IsPlatformAdmin)
            .ThenBy(x => x.DisplayName ?? x.UserName ?? x.PrimaryEmail)
            .Select(x => new IdentityUserDto
            {
                Id = x.Id,
                Email = x.PrimaryEmail,
                UserName = x.UserName,
                DisplayName = x.DisplayName,
                AvatarUrl = x.AvatarUrl,
                Status = x.Status.ToString(),
                EmailVerified = x.EmailVerified,
                IsPlatformAdmin = x.IsPlatformAdmin,
                LastLoginAt = x.LastLoginAt,
            })
            .ToListAsync(cancellationToken);

        return Ok(users);
    }

    [HttpGet("users/{id:guid}")]
    public async Task<IActionResult> GetUserById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new IdentityUserDto
            {
                Id = x.Id,
                Email = x.PrimaryEmail,
                UserName = x.UserName,
                DisplayName = x.DisplayName,
                AvatarUrl = x.AvatarUrl,
                Status = x.Status.ToString(),
                EmailVerified = x.EmailVerified,
                IsPlatformAdmin = x.IsPlatformAdmin,
                LastLoginAt = x.LastLoginAt,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
            throw new NotFoundException("User not found.");

        return Ok(user);
    }

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] CreateIdentityUserRequest request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var normalizedUserName = string.IsNullOrWhiteSpace(request.UserName)
            ? null
            : request.UserName.Trim().ToLowerInvariant();

        if (await _context.Users.AnyAsync(x => x.PrimaryEmail == normalizedEmail, cancellationToken))
            throw new ConflictException("The email is already registered.");

        if (!string.IsNullOrWhiteSpace(normalizedUserName) &&
            await _context.Users.AnyAsync(x => x.UserName == normalizedUserName, cancellationToken))
            throw new ConflictException("The username is already registered.");

        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            PrimaryEmail = normalizedEmail,
            UserName = normalizedUserName,
            DisplayName = string.IsNullOrWhiteSpace(request.DisplayName) ? null : request.DisplayName.Trim(),
            AvatarUrl = string.IsNullOrWhiteSpace(request.AvatarUrl) ? null : request.AvatarUrl.Trim(),
            EmailVerified = request.EmailVerified,
            Status = ParseStatus(request.Status),
            IsPlatformAdmin = request.IsPlatformAdmin,
        };

        var credential = new Credential
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = CredentialType.Password,
            Status = CredentialStatus.Active,
            SecretHash = _passwordHasher.Hash(request.Password),
            Algorithm = "Argon2id"
        };

        await _context.Users.AddAsync(user, cancellationToken);
        await _context.Credentials.AddAsync(credential, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ToDto(user));
    }

    [HttpPut("users/{id:guid}")]
    public async Task<IActionResult> UpdateUser([FromRoute] Guid id, [FromBody] UpdateIdentityUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (user is null)
            throw new NotFoundException("User not found.");

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var normalizedUserName = string.IsNullOrWhiteSpace(request.UserName)
            ? null
            : request.UserName.Trim().ToLowerInvariant();

        if (await _context.Users.AnyAsync(x => x.Id != id && x.PrimaryEmail == normalizedEmail, cancellationToken))
            throw new ConflictException("The email is already registered.");

        if (!string.IsNullOrWhiteSpace(normalizedUserName) &&
            await _context.Users.AnyAsync(x => x.Id != id && x.UserName == normalizedUserName, cancellationToken))
            throw new ConflictException("The username is already registered.");

        user.PrimaryEmail = normalizedEmail;
        user.UserName = normalizedUserName;
        user.DisplayName = string.IsNullOrWhiteSpace(request.DisplayName) ? null : request.DisplayName.Trim();
        user.AvatarUrl = string.IsNullOrWhiteSpace(request.AvatarUrl) ? null : request.AvatarUrl.Trim();
        user.EmailVerified = request.EmailVerified;
        user.Status = ParseStatus(request.Status);
        user.IsPlatformAdmin = request.IsPlatformAdmin;
        user.MarkAsUpdated();

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            var credential = await _context.Credentials
                .FirstOrDefaultAsync(
                    x => x.UserId == id && x.Type == CredentialType.Password && x.Status == CredentialStatus.Active,
                    cancellationToken);

            if (credential is null)
            {
                credential = new Credential
                {
                    Id = Guid.NewGuid(),
                    UserId = id,
                    Type = CredentialType.Password,
                    Status = CredentialStatus.Active,
                    Algorithm = "Argon2id",
                };

                await _context.Credentials.AddAsync(credential, cancellationToken);
            }

            credential.SecretHash = _passwordHasher.Hash(request.Password);
            credential.Algorithm = "Argon2id";
            credential.MarkAsUpdated();
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ToDto(user));
    }

    [HttpDelete("users/{id:guid}")]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (user is null)
            throw new NotFoundException("User not found.");

        user.SetDelete();
        user.MarkAsDeleted();
        user.MarkAsUpdated();

        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private static IdentityUserDto ToDto(User user) => new()
    {
        Id = user.Id,
        Email = user.PrimaryEmail,
        UserName = user.UserName,
        DisplayName = user.DisplayName,
        AvatarUrl = user.AvatarUrl,
        Status = user.Status.ToString(),
        EmailVerified = user.EmailVerified,
        IsPlatformAdmin = user.IsPlatformAdmin,
        LastLoginAt = user.LastLoginAt,
    };

    private static UserStatus ParseStatus(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return UserStatus.Active;

        return Enum.TryParse<UserStatus>(value, true, out var status)
            ? status
            : throw new ValidationException("Invalid user status.");
    }
}

public sealed class IdentityUserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string? UserName { get; set; }
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
    public string Status { get; set; } = null!;
    public bool EmailVerified { get; set; }
    public bool IsPlatformAdmin { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

public sealed class CreateIdentityUserRequest
{
    public string Email { get; set; } = null!;
    public string? UserName { get; set; }
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
    public string Password { get; set; } = null!;
    public string? Status { get; set; }
    public bool EmailVerified { get; set; }
    public bool IsPlatformAdmin { get; set; }
}

public sealed class UpdateIdentityUserRequest
{
    public string Email { get; set; } = null!;
    public string? UserName { get; set; }
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Password { get; set; }
    public string? Status { get; set; }
    public bool EmailVerified { get; set; }
    public bool IsPlatformAdmin { get; set; }
}
