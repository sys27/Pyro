// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Options;
using Pyro.BackgroundServices;
using Pyro.Services;

namespace Pyro;

public static class ServiceCollectionExtensions
{
    public static IHostApplicationBuilder AddPyroInfrastructure(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services
            .Configure<ServiceOptions>(configuration.GetRequiredSection(ServiceOptions.Name))
            .AddSingleton<IValidateOptions<ServiceOptions>, ServiceOptions>()
            .AddScoped<GitBackend>()
            .AddHostedService<OutboxMessageProcessing>()
            .Configure<OutboxMessageProcessingOptions>(
                configuration
                    .GetRequiredSection("Workers")
                    .GetRequiredSection(OutboxMessageProcessingOptions.Name))
            .AddHostedService<RemoveExpiredOneTimePasswords>()
            .Configure<RemoveExpiredOneTimePasswordsOptions>(
                configuration
                    .GetRequiredSection("Workers")
                    .GetRequiredSection(RemoveExpiredOneTimePasswordsOptions.Name))
            .AddHostedService<NotifyExpiringPasswordsBackgroundService>()
            .Configure<NotifyExpiringPasswordsBackgroundService>(
                configuration
                    .GetRequiredSection("Workers")
                    .GetRequiredSection(NotifyExpiringPasswordsOptions.Name));

        return builder;
    }
}