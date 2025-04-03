using MediatR;
using Shared.Domain.Results;

namespace Shared.Application.Mediator;

public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result> 
    where TCommand : class, ICommand;

public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : class, ICommand<TResponse>;