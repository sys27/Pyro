// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Shared.Entities;

namespace Pyro.Domain.Identity.Models;

public class SigningKey : Entity
{
    public required string Key { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }
}