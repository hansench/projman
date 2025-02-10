using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjMan.Application.Behavior;
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
            cfg.AddOpenBehavior(typeof(CachingBehavior<,>));
        });
    }
}
