using Identity.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure.Extensions.Persistence;

public static class ContextExtensions
{
    public static IServiceCollection AddContextDatabase(this IServiceCollection svc, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection")
            ?? Environment.GetEnvironmentVariable("CONNECTION_STRING");

        svc.AddDbContext<PersistenceContext>(opt =>
        {
            opt.UseSqlServer(connectionString, sqlopts =>
            {
                sqlopts.MigrationsHistoryTable("_MigrationHistory", config.GetValue<string>("SchemaName"));
                sqlopts.MigrationsAssembly(ApiConstants.Infrastructure);
            });
        });

        return svc;
    }
}
