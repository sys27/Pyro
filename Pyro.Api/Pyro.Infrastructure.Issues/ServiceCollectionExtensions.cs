// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pyro.Domain.Issues;
using Pyro.Infrastructure.Issues.DataAccess;
using Pyro.Infrastructure.Shared.DataAccess;

namespace Pyro.Infrastructure.Issues;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIssuesInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<IssuesDbContext>((provider, options) => options
            .UseSqlite(configuration.GetConnectionString("DefaultConnection"))
            .AddInterceptors(provider.GetRequiredService<DomainEventInterceptor>()));
        services.AddScoped<DbContext, IssuesDbContext>(sp => sp.GetRequiredService<IssuesDbContext>());

        return services
            .AddScoped<IIssueRepository, IssueRepository>();
    }
}