// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Domain.Identity.Models;

// TODO: remove old tokens
public class AuthenticationToken
{
    public Guid Id { get; init; }

    public required Guid TokenId { get; init; }

    public required DateTimeOffset ExpiresAt { get; init; }

    public required Guid UserId { get; init; }

    public required User User { get; init; }
}