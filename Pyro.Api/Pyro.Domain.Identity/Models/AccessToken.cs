// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Pyro.Domain.Core.Exceptions;

namespace Pyro.Domain.Identity.Models;

public class AccessToken
{
    private readonly string name;
    private readonly byte[] token;
    private readonly byte[] salt;

    public Guid Id { get; init; }

    public required string Name
    {
        get => name;
        [MemberNotNull(nameof(name))]
        init
        {
            if (value is null)
                throw new DomainValidationException("Name is required");

            if (value.Length > 50)
                throw new DomainValidationException("Name is too long");

            name = value;
        }
    }

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

    public required DateTimeOffset ExpiresAt { get; init; }

    public Guid UserId { get; init; }
}