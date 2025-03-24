using MediatR;
using Shared.Domain.Results;

namespace Shared.Application.Mediator;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
