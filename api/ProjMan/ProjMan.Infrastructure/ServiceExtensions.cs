using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProjMan.Infrastructure;

public static class ServiceExtensions
{
    public static void ConfigureInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddTransient<IDbConnection>(db => new NpgsqlConnection(connectionString));

        services.AddDbContext<ProjManDbContext>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
        services.Configure<RefreshTokenSettings>(configuration.GetSection(nameof(RefreshTokenSettings)));
        services.Configure<PasswordHasherSettings>(configuration.GetSection(nameof(PasswordHasherSettings)));
        services.AddScoped<ITokenGenerator, TokenGenerator>();
        services.AddScoped<ICurrentUser, CurrentUser>();

        services.AddTransient<IAuthRepository, AuthRepository>();
        services.AddTransient<IProjectRepository, ProjectRepository>();
    }
}
