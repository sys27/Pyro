// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.UserProfiles;

namespace Pyro.Infrastructure.DataAccess;

internal class UserProfileRepository : IUserProfileRepository
{
    private readonly PyroDbContext dbContext;

    public UserProfileRepository(PyroDbContext dbContext)
        => this.dbContext = dbContext;

    public async Task AddUserProfile(UserProfile userProfile, CancellationToken cancellationToken)
    {
        await dbContext
            .Set<UserProfile>()
            .AddAsync(userProfile, cancellationToken);
    }
}