using MediatR;
using Shared.Domain.Results;

namespace Shared.Application.Mediator;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>> 
    where TQuery: class, IQuery<TResponse>;