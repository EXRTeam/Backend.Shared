using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Shared.AspNetCoreTools;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler {
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken) {
        logger.LogError(exception, "Unhandled exception has been occured:\n{Message}", exception.Message);

        var statusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails {
            Title = "Internal server error",
            Instance = httpContext.Request.Path,
            Status = statusCode,
        };

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
