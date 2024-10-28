// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.BackgroundServices;

internal static class BackgroundServiceExtensions
{
    public static async Task<bool> WaitForApplicationStarted(
        this IHostApplicationLifetime applicationLifetime,
        CancellationToken stoppingToken = default)
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();
        await using var r1 = applicationLifetime.ApplicationStarted
            .Register(() => taskCompletionSource.SetResult(true));

        await using var registration = stoppingToken
            .Register(() => taskCompletionSource.SetResult(false));

        return await taskCompletionSource.Task;
    }
}