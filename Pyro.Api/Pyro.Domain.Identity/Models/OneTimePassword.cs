// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Shared.Entities;

namespace Pyro.Domain.Identity.Models;

public class OneTimePassword : Entity
{
    public required string Token { get; init; }

    // TODO: remove setter
    public required DateTimeOffset ExpiresAt { get; set; }

    public required OneTimePasswordPurpose Purpose { get; init; }

    public required User User { get; init; }

    public bool IsUserRegistration
        => Purpose == OneTimePasswordPurpose.UserRegistration;

    public void Invalidate(TimeProvider timeProvider)
        => ExpiresAt = timeProvider.GetUtcNow();
}