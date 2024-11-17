// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pyro.Domain.Git;
using Pyro.Domain.GitRepositories;
using Pyro.Domain.Shared.Messaging;
using Pyro.Infrastructure.DataAccess;
using Pyro.Infrastructure.Messaging;
using Pyro.Infrastructure.Shared.DataAccess;

namespace Pyro.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services
            .Configure<GitOptions>(configuration.GetSection(GitOptions.Section))
            .AddDbContext<PyroDbContext>((provider, options) =>
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
            .AddScoped<DbContext, PyroDbContext>(sp => sp.GetRequiredService<PyroDbContext>())
            .AddScoped<IBus, Bus>()
            .AddScoped<IGitService, GitService>()
            .AddScoped<IGitRepositoryRepository, GitRepositoryRepository>();

        return builder;
    }
}