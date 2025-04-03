using Shared.Domain.Entities;
using Shared.Domain.Repositories;
using System.Linq.Expressions;

namespace Shared.Domain.Extensions;

public static class RepositoryExtensions {
    public static Task<TEntity?> GetMinimalEntity<TEntity>(
        this IAggregateRootRepository<TEntity> repository,
        Guid id,
        CancellationToken token)
        where TEntity : class, IAggregateRoot
        => repository.GetEntity(
            id,
            x => new { x.Id },
            token);

    public static Task<TEntity?> GetMinimalEntity<TEntity>(
         this IRepository<TEntity> repository,
         Expression<Func<TEntity, bool>> filter,
         CancellationToken token)
         where TEntity : class, IAggregateRoot
         => repository.GetEntity(
             filter,
             x => new { x.Id },
             token);
}
