using Microsoft.EntityFrameworkCore;
using Shared.Domain.Entities;
using Shared.Domain.Repositories;
using Shared.Infrastructure.Utils;
using System.Linq.Expressions;

namespace Shared.Infrastructure.Repositories;

public abstract class BaseSqlRepository<TEntity>(DbContext context) : IRepository<TEntity>
    where TEntity : class, IAggregateRoot {
    protected DbSet<TEntity> Entities => context.Set<TEntity>();
    protected IQueryable<TEntity> EntitiesNoTracking => Entities.AsNoTracking();

    public Task<TEntity?> GetEntity<TDataLoadDefinition>(
        Guid id, 
        Expression<Func<TEntity, TDataLoadDefinition>> dataForLoad, 
        CancellationToken token = default)
        => GetEntity(x => x.Id == id, dataForLoad, token);

    public async Task<TEntity?> GetEntity<TDto>(
        Expression<Func<TEntity, bool>> filter, 
        Expression<Func<TEntity, TDto>> dataForLoad, 
        CancellationToken token = default) {
        var queryResult = await EntitiesNoTracking
            .AsNoTracking()
            .Where(filter)
            .Select(dataForLoad)
            .FirstOrDefaultAsync(token);

        if (queryResult == null) return null;

        var result = DomainEntityMapper.MapAggregateRoot<TDto, TEntity>(queryResult);

        Entities.Attach(result);

        return result;
    }

    public async Task<TEntity?> GetEntity<TDto>(
        Guid id, 
        Expression<Func<TEntity, bool>> additiveFilter, 
        Expression<Func<TEntity, TDto>> dataForLoad, 
        CancellationToken token = default) {
        var queryResult = await EntitiesNoTracking
            .Where(x => x.Id == id)
            .Where(additiveFilter)
            .Select(dataForLoad)
            .FirstOrDefaultAsync(token);

        if (queryResult == null) return null;

        var result = DomainEntityMapper.MapAggregateRoot<TDto, TEntity>(queryResult);

        Entities.Attach(result);

        return result;
    }

    public async Task<List<TEntity>> GetEntities<TDto>(
        Expression<Func<TEntity, bool>> filter, 
        Expression<Func<TEntity, TDto>> dataForLoad, 
        CancellationToken token = default) {
        var queryResult = await EntitiesNoTracking
            .Where(filter)
            .Select(dataForLoad)
            .ToListAsync(token);

        var list = new List<TEntity>(queryResult.Count);

        for (int index = 0; index < queryResult.Count; index++) {
            var entity = DomainEntityMapper.MapAggregateRoot<TDto, TEntity>(queryResult[index]);
            list[index] = entity;
            Entities.Attach(entity);
        }

        return list;
    }

    public Task<bool> Any(Expression<Func<TEntity, bool>> filter, CancellationToken token = default)
        => EntitiesNoTracking
            .Where(filter)
            .AnyAsync(token);

    public virtual ValueTask Insert(TEntity entity, CancellationToken token = default) {
        Entities.Entry(entity).State = EntityState.Added;
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask InsertMany(IEnumerable<TEntity> entities, CancellationToken token = default) {
        Entities.AddRange(entities);
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask Delete(TEntity entity, CancellationToken token) {
        Entities.Entry(entity).State = EntityState.Deleted;
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask DeleteMany(IEnumerable<TEntity> entities, CancellationToken token = default) {
        Entities.RemoveRange(entities);
        return ValueTask.CompletedTask;
    }
}
