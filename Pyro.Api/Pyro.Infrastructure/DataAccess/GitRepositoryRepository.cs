// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Pyro.Domain.GitRepositories;
using Pyro.Domain.GitRepositories.Queries;

namespace Pyro.Infrastructure.DataAccess;

internal class GitRepositoryRepository : IGitRepositoryRepository
{
    private readonly PyroDbContext dbContext;

    public GitRepositoryRepository(PyroDbContext dbContext)
        => this.dbContext = dbContext;

    public async Task<IReadOnlyList<GitRepository>> GetRepositories(
        GetGitRepositories query,
        CancellationToken cancellationToken = default)
    {
        Debug.Assert(query.Before is null || query.After is null, "Both After and Before cannot be set.");

        var repositories = dbContext.Set<GitRepository>()
            .AsQueryable();

        if (query.Before is not null)
        {
            repositories = repositories
                .Where(x => query.Before == null || x.Name.CompareTo(query.Before) < 0)
                .OrderByDescending(x => x.Name)
                .Take(query.Size)
                .OrderBy(x => x.Name);
        }
        else if (query.After is not null)
        {
            repositories = repositories
                .Where(x => query.After == null || x.Name.CompareTo(query.After) > 0)
                .OrderBy(x => x.Name)
                .Take(query.Size);
        }
        else
        {
            repositories = repositories
                .OrderBy(x => x.Name)
                .Take(query.Size);
        }

        var gitRepositories = await repositories.ToListAsync(cancellationToken);

        return gitRepositories;
    }

    public async Task<GitRepository?> GetGitRepository(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var gitRepository = await dbContext.Set<GitRepository>()
            .Include(x => x.Tags)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return gitRepository;
    }

    public async Task<GitRepository?> GetGitRepository(
        string name,
        CancellationToken cancellationToken = default)
    {
        var gitRepository = await dbContext.Set<GitRepository>()
            .Include(x => x.Tags)
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

        return gitRepository;
    }

    public async Task<GitRepository> AddGitRepository(
        GitRepository gitRepository,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Set<GitRepository>().AddAsync(gitRepository, cancellationToken);

        return gitRepository;
    }
}