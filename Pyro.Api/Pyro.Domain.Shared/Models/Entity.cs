// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MassTransit;

namespace Pyro.Domain.Shared.Models;

public abstract class Entity : IEntity
{
    public Guid Id { get; init; } = NewId.NextSequentialGuid();
}