using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Shared.Application;
using Shared.Domain.Entities;
using Shared.Domain.Repositories;
using Shared.Infrastructure.Interceptors;
using Shared.Infrastructure.OutboxMessages;

namespace Shared.Infrastructure.Extensions;

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddDomainEventHandling(this IServiceCollection services) 
        => services.AddSingleton(new ConvertDomainEventsToOutboxInterceptor());

    public static IServiceCollectionQuartzConfigurator AddDomainEventHandlingJob(
        this IServiceCollectionQuartzConfigurator quartzServices, Action<ITriggerConfigurator> configureTrigger) 
        => quartzServices
                .AddJob<ProcessOutboxMessagesJob>(
                    ProcessOutboxMessagesJob.Key,
                    jobOptions => jobOptions.DisallowConcurrentExecution())
                .AddTrigger(triggerOptions => {
                    configureTrigger(triggerOptions);

                    triggerOptions
                        .ForJob(ProcessOutboxMessagesJob.Key);
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
