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
        var gitRepository = await dbContext
            .Set<GitRepository>()
            .FirstOrDefaultAsync(x => x.Name == query.RepositoryName, cancellationToken);
        if (gitRepository is null)
            return [];

        var issues = dbContext
            .Set<Issue>()
            .Include(x => x.Assignee)
            .Include(x => x.Author)
            .Include(x => x.Labels)
            .Include(x => x.Status)
            .Where(x => x.RepositoryId == gitRepository.Id)
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

    public async Task<Issue?> GetIssue(
        string repositoryName,
        int number,
        CancellationToken cancellationToken = default)
    {
        var gitRepository = await dbContext
            .Set<GitRepository>()
            .FirstOrDefaultAsync(x => x.Name == repositoryName, cancellationToken);
        if (gitRepository is null)
            return null;

        var issue = await dbContext
            .Set<Issue>()
            .Include(x => x.Assignee)
            .Include(x => x.Author)
            .Include(x => x.ChangeLogs).ThenInclude(x => ((IssueAssigneeChangeLog)x).OldAssignee)
            .Include(x => x.ChangeLogs).ThenInclude(x => ((IssueAssigneeChangeLog)x).NewAssignee)
            .Include(x => x.ChangeLogs).ThenInclude(x => ((IssueLabelChangeLog)x).OldLabel)
            .Include(x => x.ChangeLogs).ThenInclude(x => ((IssueLabelChangeLog)x).NewLabel)
            .Include(x => x.ChangeLogs).ThenInclude(x => ((IssueStatusChangeLog)x).OldStatus)
            .Include(x => x.ChangeLogs).ThenInclude(x => ((IssueStatusChangeLog)x).NewStatus)
            .Include(x => x.Comments)
            .ThenInclude(x => x.Author)
            .Include(x => x.Labels)
            .Include(x => x.Status)
            .AsSingleQuery()
            .FirstOrDefaultAsync(
                i => i.RepositoryId == gitRepository.Id &&
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
        => dbContext.Set<Issue>().Remove(issue);
}