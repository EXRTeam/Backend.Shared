using MediatR;
using Shared.Domain.Entities;

namespace Shared.Application;

public interface IDomainEventHandler<T> : INotificationHandler<T> where T : class, IDomainEvent;
