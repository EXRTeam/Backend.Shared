namespace Shared.AspNetCoreTools.Extensions;

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddMyProblemDetails(this IServiceCollection services) 
        => services.AddProblemDetails(options => 
            options.CustomizeProblemDetails = (context) => {
                context.ProblemDetails.Instance = context.HttpContext.Request.Path;
                context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
            });

    public static IServiceCollection AddMyExceptionHandler(this IServiceCollection services)
        => services.AddExceptionHandler<GlobalExceptionHandler>();
}
