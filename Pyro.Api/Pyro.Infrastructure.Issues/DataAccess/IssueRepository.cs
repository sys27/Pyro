// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Pyro.Domain.Issues;
using Pyro.Domain.Issues.Queries;

namespace Pyro.Infrastructure.Issues.DataAccess;

internal class IssueRepository : IIssueRepository
{
    private readonly IssuesDbContext dbContext;

    public IssueRepository(IssuesDbContext dbContext)
        => this.dbContext = dbContext;

    public async Task<IReadOnlyList<Issue>> GetIssues(
        GetIssues query,
        CancellationToken cancellationToken = default)
    {
        var issues = dbContext
            .Set<Issue>()
            .Include(x => x.Assignee)
            .Include(x => x.Author)
            .Include(x => x.Labels)
            .Where(x => x.Repository.Name == query.RepositoryName)
            .AsNoTracking();

        if (query.Before is not null)
        {
            issues = issues
                .Where(x => query.Before == null || x.Id > query.Before)
                .OrderBy(x => x.Id)
                .Take(query.Size)
                .OrderByDescending(x => x.Id);
        }
        else if (query.After is not null)
        {
            issues = issues
                .Where(x => query.After == null || x.Id < query.After)
                .OrderByDescending(x => x.Id)
                .Take(query.Size);
        }
        else
        {
            issues = issues
                .OrderByDescending(x => x.Id)
                .Take(query.Size);
        }

        var result = await issues.ToListAsync(cancellationToken);

        return result;
    }

    public Task<Issue?> GetIssue(
        string repositoryName,
        int number,
        CancellationToken cancellationToken = default)
    {
        var issue = dbContext
            .Set<Issue>()
            .Include(x => x.Assignee)
            .Include(x => x.Author)
            .Include(x => x.Comments.OrderBy(c => c.CreatedAt))
            .Include(x => x.Labels)
            .FirstOrDefaultAsync(
                i => i.Repository.Name == repositoryName &&
                     i.IssueNumber == number,
                cancellationToken);

        return issue;
    }

    public async Task AddIssue(
        Issue issue,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Set<Issue>().AddAsync(issue, cancellationToken);
    }

    public void DeleteIssue(Issue issue)
    {
        issue.ClearLabels();
        dbContext.Set<Issue>().Remove(issue);
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