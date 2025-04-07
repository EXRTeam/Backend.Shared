using Microsoft.EntityFrameworkCore;
using Shared.Domain.Repositories;
using Shared.Infrastructure.Utils;
using System.Linq.Expressions;

namespace Shared.Infrastructure.Repositories;

public abstract class BaseRepository<TEntity>(DbContext context) : IRepository<TEntity> where TEntity: class {
    protected DbSet<TEntity> Entities => context.Set<TEntity>();
    protected IQueryable<TEntity> EntitiesNoTracking => Entities.AsNoTracking();

    public Task<TEntity?> GetEntity<TLoadData>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TLoadData>> dataForLoad,
        CancellationToken token = default) 
        => GetEntity(filter, dataForLoad, ignoreGlobalFilters: false, token);


    public Task<TEntity> GetRequiredEntity<TLoadData>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TLoadData>> dataForLoad,
        CancellationToken token = default)
        => GetEntity(filter, dataForLoad, token)!;

    protected async Task<TEntity?> GetEntity<TLoadData>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TLoadData>> dataForLoad,
        bool ignoreGlobalFilters,
        CancellationToken token = default) {
        var query = ignoreGlobalFilters 
            ? EntitiesNoTracking.IgnoreQueryFilters() 
            : EntitiesNoTracking;

        var queryResult = await query
            .Where(filter)
            .Select(dataForLoad)
            .FirstOrDefaultAsync(token);

        if (queryResult == null) return null;

        var result = DomainEntityMapper.Map<TLoadData, TEntity>(queryResult);

        Entities.Attach(result);

        return result;
    }

    public async Task<List<TEntity>> GetEntities<TLoadData>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TLoadData>> dataForLoad,
        CancellationToken token = default) {
        var queryResult = await EntitiesNoTracking
            .Where(filter)
            .Select(dataForLoad)
            .ToListAsync(token);

        var list = new List<TEntity>(queryResult.Count);

        for (int index = 0; index < queryResult.Count; index++) {
            var entity = DomainEntityMapper.Map<TLoadData, TEntity>(queryResult[index]);
            list[index] = entity;
            Entities.Attach(entity);
        }

        return list;
    }

    public Task<bool> Any(Expression<Func<TEntity, bool>> filter, CancellationToken token = default)
        => EntitiesNoTracking
            .Where(filter)
            .AnyAsync(token);

    public ValueTask Insert(TEntity entity, CancellationToken token = default) {
        Entities.Add(entity);
        return ValueTask.CompletedTask;
    }

    public ValueTask InsertMany(IEnumerable<TEntity> entities, CancellationToken token = default) {
        Entities.AddRange(entities);
        return ValueTask.CompletedTask;
    }

    public ValueTask Delete(TEntity entity, CancellationToken token = default) {
        Entities.Remove(entity);
        return ValueTask.CompletedTask;
    }

    public ValueTask DeleteMany(IEnumerable<TEntity> entities, CancellationToken token = default) {
        Entities.RemoveRange(entities);
        return ValueTask.CompletedTask;
    }
}
