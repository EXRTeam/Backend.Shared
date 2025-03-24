namespace Shared.Domain.Entities;

public interface IEntity {
    public IEnumerable<IDomainEvent> GetDomainEvents();
}