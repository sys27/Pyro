// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Shared.Entities;

namespace Pyro.Domain.UserProfiles;

public class UserProfile : Entity
{
    public static readonly Guid Pyro = Guid.Parse("F9BA057A-35B0-4D10-8326-702D8F7EC966");

    public required string Name { get; set; }

    public string? Status { get; set; }

    public UserAvatar? Avatar { get; init; }

    public User User { get; init; } = null!;
}