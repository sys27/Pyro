// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pyro.Domain.Identity;
using Pyro.Infrastructure.Identity.DataAccess;
using Pyro.Infrastructure.Shared.DataAccess;

namespace Pyro.Infrastructure.Identity;

public static class ServiceCollectionExtensions
{
    public static IHostApplicationBuilder AddIdentityInfrastructure(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.AddDbContext<IdentityDbContext>((provider, options) =>
            {
                var env = provider.GetRequiredService<IHostEnvironment>();

                options
                    .UseSqlite(configuration.GetConnectionString("DefaultConnection"))
                    .EnableDetailedErrors(env.IsDevelopment())
                    .EnableSensitiveDataLogging(env.IsDevelopment())
                    .AddInterceptors(provider.GetRequiredService<DomainEventInterceptor>())
                    .ConfigureWarnings(w =>
                    {
#if DEBUG
                        w.Throw(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.MultipleCollectionIncludeWarning);
#endif
                    });
            })
            .AddScoped<DbContext, IdentityDbContext>(sp => sp.GetRequiredService<IdentityDbContext>())
            .AddScoped<IUserRepository, UserRepository>()
            .AddSingleton<ISigningKeyService, SigningKeyService>();

        return builder;
    }
}