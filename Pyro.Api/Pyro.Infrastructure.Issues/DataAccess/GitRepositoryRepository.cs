// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Pyro.Domain.Issues;

namespace Pyro.Infrastructure.Issues.DataAccess;

internal class GitRepositoryRepository : IGitRepositoryRepository
{
    private readonly IssuesDbContext dbContext;

    public GitRepositoryRepository(IssuesDbContext dbContext)
        => this.dbContext = dbContext;

    public async Task<GitRepository?> GetRepository(string name, CancellationToken cancellationToken = default)
    {
        var repository = await dbContext
            .Set<GitRepository>()
            .Include(x => x.Labels)
            .Include(x => x.IssueStatuses).ThenInclude(x => x.FromTransitions)
            .Include(x => x.IssueStatuses).ThenInclude(x => x.ToTransitions)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

        return repository;
    }

    public async Task<User?> GetUser(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var user = await dbContext
            .Set<User>()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        return user;
    }

    public async Task<IReadOnlyList<User>> GetUsers(CancellationToken cancellationToken = default)
    {
        var users = await dbContext
            .Set<User>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return users;
    }
}