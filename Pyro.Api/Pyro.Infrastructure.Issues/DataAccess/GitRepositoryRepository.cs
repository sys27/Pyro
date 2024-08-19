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
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

        return repository;
    }
}