// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging.Console;

namespace Pyro.Services;

internal sealed class LoggerProvider : ILoggerProvider
{
    private readonly ConsoleLoggerProvider loggerProvider;

    public LoggerProvider(ConsoleLoggerProvider loggerProvider)
        => this.loggerProvider = loggerProvider;

    public void Dispose()
        => loggerProvider.Dispose();

    public ILogger CreateLogger(string categoryName)
    {
        var logger = loggerProvider.CreateLogger(categoryName);

        return new Logger(logger);
    }
}

internal sealed class Logger : ILogger
{
    private readonly ILogger logger;

    public Logger(ILogger logger)
        => this.logger = logger;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        => logger.BeginScope(state);

    public bool IsEnabled(LogLevel logLevel)
        => logger.IsEnabled(logLevel);

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        var log = formatter(state, exception);
        if (log.Contains("-- NO_LOGGING"))
            return;

        logger.Log(logLevel, eventId, state, exception, formatter);
    }
}