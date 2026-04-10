using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using Identity.Domain.Entities.Base;
using Identity.Domain.Entities.Identity;
using Identity.Infrastructure.Extensions.Persistence;

namespace Identity.Infrastructure.Contexts;

public class PersistenceContext : DbContext
{
    private readonly IConfiguration _config;

    public PersistenceContext(DbContextOptions<PersistenceContext> options, IConfiguration config) : base(options)
    {
        _config = config;
    }


    public DbSet<User> Users => Set<User>();
    public DbSet<Credential> Credentials => Set<Credential>();
    public DbSet<ExternalIdentity> ExternalIdentities => Set<ExternalIdentity>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<VerificationToken> VerificationTokens => Set<VerificationToken>();
    public DbSet<TenantMembership> TenantMemberships => Set<TenantMembership>();

    public async Task CommitAsync()
    {
        await SaveChangesAsync().ConfigureAwait(false);

    }

    protected override void OnModelCreating(ModelBuilder? modelBuilder)
    {
        if (modelBuilder == null)
        {
            return;
        }

        modelBuilder.HasDefaultSchema(_config.GetValue<string>("SchemaName"));

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var t = entityType.ClrType;
            if (!typeof(DomainEntity).IsAssignableFrom(t)) continue;
            modelBuilder.Entity(entityType.Name).Property<DateTime>("CreatedAt").HasDefaultValueSql("GETUTCDATE()");
            modelBuilder.Entity(entityType.Name).Property<DateTime>("UpdatedAt").HasDefaultValueSql("GETUTCDATE()");
            modelBuilder.Entity(entityType.Name).Property<DateTime?>("DeletedOn").HasDefaultValue(null);
            modelBuilder.Entity(entityType.Name).Property<bool>("IsDeleted").HasDefaultValue(false);
        }
        modelBuilder.AppendGlobalQueryFilter<ISoftDelete>(s => s.DeletedOn == null);
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public static string Unaccent(string value) => value;
}
