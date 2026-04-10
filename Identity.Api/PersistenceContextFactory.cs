using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Identity.Infrastructure.Contexts;
using Identity.Infrastructure.Extensions;

namespace Identity.Api;
/// <summary>
/// Factory for creating the PersistenceContext.
/// </summary>
public class PersistenceContextFactory : IDesignTimeDbContextFactory<PersistenceContext>
{
    /// <summary>
    /// Creates a new instance of the PersistenceContext.
    /// </summary>
    public PersistenceContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json")
           .AddJsonFile("appsettings.Development.json", optional: true)
           .AddEnvironmentVariables()
           .Build();
        DotNetEnv.Env.Load();
        var connectionString = config.GetConnectionString("DefaultConnection")
            ?? Environment.GetEnvironmentVariable("CONNECTION_STRING");
        Console.WriteLine($"Connection string: {connectionString}");
        var optionsBuilder = new DbContextOptionsBuilder<PersistenceContext>();
        optionsBuilder.UseSqlServer(connectionString, sqlopts =>
        {
            sqlopts.MigrationsHistoryTable("_MigrationHistory", config.GetValue<string>("SchemaName"));
            sqlopts.MigrationsAssembly(ApiConstants.Infrastructure);
        });

        return new PersistenceContext(optionsBuilder.Options, config);
    }
}
