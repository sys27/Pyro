// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Domain.Shared.Entities;

public abstract class DomainEntity : Entity
{
    private readonly HashSet<IDomainEvent> domainEvents = [];

    protected void PublishEvent(IDomainEvent domainEvent)
        => domainEvents.Add(domainEvent);

    public void ClearEvents()
        => domainEvents.Clear();

    public IReadOnlyCollection<IDomainEvent> DomainEvents
        => domainEvents;
}