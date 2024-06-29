// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;

namespace Pyro;

internal class LoggingPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingPipeline<TRequest, TResponse>> logger;

    public LoggingPipeline(ILogger<LoggingPipeline<TRequest, TResponse>> logger)
        => this.logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling {Request}", request.GetType().Name);
        var response = await next();
        logger.LogInformation("Handled {Request}", request.GetType().Name);

        return response;
    }
}