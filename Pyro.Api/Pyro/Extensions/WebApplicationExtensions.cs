// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Pyro.Infrastructure.DataAccess;
using Pyro.Infrastructure.Identity.DataAccess;
using Pyro.Infrastructure.Issues.DataAccess;

namespace Pyro.Extensions;

public static class WebApplicationExtensions
{
    public static void ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var pyroDbContext = scope.ServiceProvider.GetRequiredService<PyroDbContext>();
        pyroDbContext.Database.Migrate();

        var identityDbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        identityDbContext.Database.Migrate();

        var issuesDbContext = scope.ServiceProvider.GetRequiredService<IssuesDbContext>();
        issuesDbContext.Database.Migrate();
    }
}