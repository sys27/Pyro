// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Shared.Models;

namespace Pyro.Domain.UserProfiles;

public class UserAvatar : Entity
{
#pragma warning disable CA1819
    public required byte[] Image { get; init; }
#pragma warning restore CA1819
}