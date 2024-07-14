// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;

namespace Pyro.Domain.Shared;

public class ValidatorPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> validators;

    public ValidatorPipeline(IEnumerable<IValidator<TRequest>> validators)
        => this.validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        foreach (var validator in validators)
        {
            await validator.ValidateAndThrowAsync(request, cancellationToken);
        }

        return await next();
    }
}