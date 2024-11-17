// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;
using Microsoft.Extensions.Options;
using Pyro.Domain.Identity.Commands;

namespace Pyro.BackgroundServices;

internal class NotifyExpiringPasswordsBackgroundService : BackgroundService
{
    private readonly ILogger<NotifyExpiringPasswordsBackgroundService> logger;
    private readonly IHostApplicationLifetime applicationLifetime;
    private readonly IServiceProvider serviceProvider;
    private readonly NotifyExpiringPasswordsOptions options;

    public NotifyExpiringPasswordsBackgroundService(
        ILogger<NotifyExpiringPasswordsBackgroundService> logger,
        IHostApplicationLifetime applicationLifetime,
        IServiceProvider serviceProvider,
        IOptions<NotifyExpiringPasswordsOptions> options)
    {
        this.logger = logger;
        this.applicationLifetime = applicationLifetime;
        this.serviceProvider = serviceProvider;
        this.options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!await applicationLifetime.WaitForApplicationStarted(stoppingToken))
            return;

        logger.LogInformation("NotifyExpiringPasswords started.");

        using var scope = serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        while (!stoppingToken.IsCancellationRequested)
        {
            await mediator.Send(new NotifyExpiringPasswords(), stoppingToken);

            await Task.Delay(options.Interval, stoppingToken);
        }
    }
}

internal class NotifyExpiringPasswordsOptions
{
    public const string Name = nameof(NotifyExpiringPasswordsBackgroundService);

    public TimeSpan Interval { get; set; } = TimeSpan.FromDays(1);
}