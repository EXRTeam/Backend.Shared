using FluentValidation;
using MediatR;
using Shared.Domain.Results;

namespace Shared.Application.Mediator;

internal class ValidationPipeline<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validator)
    : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : class, IRequest<TResponse>
    where TResponse : Result {
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken token) {
        if (!validator.Any()) {
            return await next();
        }

        var result = await validator.First().ValidateAsync(request, token);

        if (!result.IsValid) {
            return result.ToResult<TResponse>();
        }

        return await next();
    }
}
