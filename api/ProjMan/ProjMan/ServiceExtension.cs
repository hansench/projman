using ProjMan.Infrastructure.Database.Migrations;
using ProjMan.Infrastructure.Database;
using ProjMan.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ProjMan;

public static class ServiceExtension
{
    public static void RegisterCors(this IServiceCollection services, IConfiguration configuration, bool exposedTokenExpired = true)
    {
        services.AddCors(o => o.AddPolicy("Default", policyBuilder =>
        {
            policyBuilder.WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>()!)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("Content-Disposition");

            if (exposedTokenExpired)
            {
                policyBuilder.WithExposedHeaders("Token-Expired");
            }
        }));
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

    public static void ConfigureJwt(this IServiceCollection services)
    {
        var signingCredentials = services.BuildServiceProvider().GetService<IOptions<JwtSettings>>();

        if (signingCredentials == null)
        {
            throw new ArgumentNullException(nameof(signingCredentials));
        }

        services.Configure<JwtIssuerOptions>(
            options =>
            {
                options.Issuer = signingCredentials.Value.Issuer;
                options.Audience = signingCredentials.Value.Audience;
                options.SigningCredentials = signingCredentials.Value.SigningCredentials;
                options.ValidFor = TimeSpan.FromMinutes(signingCredentials.Value.ValidMinutes);
            });

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingCredentials?.Value?.SigningCredentials?.Key ?? throw new ArgumentNullException(nameof(signingCredentials)),
            ValidateIssuer = true,
            ValidIssuer = signingCredentials.Value.Issuer,
            ValidateAudience = true,
            ValidAudience = signingCredentials.Value.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(
                options =>
                {
                    options.TokenValidationParameters = tokenValidationParameters;
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context => GetToken(context),
                        OnAuthenticationFailed = context => TokenExpired(context),
                    };
                });
    }


    private static Task GetToken(MessageReceivedContext context)
    {
        var allowAnonymous = context.HttpContext.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>();

        if (allowAnonymous == null)
        {
            var authHeader = context.HttpContext.Request.Headers["Authorization"];
            if (authHeader.Count == 0)
            {
                throw new UnauthorizedAccessException("Invalid operation");
            }

            var token = authHeader[0] ?? string.Empty;
            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                context.Token = token.Replace("Bearer", "").Trim();
            }
            else
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }
        }

        return Task.CompletedTask;
    }


    private static Task TokenExpired(AuthenticationFailedContext context)
    {
        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
        {
            context.Response.Headers.Append("Token-Expired", "true");
        }

        return Task.CompletedTask;
    }
}
