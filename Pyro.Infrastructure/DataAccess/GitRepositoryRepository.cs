// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Pyro.Domain.GitRepositories;

namespace Pyro.Infrastructure.DataAccess;

internal class GitRepositoryRepository : IGitRepositoryRepository
{
    private readonly PyroDbContext dbContext;

    public GitRepositoryRepository(PyroDbContext dbContext)
        => this.dbContext = dbContext;

    public async Task<GitRepository?> GetGitRepository(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var gitRepository = await dbContext.GitRepositories
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return gitRepository;
    }

    public async Task<GitRepository?> GetGitRepository(
        string name,
        CancellationToken cancellationToken = default)
    {
        var gitRepository = await dbContext.GitRepositories
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

        return gitRepository;
    }

    public async Task<GitRepository> AddGitRepository(
        GitRepository gitRepository,
        CancellationToken cancellationToken = default)
    {
        await dbContext.GitRepositories.AddAsync(gitRepository, cancellationToken);

        return gitRepository;
    }
}