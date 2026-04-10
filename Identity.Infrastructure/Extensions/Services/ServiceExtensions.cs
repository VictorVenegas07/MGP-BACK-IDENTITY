using Microsoft.Extensions.DependencyInjection;
using Identity.Domain.Ports;
using Identity.Domain.Ports.Repositories;
using Identity.Domain.Ports.Security;
using Identity.Infrastructure.Adapters.Repositories;
using Identity.Infrastructure.Adapters.Security;
using Identity.Infrastructure.Config.Seed;
using Microsoft.Extensions.Configuration;

namespace Identity.Infrastructure.Extensions.Services;

public static class ServiceExtensions 
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add your domain services here
        // Example: svc.AddScoped<IMyDomainService, MyDomainService>();

        services.Configure<SmtpEmailSettings>(
            configuration.GetSection(SmtpEmailSettings.SectionName));
        services.Configure<EmailVerificationSettings>(
            configuration.GetSection(EmailVerificationSettings.SectionName));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITenantMembershipRepository, TenantMembershipRepository>();
        services.AddScoped<ICredentialRepository, CredentialRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IVerificationTokenRepository, VerificationTokenRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ITokenHashingService, TokenHashingService>();
        services.AddScoped<IEmailVerificationService, EmailVerificationService>();

        services.AddScoped<IDataSeeder, SuperAdminSeeder>();
        return services;
    }
}
    
