using Identity.Infrastructure.Extensions.Cors;
using Identity.Infrastructure.Extensions.Feature;
using Identity.Infrastructure.Extensions.Jwt;
using Identity.Infrastructure.Extensions.Mapper;
using Identity.Infrastructure.Extensions.Mediator;
using Identity.Infrastructure.Extensions.Persistence;
using Identity.Infrastructure.Extensions.Swagger;
using Identity.Infrastructure.Extensions.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Identity.Infrastructure.Extensions.Services;

namespace Identity.Infrastructure.Extensions;

public static class Startup
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddSwagger()
            .AddDomainServices(config)
            .AddValidation()
            .AddMediator()
            .AddJwtAuthentication(config)
            .AddMapper()
            .AddContextDatabase(config)
            .AddCorsPolicy(config)
            .AddPersistence(config)
            .AddHttpContextAccessor()
            .AddFeature(config);
    }

    public static void UseInfrastructure(this IApplicationBuilder builder, IWebHostEnvironment env)
    {
        builder
            .UseSwagger(env)
            .UseCorsPolicy();
    }
}
