// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Pyro.Domain.Shared.Entities;

namespace Pyro.Infrastructure.Shared.DataAccess;

public class UnitOfWork
{
    private readonly IEnumerable<DbContext> dbContexts;

    public UnitOfWork(IEnumerable<DbContext> dbContexts)
        => this.dbContexts = dbContexts;

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var hasEvents = dbContexts
            .SelectMany(x => x.ChangeTracker.Entries<DomainEntity>())
            .Select(x => x.Entity)
            .Any(x => x.DomainEvents.Any());

        await Save(cancellationToken);

        if (hasEvents)
            await Save(cancellationToken);
    }

    private async Task Save(CancellationToken cancellationToken)
    {
        foreach (var dbContext in dbContexts)
            await dbContext.SaveChangesAsync(cancellationToken);
    }
}