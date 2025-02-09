using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ProjMan.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjMan.Application.Exceptions.Handlers;

public class ValidationExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ValidationExceptionHandler> _logger;

    public ValidationExceptionHandler(ILogger<ValidationExceptionHandler> logger)
    {
        _logger = logger;
    }


    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not CustomValidationException)
        {
            return false;
        }

        _logger.LogError(exception, exception.Message);

        var ex = exception as CustomValidationException;
        if (ex != null)
        {
            var response = new BaseResponse
            {
                Ok = false,
                Message = exception.Message + ": " + string.Join(", ", ex.ErrorMessages ?? new List<string>())
            };

            httpContext.Response.StatusCode = (int)ex.StatusCode;

            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        }

        return true;
    }
}
