// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Issues.Queries;

namespace Pyro.Domain.Issues;

public interface IIssueRepository
{
    Task<IReadOnlyList<Issue>> GetIssues(
        GetIssues query,
        CancellationToken cancellationToken = default);

    Task<Issue?> GetIssue(
        string repositoryName,
        int number,
        CancellationToken cancellationToken = default);

    Task AddIssue(
        Issue issue,
        CancellationToken cancellationToken = default);

    void DeleteIssue(Issue issue);
}