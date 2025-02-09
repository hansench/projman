using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using ProjMan.Application.Interfaces;
using ProjMan.Application.Security;
using ProjMan.Infrastructure.Database;
using ProjMan.Infrastructure.Database.Migrations;
using ProjMan.Infrastructure.Repositories;
using System.Data;

namespace ProjMan.Infrastructure;

public static class ServiceExtensions
{
    public static void ConfigureInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddTransient<IDbConnection>(db => new NpgsqlConnection(connectionString));

        services.AddDbContext<ProjManDbContext>();
        // services.AddScoped<IUnitOfWork, UnitOfWork>();
        // services.AddScoped<ITestTakerRepository, TestTakerRepository>();

        services.AddTransient<IAuthRepository, AuthRepository>();
    }


    public static async Task<WebApplication> MigrateDatabase(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            using (var dbContext = scope.ServiceProvider.GetRequiredService<ProjManDbContext>())
            {
                try
                {
                    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
                    await DataSeeder.SeedTestUser(dbContext, hasher);
                }
                catch (Exception)
                {
                    //Log errors or do anything you think it's needed
                    throw;
                }
            }
        }
        return app;
    }
}
