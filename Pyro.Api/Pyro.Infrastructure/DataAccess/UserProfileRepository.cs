// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Pyro.Domain.UserProfiles;

namespace Pyro.Infrastructure.DataAccess;

internal class UserProfileRepository : IUserProfileRepository
{
    private readonly PyroDbContext dbContext;

    public UserProfileRepository(PyroDbContext dbContext)
        => this.dbContext = dbContext;

    public async Task<UserProfile?> GetUserProfile(Guid userId, CancellationToken cancellationToken)
    {
        var profile = await dbContext
            .Set<UserProfile>()
            .Include(x => x.Avatar)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        return profile;
    }

    public async Task AddUserProfile(UserProfile userProfile, CancellationToken cancellationToken)
    {
        await dbContext
            .Set<UserProfile>()
            .AddAsync(userProfile, cancellationToken);
    }
}