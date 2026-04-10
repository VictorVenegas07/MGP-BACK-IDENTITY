using Identity.Infrastructure.Config.Seed;
using Identity.Infrastructure.Contexts;
using Identity.Infrastructure.Extensions;
using Identity.Application.UseCase.Auth.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
// Add services to the container.
DotNetEnv.Env.Load();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(config);
builder.Services.AddScoped<SessionRevocationService>();
builder.Services.Configure<SuperAdminSeedSettings>(
    builder.Configuration.GetSection(SuperAdminSeedSettings.SectionName));

    
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = services.GetRequiredService<PersistenceContext>();
        await context.Database.MigrateAsync();

        var seeder = services.GetRequiredService<IDataSeeder>();
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while applying migrations or seeding the database.");
        throw;
    }
}
app.UseInfrastructure(app.Environment);

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
