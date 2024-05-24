// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Pyro.Domain.Identity;
using Pyro.Domain.Identity.Models;

namespace Pyro.Infrastructure.DataAccess;

public class UserRepository : IUserRepository
{
    private readonly PyroDbContext dbContext;

    public UserRepository(PyroDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<User?> GetUserById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var user = await dbContext
            .Set<User>()
            .Include(x => x.Roles)
            .ThenInclude(x => x.Permissions)
            .Include(x => x.Tokens)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return user;
    }

    public async Task<User?> GetUserByEmail(
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await dbContext
            .Set<User>()
            .Include(x => x.Roles)
            .ThenInclude(x => x.Permissions)
            .Include(x => x.Tokens)
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

        return user;
    }

    public async Task<IEnumerable<Role>> GetRolesAsync(CancellationToken cancellationToken)
    {
        var roles = await dbContext
            .Set<Role>()
            .Include(x => x.Permissions)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return roles;
    }

    public async Task<IEnumerable<Permission>> GetPermissionsAsync(CancellationToken cancellationToken)
    {
        var permissions = await dbContext
            .Set<Permission>()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return permissions;
    }
}