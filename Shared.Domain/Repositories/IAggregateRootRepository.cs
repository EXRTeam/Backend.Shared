namespace Shared.Domain.Repositories;

public interface IRepository<TEntity> where TEntity : class {
    public ValueTask Insert(
        TEntity entity,
        CancellationToken token = default);

    public ValueTask Delete(
        TEntity entity,
        CancellationToken token = default);

    public ValueTask InsertMany(
        IEnumerable<TEntity> entities,
        CancellationToken token = default);

    public ValueTask DeleteMany(
        IEnumerable<TEntity> entities,
        CancellationToken token = default);
}
