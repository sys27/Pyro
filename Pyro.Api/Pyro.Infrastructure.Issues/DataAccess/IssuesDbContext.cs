// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;

namespace Pyro.Infrastructure.Issues.DataAccess;

public class IssuesDbContext : DbContext
{
    public IssuesDbContext(DbContextOptions<IssuesDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IssuesDbContext).Assembly);
    }
}