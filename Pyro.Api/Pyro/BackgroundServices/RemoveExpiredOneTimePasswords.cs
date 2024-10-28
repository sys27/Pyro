// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pyro.Domain.Identity.Models;
using Pyro.Infrastructure.Identity.DataAccess;

namespace Pyro.BackgroundServices;

internal class RemoveExpiredOneTimePasswords : BackgroundService
{
    private readonly TimeProvider timeProvider;
    private readonly IHostApplicationLifetime applicationLifetime;
    private readonly ILogger<RemoveExpiredOneTimePasswords> logger;
    private readonly RemoveExpiredOneTimePasswordsOptions options;
    private readonly IServiceProvider serviceProvider;

    public RemoveExpiredOneTimePasswords(
        TimeProvider timeProvider,
        IHostApplicationLifetime applicationLifetime,
        ILogger<RemoveExpiredOneTimePasswords> logger,
        IOptions<RemoveExpiredOneTimePasswordsOptions> options,
        IServiceProvider serviceProvider)
    {
        this.timeProvider = timeProvider;
        this.applicationLifetime = applicationLifetime;
        this.logger = logger;
        this.options = options.Value;
        this.serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!await applicationLifetime.WaitForApplicationStarted(stoppingToken))
            return;

        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = timeProvider.GetUtcNow();
            var count = await dbContext.Set<OneTimePassword>()
                .Where(otp => otp.ExpiresAt < now)
                .ExecuteDeleteAsync(stoppingToken);

            if (count > 0)
                logger.LogInformation("Removed {Count} expired one-time passwords", count);

            await Task.Delay(options.Interval, stoppingToken);
        }
    }
}

internal class RemoveExpiredOneTimePasswordsOptions
{
    public const string Name = nameof(RemoveExpiredOneTimePasswords);

    public TimeSpan Interval { get; set; } = TimeSpan.FromMinutes(30);
}