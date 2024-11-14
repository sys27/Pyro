// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using JWT.Extensions.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

namespace Pyro.Extensions;

internal static class OpenApiExtensions
{
    public static IServiceCollection AddPyroOpenApi(this IServiceCollection services)
        => services.AddOpenApi(options =>
        {
            var scheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Name = JwtAuthenticationDefaults.AuthenticationScheme,
                Scheme = JwtAuthenticationDefaults.AuthenticationScheme,
                Reference = new OpenApiReference
                {
                    Id = JwtAuthenticationDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme,
                },
            };

            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes.Add(JwtAuthenticationDefaults.AuthenticationScheme, scheme);

                return Task.CompletedTask;
            });
            options.AddOperationTransformer((operation, context, _) =>
            {
                var metadata = context.Description.ActionDescriptor.EndpointMetadata;
                if (metadata.OfType<AllowAnonymousAttribute>().Any())
                    return Task.CompletedTask;

                if (metadata.OfType<IAuthorizeData>().Any())
                    operation.Security = [new OpenApiSecurityRequirement { [scheme] = [] }];

                return Task.CompletedTask;
            });
        });
}