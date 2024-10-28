// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Pyro.Domain.Identity;
using Pyro.Domain.Identity.Models;
using Pyro.Domain.Identity.Queries;

namespace Pyro.Infrastructure.Identity.DataAccess;

internal class UserRepository : IUserRepository
{
    private readonly IdentityDbContext dbContext;

    public UserRepository(IdentityDbContext dbContext)
        => this.dbContext = dbContext;

    public async Task<IReadOnlyList<User>> GetUsers(
        GetUsers query,
        CancellationToken cancellationToken = default)
    {
        var users = Users;

        if (query.Before is not null)
        {
            users = users
                .Where(x => query.Before == null || x.Login.CompareTo(query.Before) < 0)
                .OrderByDescending(x => x.Login)
                .Take(query.Size)
                .OrderBy(x => x.Login);
        }
        else if (query.After is not null)
        {
            users = users
                .Where(x => query.After == null || x.Login.CompareTo(query.After) > 0)
                .OrderBy(x => x.Login)
                .Take(query.Size);
        }
        else
        {
            users = users
                .OrderBy(x => x.Login)
                .Take(query.Size);
        }

        var result = await users.ToListAsync(cancellationToken);

        return result;
    }

    public async Task<User?> GetUserById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var user = await Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return user;
    }

    public async Task<User?> GetUserByLogin(
        string login,
        CancellationToken cancellationToken = default)
    {
        var user = await Users.FirstOrDefaultAsync(x => x.Login == login, cancellationToken);

        return user;
    }

    public async Task<User?> GetUserByToken(
        string token,
        CancellationToken cancellationToken = default)
    {
        var user = await Users.FirstOrDefaultAsync(
            x => x.OneTimePasswords.Any(y => y.Token == token),
            cancellationToken);

        return user;
    }

    public async Task<User> AddUser(User user, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Set<User>().AddAsync(user, cancellationToken);

        return entity.Entity;
    }

    public async Task<IReadOnlyList<Role>> GetRolesAsync(CancellationToken cancellationToken)
    {
        var roles = await dbContext
            .Set<Role>()
            .Include(x => x.Permissions)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return roles;
    }

    public async Task<IReadOnlyList<Permission>> GetPermissionsAsync(CancellationToken cancellationToken)
    {
        var permissions = await dbContext
            .Set<Permission>()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return permissions;
    }

    private IQueryable<User> Users
        => dbContext
            .Set<User>()
            .Include(x => x.Roles)
            .ThenInclude(x => x.Permissions)
            .Include(x => x.AuthenticationTokens)
            .Include(x => x.AccessTokens)
            .Include(x => x.OneTimePasswords)
            .AsSplitQuery();
}