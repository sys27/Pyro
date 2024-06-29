// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Domain.Identity.Models;

public class SigningKey
{
    public Guid Id { get; init; }

    public required string Key { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }
}