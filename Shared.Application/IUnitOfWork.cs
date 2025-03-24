using System.Data;

namespace Shared.Application;

public interface IUnitOfWork {
    public Task<int> SaveChanges(CancellationToken token = default);
    public Task<IDbTransaction> BeginTransaction(CancellationToken token = default);
}
