// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Domain.UserProfiles;

public class UserProfile
{
    public Guid Id { get; init; }

    public string? Email { get; init; }

    public string? Name { get; set; }

    public string? Status { get; set; }

    public UserAvatar? Avatar { get; init; }
}