// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging.Console;
using Pyro.Services;

namespace Pyro.Extensions;

internal static class LoggingExtensions
{
    public static IHostApplicationBuilder AddLogging(this IHostApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Services.AddSingleton<ConsoleLoggerProvider>();
        builder.Services.AddSingleton<ILoggerProvider, LoggerProvider>();

        return builder;
    }
}