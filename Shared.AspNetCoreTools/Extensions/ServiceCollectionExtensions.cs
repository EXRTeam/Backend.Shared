using MassTransit;
using Shared.AspNetCoreTools.Options;
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

            var messageBrokerOptions = messageBrokerOptionsSection.Get<MessageBrokerOptions>();

            options.UsingRabbitMq((context, configurator) => {
                configurator.Host(new Uri(messageBrokerOptions!.Host), hostConfigurator => {
                    hostConfigurator.Username(messageBrokerOptions.Username);
                    hostConfigurator.Password(messageBrokerOptions.Password);
                });

                configurator.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
