// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pyro.Infrastructure.Messaging;
using Pyro.Infrastructure.Shared.DataAccess;

namespace Pyro.Infrastructure.DataAccess;

/// <summary>
/// Represents the database context for Pyro.
/// </summary>
public class PyroDbContext : DbContext, IDataProtectionKeyContext
{
    public PyroDbContext(DbContextOptions<PyroDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PyroDbContext).Assembly);
        modelBuilder.ConfigureIds();
    }

    internal DbSet<OutboxMessage> OutboxMessages { get; init; }

    public DbSet<DataProtectionKey> DataProtectionKeys { get; init; }
}