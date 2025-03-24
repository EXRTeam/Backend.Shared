using FluentValidation;
using MediatR;
using Shared.Domain.Results;

namespace Shared.Application.Pagination;

internal class PaginationQueryValidationPipeline<TRequest, TResponse>(
    IValidator<PaginationOptions> validator)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IPaginableQuery<TResponse>
    where TResponse : Result {
    public Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next,
        CancellationToken token) {
        var validationResult = validator.Validate(new PaginationOptions(request.Page, request.Size));

        return validationResult.IsValid
            ? next()
            : Task.FromResult(validationResult.ToResult<TResponse>());
    }
}
