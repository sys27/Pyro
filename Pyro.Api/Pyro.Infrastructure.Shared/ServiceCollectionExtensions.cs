// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using Pyro.Domain.Shared.Email;
using Pyro.Infrastructure.Shared.DataAccess;
using Pyro.Infrastructure.Shared.Email;

namespace Pyro.Infrastructure.Shared;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services)
    {
        return services
            .AddTransient<DomainEventInterceptor>()
            .AddScoped<UnitOfWork>()
            .AddTransient<IEmailService, ConsoleEmailService>(); // TODO: Replace with real email service
    }
}