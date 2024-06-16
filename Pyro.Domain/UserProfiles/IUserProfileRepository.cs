// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Domain.UserProfiles;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetUserProfile(Guid userId, CancellationToken cancellationToken);

    Task AddUserProfile(UserProfile userProfile, CancellationToken cancellationToken);
}