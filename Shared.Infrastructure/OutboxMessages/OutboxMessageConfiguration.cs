using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shared.Infrastructure.OutboxMessages;

internal class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage> {
    public void Configure(EntityTypeBuilder<OutboxMessage> builder) {
        builder.ToTable("outbox_messages");

        builder.HasKey(x => x.Id);
    }
}
