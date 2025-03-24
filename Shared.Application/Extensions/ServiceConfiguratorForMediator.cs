using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.Mediator;
using Shared.Application.Pagination;

namespace Shared.Application;

public static class ServiceConfiguratorForMediator {
    public static IServiceCollection AddValidationPipeline(
        this IServiceCollection services)
        => services
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipeline<,>));

    public static IServiceCollection AddPaginationQueryValidationPipeline(
        this IServiceCollection services)
        => services
            .AddSingleton<IValidator<PaginationOptions>, PaginationOptionsValidator>()
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(PaginationQueryValidationPipeline<,>));
}