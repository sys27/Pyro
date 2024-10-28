// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;
using Microsoft.Extensions.Options;
using Pyro.Domain.Shared.Messaging;

namespace Pyro.BackgroundServices;

internal class OutboxMessageProcessing : BackgroundService
{
    private readonly ILogger<OutboxMessageProcessing> logger;
    private readonly IServiceProvider serviceProvider;
    private readonly IHostApplicationLifetime applicationLifetime;
    private readonly OutboxMessageProcessingOptions options;

    public OutboxMessageProcessing(
        ILogger<OutboxMessageProcessing> logger,
        IServiceProvider serviceProvider,
        IHostApplicationLifetime applicationLifetime,
        IOptions<OutboxMessageProcessingOptions> options)
    {
        this.logger = logger;
        this.serviceProvider = serviceProvider;
        this.applicationLifetime = applicationLifetime;
        this.options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!await applicationLifetime.WaitForApplicationStarted(stoppingToken))
            return;

        logger.LogInformation("Outbox message processing started");

        using var scope = serviceProvider.CreateScope();
        var bus = scope.ServiceProvider.GetRequiredService<IBus>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        while (!stoppingToken.IsCancellationRequested)
        {
            var messages = bus.GetBatch(options.BatchSize, stoppingToken);
            await foreach (var message in messages)
            {
                logger.LogInformation("Processing message {MessageId}", message.MessageId);

                // TODO: handle exceptions, limit retry count
                await mediator.Publish(message, stoppingToken);
                await bus.Acknowledge(message, stoppingToken);

                logger.LogInformation("Message {MessageId} processed", message.MessageId);
            }

            await Task.Delay(options.Interval, stoppingToken);
        }
    }
}

internal class OutboxMessageProcessingOptions
{
    public const string Name = nameof(OutboxMessageProcessing);

    public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(3);
    public int BatchSize { get; set; } = 10;
}