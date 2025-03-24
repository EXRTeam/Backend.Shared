using MediatR;
using Shared.Domain.Results;

namespace Shared.Application.Mediator;

public interface ICommandHandler<TRequest> : IRequestHandler<TRequest, Result> 
    where TRequest : class, ICommand;

public interface ICommandHandler<TRequest, TResponse> : IRequestHandler<TRequest, Result<TResponse>>
    where TRequest : class, ICommand<TResponse>;