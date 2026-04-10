using Identity.Domain.Entities.Identity;
using Identity.Domain.Enums;
using Identity.Domain.Ports.Security;
using Identity.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Identity.Infrastructure.Config.Seed;

public class SuperAdminSeeder : IDataSeeder
{
    private readonly PersistenceContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly SuperAdminSeedSettings _settings;
    private readonly ILogger<SuperAdminSeeder> _logger;

    public SuperAdminSeeder(
        PersistenceContext context,
        IPasswordHasher passwordHasher,
        IOptions<SuperAdminSeedSettings> settings,
        ILogger<SuperAdminSeeder> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!_settings.Enabled)
        {
            _logger.LogInformation("Super admin seed is disabled.");
            return;
        }

        if (string.IsNullOrWhiteSpace(_settings.Email))
            throw new InvalidOperationException("SuperAdminSeed:Email is required.");

        if (string.IsNullOrWhiteSpace(_settings.UserName))
            throw new InvalidOperationException("SuperAdminSeed:UserName is required.");

        if (string.IsNullOrWhiteSpace(_settings.Password))
            throw new InvalidOperationException("SuperAdminSeed:Password is required.");

        var normalizedEmail = _settings.Email.Trim().ToLowerInvariant();
        var normalizedUserName = _settings.UserName.Trim().ToLowerInvariant();
        var displayName = string.IsNullOrWhiteSpace(_settings.DisplayName)
            ? "Super Administrator"
            : _settings.DisplayName.Trim();

        var existingUser = await _context.Users
            .FirstOrDefaultAsync(x => x.PrimaryEmail == normalizedEmail && !x.IsDeleted, cancellationToken);

        if (existingUser is not null)
        {
            existingUser.IsPlatformAdmin = true;
            existingUser.Status = UserStatus.Active;
            existingUser.EmailVerified = true;

            var existingCredential = await _context.Credentials
                .FirstOrDefaultAsync(x =>
                    x.UserId == existingUser.Id &&
                    x.Type == CredentialType.Password &&
                    x.Status == CredentialStatus.Active &&
                    !x.IsDeleted,
                    cancellationToken);

            if (existingCredential is null)
            {
                existingCredential = new Credential
                {
                    UserId = existingUser.Id,
                    Type = CredentialType.Password,
                    Status = CredentialStatus.Active,
                    SecretHash = _passwordHasher.Hash(_settings.Password),
                    Algorithm = "BCrypt"
                };

                await _context.Credentials.AddAsync(existingCredential, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Super admin already exists and was normalized: {Email}", normalizedEmail);
            return;
        }

       var userId = Guid.NewGuid();

var user = new User
{
    Id = userId,
    PrimaryEmail = normalizedEmail,
    UserName = normalizedUserName,
    DisplayName = displayName,
    EmailVerified = true,
    IsPlatformAdmin = true,
    Status = UserStatus.Active,
};

var credential = new Credential
{
    UserId = userId,
    Type = CredentialType.Password,
    Status = CredentialStatus.Active,
    SecretHash = _passwordHasher.Hash(_settings.Password),
    Algorithm = "BCrypt"
};


        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        await _context.Credentials.AddAsync(credential, cancellationToken);
        

        _logger.LogInformation("Super admin created successfully: {Email}", normalizedEmail);
    }
}