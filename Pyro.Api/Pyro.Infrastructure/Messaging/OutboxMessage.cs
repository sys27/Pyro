// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Infrastructure.Messaging;

internal sealed class OutboxMessage
{
    public Guid Id { get; init; }

    public required string Type { get; init; }

    public required string Message { get; init; }

    public int Retries { get; private set; }

    public required long CreatedAt { get; init; }
}