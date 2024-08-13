// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Extensions;

internal static class ProblemDetailsExtensions
{
    public static IServiceCollection AddProblemDetails(
        this IServiceCollection services,
        IWebHostEnvironment environment)
    {
        // TODO: only needed for problem details (see https://github.com/khellang/Middleware/issues/182)
        services.AddMvcCore();
        services.AddProblemDetails(options =>
        {
            options.IncludeExceptionDetails = (_, _) => environment.IsDevelopment();

            options.Map<ValidationException>(ex =>
            {
                var errors = ex.Errors
                    .GroupBy(x => x.PropertyName, x => x.ErrorMessage)
                    .ToDictionary(x => x.Key, x => x.ToArray());

                return new HttpValidationProblemDetails(errors)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://httpstatuses.io/400",
                };
            });

            options.Map<DomainValidationException>(ex =>
            {
                var errors = new Dictionary<string, string[]>
                {
                    { string.Empty, [ex.Message] },
                };

                return new HttpValidationProblemDetails(errors)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://httpstatuses.io/400",
                };
            });

            options.MapToStatusCode<NotFoundException>(StatusCodes.Status404NotFound);
            options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
        });

        return services;
    }
}