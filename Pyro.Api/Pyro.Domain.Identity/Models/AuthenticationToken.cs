// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Shared.Models;

namespace Pyro.Domain.Identity.Models;

// TODO: remove old tokens
public class AuthenticationToken : Entity
{
    public static AuthenticationToken Create(Guid tokenId, User user, DateTimeOffset expiresAt)
        => new AuthenticationToken
        {
            TokenId = tokenId,
            ExpiresAt = expiresAt,
            UserId = user.Id,
            User = user,
        };

    public Guid TokenId { get; private set; }

    public DateTimeOffset ExpiresAt { get; private set; }

    public Guid UserId { get; private set; }

    public User User { get; private set; } = null!;

    public bool IsExpired(TimeProvider timeProvider)
        => ExpiresAt <= timeProvider.GetUtcNow();
}