using Microsoft.EntityFrameworkCore;
using Shared.Domain.Entities;
using Shared.Infrastructure.Utils;
using System.Linq.Expressions;

namespace Shared.Infrastructure.Repositories;

public abstract class BaseAggregateRootRepository<TEntity>(DbContext context) 
    : BaseRepository<TEntity>(context)
    where TEntity : class, IAggregateRoot {
    protected Task<TEntity?> GetEntity<TDataLoadDefinition>(
        Guid id,
        Expression<Func<TEntity, TDataLoadDefinition>> dataForLoad,
        CancellationToken token = default)
        => GetEntity(x => x.Id == id, dataForLoad, token);

    protected async Task<TEntity?> GetEntity<TDto>(
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

        var result = DomainEntityMapper.Map<TDto, TEntity>(queryResult);

        Entities.Attach(result);

        return result;
    }
}
