using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using Shared.Domain.Entities;

namespace Shared.Infrastructure.OutboxMessages;

internal sealed class DomainEventsProcessingJob(
    ApplicationDbContextBase dbContext, 
    IPublisher publisher,
    ILogger<DomainEventsProcessingJob> logger) 
    : IJob {
    public static readonly JobKey Key = new(nameof(DomainEventsProcessingJob));

    public async Task Execute(IJobExecutionContext context) {
        logger.LogInformation("Domain events processing has started...");

         var messages = await dbContext.Set<OutboxMessage>()
            .Where(x => x.ProcessedOnUtc == null)
            .OrderBy(x => x.OccuredOnUtc)
            .Take(15)
            .ToListAsync(context.CancellationToken);

        foreach (var message in messages) {
            var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                message.Content, 
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

            if (domainEvent == null) {
                logger.LogError("Couldn't deserialize the event: {eventJson}", message.Content);
                message.Error = $"Couldn't deserialize the event";
                continue;
            }

            try {
                await publisher.Publish(domainEvent, context.CancellationToken);

                message.ProcessedOnUtc = DateTime.UtcNow;
                message.Error = null;
            } catch (Exception exception) {
                message.Error = exception.Message;
            }
        }

        var processedMessagesCount = await dbContext.SaveChangesAsync(context.CancellationToken);

        logger.LogInformation("{MessagesCount} domain events were processed", processedMessagesCount);
    }
}
