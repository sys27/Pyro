// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.UserProfiles;

namespace Pyro.Infrastructure.DataAccess.Configurations;

public static class SeedData
{
    public static readonly IReadOnlyCollection<UserProfile> UserProfiles;

    static SeedData()
    {
        UserProfiles =
        [
            new UserProfile
            {
                Id = UserProfile.Pyro,
                Email = "pyro@localhost.local",
                Name = "Pyro",
            }
        ];
    }
}