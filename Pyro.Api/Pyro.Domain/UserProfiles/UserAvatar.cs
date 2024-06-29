// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Domain.UserProfiles;

public class UserAvatar
{
    public Guid Id { get; init; }

    public required byte[] Image { get; init; }
}