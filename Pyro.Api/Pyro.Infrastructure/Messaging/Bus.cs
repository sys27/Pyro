// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pyro.Domain.Shared.Messaging;
using Pyro.Infrastructure.DataAccess;

namespace Pyro.Infrastructure.Messaging;

internal class Bus : IBus
{
    private readonly PyroDbContext dbContext;
    private readonly JsonSerializerOptions jsonSerializerOptions;
    private readonly TimeProvider timeProvider;
    private readonly ILogger<Bus> logger;

    public Bus(
        PyroDbContext dbContext,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        TimeProvider timeProvider,
        ILogger<Bus> logger)
    {
        this.dbContext = dbContext;
        this.jsonSerializerOptions = jsonSerializerOptions.Value;
        this.timeProvider = timeProvider;
        this.logger = logger;
    }

    public async Task Publish<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent
    {
        var outboxMessage = new OutboxMessage
        {
            Id = integrationEvent.MessageId,
            Type = typeof(TEvent).AssemblyQualifiedName ??
                   throw new InvalidOperationException("Failed to get type name"),
            Message = JsonSerializer.Serialize(integrationEvent, jsonSerializerOptions),
            CreatedAt = timeProvider.GetUtcNow().ToUnixTimeMilliseconds(),
        };

        await dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    }

    public async IAsyncEnumerable<IIntegrationEvent> GetBatch(
        int batchSize = 10,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var outboxMessages = dbContext.OutboxMessages
            .OrderBy(x => x.CreatedAt)
            .Take(batchSize)
            .AsNoTracking()
            .AsAsyncEnumerable();

        await foreach (var outboxMessage in outboxMessages)
        {
            var type = Type.GetType(outboxMessage.Type);
            if (type is null)
            {
                logger.LogWarning("Type '{Type}' not found", outboxMessage.Type);

                continue;
            }

            var integrationEvent = JsonSerializer.Deserialize(outboxMessage.Message, type, jsonSerializerOptions);
            if (integrationEvent is null)
            {
                logger.LogWarning("Failed to deserialize message '{Message}'", outboxMessage.Id);

                continue;
            }

            yield return (IIntegrationEvent)integrationEvent;
        }
    }

    public async Task Acknowledge(
        IIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        var outboxMessage = await dbContext.OutboxMessages
            .FirstOrDefaultAsync(x => x.Id == integrationEvent.MessageId, cancellationToken);

        if (outboxMessage is null)
        {
            logger.LogWarning("Outbox message {MessageId} not found", integrationEvent.MessageId);
            return;
        }

        dbContext.OutboxMessages.Remove(outboxMessage);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}