using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Shared.Application;
using Shared.Infrastructure.OutboxMessages;
using System.Data;

namespace Shared.Infrastructure;

public abstract class ApplicationDbContextBase(DbContextOptions options) : DbContext(options), IUnitOfWork {
    public async Task<IDbTransaction> BeginTransaction(CancellationToken token) 
        => (await Database.BeginTransactionAsync(token)).GetDbTransaction();

    public Task<int> SaveChanges(CancellationToken token) => SaveChangesAsync(token);

    protected sealed override void OnModelCreating(ModelBuilder modelBuilder) {
        ConfigureModel(modelBuilder);
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
    }

    protected sealed override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected abstract void ConfigureModel(ModelBuilder modelBuilder);
}