using Shared.Domain.Entities;
using System.Linq.Expressions;

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

    public Task<TEntity?> GetEntity<TDataLoadDefinition>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TDataLoadDefinition>> dataForLoad,
        CancellationToken token = default);

    public Task<List<TEntity>> GetEntities<TDataLoadDefinition>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TDataLoadDefinition>> dataForLoad,
        CancellationToken token = default);

    public Task<bool> Any(
        Expression<Func<TEntity, bool>> filter,
        CancellationToken token = default);
}

public interface IAggregateRootRepository<TEntity> : IRepository<TEntity> where TEntity : class, IAggregateRoot {
    public Task<TEntity?> GetEntity<TDataLoadDefinition>(
        Guid id,
        Expression<Func<TEntity, TDataLoadDefinition>> dataForLoad,
        CancellationToken token = default);

    public Task<TEntity?> GetEntity<TDataLoadDefinition>(
        Guid id,
        Expression<Func<TEntity, bool>> additiveFilter,
        Expression<Func<TEntity, TDataLoadDefinition>> dataForLoad,
        CancellationToken token = default);
}
