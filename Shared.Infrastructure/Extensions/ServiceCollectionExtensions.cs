using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Shared.Application;
using Shared.Domain.Entities;
using Shared.Domain.Repositories;
using Shared.Infrastructure.Interceptors;
using Shared.Infrastructure.Options;
using Shared.Infrastructure.OutboxMessages;
using System.Reflection;

namespace Shared.Infrastructure.Extensions;

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddDomainEventsProcessing(this IServiceCollection services) 
        => services.AddSingleton(new ConvertDomainEventsToOutboxInterceptor());

    public static IServiceCollection AddRabbitMq(
        this IServiceCollection services,
        Action<MessageBrokerOptions> configurator,
        Assembly? consumersAssembly = null) {
        var brokerOptions = new MessageBrokerOptions();
        configurator(brokerOptions);

        services.AddMassTransit(options => {
            options.SetKebabCaseEndpointNameFormatter();
            
            if (consumersAssembly != null) {
                options.AddConsumers(consumersAssembly);
            }

            options.UsingRabbitMq((context, configurator) => {
                configurator.Host(new Uri(brokerOptions.Host), hostConfigurator => {
                    hostConfigurator.Username(brokerOptions.Username);
                    hostConfigurator.Password(brokerOptions.Password);
                });

                configurator.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    public static IServiceCollectionQuartzConfigurator AddDomainEventsProcessingJob(
        this IServiceCollectionQuartzConfigurator quartzServices, 
        Action<ITriggerConfigurator> configureTrigger) 
        => quartzServices
                .AddJob<DomainEventsProcessingJob>(
                    DomainEventsProcessingJob.Key,
                    jobOptions => jobOptions.DisallowConcurrentExecution())
                .AddTrigger(triggerOptions => {
                    configureTrigger(triggerOptions);

                    triggerOptions
                        .ForJob(DomainEventsProcessingJob.Key);
                });

    public static IServiceCollection AddApplicationContext<TContext>(
        this IServiceCollection services, Action<DbContextOptionsBuilder> optionsConfigurator) 
        where TContext : ApplicationDbContextBase {
        services.AddDbContext<TContext>(
            optionsAction: (services, options) => {
                optionsConfigurator(options);
                var interceptor = services.GetService<ConvertDomainEventsToOutboxInterceptor>();

                if (interceptor != null) {
                    options.AddInterceptors(interceptor);
                }
            }, 
            contextLifetime: ServiceLifetime.Scoped,
            optionsLifetime: ServiceLifetime.Singleton);

        services.AddScoped<ApplicationDbContextBase>(x => x.GetRequiredService<TContext>());
        services.AddScoped<IUnitOfWork>(x => x.GetRequiredService<TContext>());

        return services;
    }

    public static IServiceCollection AddRepository<TContract, TRepository, TEntity>(
        this IServiceCollection services)
        where TEntity : class, IAggregateRoot
        where TContract : class
        where TRepository : class, IRepository<TEntity>, TContract
        => services
            .AddScoped<TContract, TRepository>()
            .AddScoped<IRepository<TEntity>, TRepository>();
}
