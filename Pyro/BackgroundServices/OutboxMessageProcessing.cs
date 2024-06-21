// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;
using Pyro.Domain.Core;

namespace Pyro.BackgroundServices;

public class OutboxMessageProcessing : BackgroundService
{
    private readonly ILogger<OutboxMessageProcessing> logger;
    private readonly IServiceProvider serviceProvider;
    private readonly IHostApplicationLifetime applicationLifetime;

    public OutboxMessageProcessing(
        ILogger<OutboxMessageProcessing> logger,
        IServiceProvider serviceProvider,
        IHostApplicationLifetime applicationLifetime)
    {
        this.logger = logger;
        this.serviceProvider = serviceProvider;
        this.applicationLifetime = applicationLifetime;
    }

    private async Task<bool> WaitForApplicationStarted(CancellationToken stoppingToken)
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();
        await using var r1 = applicationLifetime.ApplicationStarted
            .Register(() => taskCompletionSource.SetResult(true));

        await using var registration = stoppingToken
            .Register(() => taskCompletionSource.SetResult(false));

        return await taskCompletionSource.Task;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!await WaitForApplicationStarted(stoppingToken))
        {
            return;
        }

        // TODO: configuration
        const int batchSize = 10;
        var delay = TimeSpan.FromSeconds(3);

        logger.LogInformation("Outbox message processing started");

        using var scope = serviceProvider.CreateScope();
        var bus = scope.ServiceProvider.GetRequiredService<IBus>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        while (!stoppingToken.IsCancellationRequested)
        {
            var messages = bus.GetBatch(batchSize, stoppingToken);
            await foreach (var message in messages)
            {
                logger.LogInformation("Processing message {MessageId}", message.MessageId);

                // TODO: handle exceptions, limit retry count
                await mediator.Publish(message, stoppingToken);
                await bus.Acknowledge(message, stoppingToken);

                logger.LogInformation("Message {MessageId} processed", message.MessageId);
            }

            await Task.Delay(delay, stoppingToken);
        }
    }
}