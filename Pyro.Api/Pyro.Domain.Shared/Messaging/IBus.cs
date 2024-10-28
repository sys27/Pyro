// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Domain.Shared.Messaging;

/// <summary>
/// Represents a bus that can be used to publish and consume integration events.
/// </summary>
public interface IBus
{
    Task Publish<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent;

    IAsyncEnumerable<IIntegrationEvent> GetBatch(
        int batchSize = 10,
        CancellationToken cancellationToken = default);

    Task Acknowledge(
        IIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default);
}