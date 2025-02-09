using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProjMan.Application.Security;
using ProjMan.Application.Validation;
using System.Reflection;

namespace ProjMan.Application;

public static class ServiceExtension
{
    public static void ConfigureeApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
        });

        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
        services.Configure<RefreshTokenSettings>(configuration.GetSection(nameof(RefreshTokenSettings)));
        services.Configure<PasswordHasherSettings>(configuration.GetSection(nameof(PasswordHasherSettings)));

        services.AddScoped<ITokenGenerator, TokenGenerator>();
        services.AddScoped<ICurrentUser, CurrentUser>();

        services.ConfigureJwt();
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
