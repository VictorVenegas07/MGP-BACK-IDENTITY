using Identity.Domain.Ports;
using Identity.Infrastructure.Adapters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Identity.Infrastructure.Extensions.Persistence;

/// <summary>
/// Aquí se registra el servicio de base de datos, en este caso el cliente
/// que se va a usar ya sea Mongo, SQL Server o Postgres.
/// </summary>
public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection svc, IConfiguration config)
    {
        svc.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        return svc;
    }
}
