// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Domain.UserProfiles;

public class UserProfile
{
    private readonly string? name;
    private readonly string? status;

    public Guid Id { get; init; }

    public string? Name
    {
        get => name;
        init
        {
            if (value?.Length > 50)
                throw new ArgumentOutOfRangeException(nameof(Name), "Name cannot be longer than 50 characters.");

            name = value;
        }
    }

    public string? Status
    {
        get => status;
        init
        {
            if (value?.Length > 150)
                throw new ArgumentOutOfRangeException(nameof(Status), "Status cannot be longer than 50 characters.");

            status = value;
        }
    }

    public UserAvatar? Avatar { get; init; }
}