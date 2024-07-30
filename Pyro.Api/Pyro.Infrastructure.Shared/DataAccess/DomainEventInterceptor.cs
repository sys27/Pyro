// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Pyro.Domain.Shared.Models;

namespace Pyro.Infrastructure.Shared.DataAccess;

public class DomainEventInterceptor : SaveChangesInterceptor
{
    private readonly IMediator mediator;

    public DomainEventInterceptor(IMediator mediator)
        => this.mediator = mediator;

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context is null)
            return result;

        var domainEntities = context.ChangeTracker
            .Entries<DomainEntity>()
            .Select(x => x.Entity)
            .Where(x => x.DomainEvents.Any())
            .ToArray();

        foreach (var domainEntity in domainEntities)
        {
            foreach (var domainEvent in domainEntity.DomainEvents)
                await mediator.Publish(domainEvent, cancellationToken);

            domainEntity.ClearEvents();
        }

        return result;
    }
}