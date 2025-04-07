using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Shared.Application;
using Shared.Infrastructure.OutboxMessages;
using System.Data;

namespace Shared.Infrastructure;

public abstract class ApplicationDbContextBase(DbContextOptions options) : DbContext(options), IUnitOfWork {
    public async Task<IDbTransaction> BeginTransaction(IsolationLevel isolationLevel, CancellationToken token) 
        => (await Database.BeginTransactionAsync(isolationLevel, token)).GetDbTransaction();

    public Task<int> SaveChanges(CancellationToken token) {
#if DEBUG
        var tracker = ChangeTracker.DebugView.LongView;
#endif
        return SaveChangesAsync(token);
    }

    protected static void ApplyOutboxMessageConfiguration(ModelBuilder builder)
        => builder
            .Entity<OutboxMessage>()
            .ToTable("outbox_messages")
            .HasKey(x => x.Id);
}
