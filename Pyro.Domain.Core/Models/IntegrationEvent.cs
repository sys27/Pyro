// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Domain.Core.Models;

public abstract record IntegrationEvent : IIntegrationEvent
{
    public Guid MessageId { get; init; } = Guid.NewGuid();
}