using ProjMan;
using ProjMan.Application;
using ProjMan.Application.Exceptions.Handlers;
using ProjMan.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions();
builder.Services.AddHttpContextAccessor();

builder.Services.ConfigureInfrastructure(builder.Configuration);
builder.Services.ConfigureeApplication(builder.Configuration);
builder.Services.ConfigureJwt();
builder.Services.RegisterCors(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddExceptionHandler<UnauthorizedExceptionHandler>();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddProblemDetails();

await using (var app = builder.Build())
{
    await app.MigrateDatabase();

    app.Use(async (context, next) =>
    {
        if (context.Request.Path.Value == "/favicon.ico")
        {
            // Favicon request, return 404
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        // No favicon, call next middleware
        await next.Invoke();
    });

    app.UseHttpsRedirection();
    app.UseExceptionHandler();
    app.UseStatusCodePages();

    app.UseCors("Default");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers()
        .RequireAuthorization();

    await app.RunAsync();
}
