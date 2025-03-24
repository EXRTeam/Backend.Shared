﻿using MediatR;
using Shared.Domain.Results;

namespace Shared.Application.Mediator;

public interface ICommand : IRequest<Result>;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>;