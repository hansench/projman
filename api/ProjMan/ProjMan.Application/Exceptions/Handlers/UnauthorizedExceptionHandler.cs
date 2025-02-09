using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ProjMan.Application.Dtos;

namespace ProjMan.Application.Exceptions.Handlers;

public class UnauthorizedExceptionHandler : IExceptionHandler
{
    private readonly ILogger<UnauthorizedExceptionHandler> _logger;

    public UnauthorizedExceptionHandler(ILogger<UnauthorizedExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not UnauthorizedException unauthorizedException)
        {
            return false;
        }

        _logger.LogError(unauthorizedException,
            "Exception occurred: {Message}",
            unauthorizedException.Message);

        var response = new BaseResponse
        {
            Ok = false,
            Message = unauthorizedException.Message
        };

        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}
