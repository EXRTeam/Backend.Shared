namespace Shared.Domain.Entities;

public abstract class Entity : IEntity {
    private readonly Queue<IDomainEvent> events = [];

    public IEnumerable<IDomainEvent> GetDomainEvents() {
        while (events.Count > 0) {
            yield return events.Dequeue();
        }
    }

    protected void PushEvent(IDomainEvent @event) => events.Enqueue(@event);
}
