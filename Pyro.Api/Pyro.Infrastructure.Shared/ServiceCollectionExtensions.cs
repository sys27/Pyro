// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using Pyro.Infrastructure.Shared.DataAccess;

namespace Pyro.Infrastructure.Shared;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services)
    {
        return services
            .AddTransient<DomainEventInterceptor>()
            .AddScoped<UnitOfWork>();
    }
}