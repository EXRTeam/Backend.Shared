using MassTransit;
using System.Reflection;

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

    public static IServiceCollection AddMassTransitWithRabbitMq(
        this IServiceCollection services, 
        Assembly consumersAssembly,
        IConfiguration messageBrokerOptionsSection) {
        services.AddMassTransit(options => {
            options.SetKebabCaseEndpointNameFormatter();

            options.AddConsumers(consumersAssembly);

            options.UsingRabbitMq((context, configurator) => {
                configurator.Host(new Uri(messageBrokerOptionsSection["Host"]!), hostConfigurator => {
                    hostConfigurator.Username(messageBrokerOptionsSection["Username"]!);
                    hostConfigurator.Password(messageBrokerOptionsSection["Password"]!);
                });

                configurator.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
