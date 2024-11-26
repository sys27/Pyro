// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Pyro.Domain.Shared.Entities;

namespace Pyro.Domain.Identity.Models;

public class AccessToken : Entity
{
    private readonly byte[] token;
    private readonly byte[] salt;

    public required string Name { get; init; }

    public required IReadOnlyList<byte> Token
    {
        get => token;
        [MemberNotNull(nameof(token))]
        init => token = [..value];
    }

    public required IReadOnlyList<byte> Salt
    {
        get => salt;
        [MemberNotNull(nameof(salt))]
        init => salt = [..value];
    }

    // TODO: remove setter
    public required DateTimeOffset ExpiresAt { get; set; }

    public Guid UserId { get; init; }

    public void Invalidate(TimeProvider timeProvider)
        => ExpiresAt = timeProvider.GetUtcNow();
}