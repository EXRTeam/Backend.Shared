using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using Shared.Domain.Entities;
using Shared.Infrastructure.OutboxMessages;

namespace Shared.Infrastructure.Interceptors;

internal sealed class ConvertDomainEventsToOutboxInterceptor : SaveChangesInterceptor {
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, 
        InterceptionResult<int> result, 
        CancellationToken token) {
        if (eventData.Context == null) {
            return base.SavingChangesAsync(eventData, result, token);
        }

        var messages = eventData.Context.ChangeTracker
            .Entries<IEntity>()
            .SelectMany(entry => entry.Entity.GetDomainEvents())
            .Select(domainEvent => new OutboxMessage {
                Id = Guid.CreateVersion7(),
                Type = domainEvent.GetType().Name,
                Content = JsonConvert.SerializeObject(
                    domainEvent,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }),
                OccuredOnUtc = DateTime.UtcNow,
            })
            .ToArray();

        eventData.Context.Set<OutboxMessage>().AddRange(messages);

        return base.SavingChangesAsync(eventData, result, token);
    }
}