using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Identity.Infrastructure.Extensions.Swagger;

public static class SwaggerExtension
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{Assembly.GetEntryAssembly()!.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);

            var httpContextAccessor = services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>();
            var host = httpContextAccessor.HttpContext?.Request.Host.Value ?? "localhost";
            var scheme = httpContextAccessor.HttpContext?.Request.Scheme ?? "http";
            var serverUrl = Environment.GetEnvironmentVariable("SWAGGER_HOST");

            // Establecer la URL base para las rutas
            options.AddServer(new OpenApiServer { Url = serverUrl });

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter JWT Bearer token *only*",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            options.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
              {
                  { securityScheme, new List<string>() { } }
              });
            options.ExampleFilters();

            options.SchemaFilter<IgnoreSwaggerPropertiesSchemaFilter>();
        });
        return services;
    }

    public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsProduction()) return app;
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.DefaultModelExpandDepth(2);
            options.DefaultModelsExpandDepth(-1);
            options.DisplayOperationId();
            options.DisplayRequestDuration();
            options.EnableDeepLinking();
            options.ShowExtensions();
            options.EnableValidator(null);
            options.DocExpansion(DocExpansion.None);
        });

        return app;
    }
}

