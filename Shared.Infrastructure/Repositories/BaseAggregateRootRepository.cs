using Microsoft.EntityFrameworkCore;
using Shared.Domain.Entities;
using Shared.Domain.Repositories;
using Shared.Infrastructure.Utils;
using System.Linq.Expressions;

namespace Shared.Infrastructure.Repositories;

public abstract class BaseAggregateRootRepository<TEntity>(DbContext context) 
    : BaseRepository<TEntity>(context), IAggregateRootRepository<TEntity>
    where TEntity : class, IAggregateRoot {
    public Task<TEntity?> GetEntity<TDataLoadDefinition>(
        Guid id,
        Expression<Func<TEntity, TDataLoadDefinition>> dataForLoad,
        CancellationToken token = default)
        => GetEntity(x => x.Id == id, dataForLoad, token);

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
}
