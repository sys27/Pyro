// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Domain.Issues;

public interface IIssueRepository
{
    Task<IReadOnlyList<Issue>> GetIssues(
        string repositoryName,
        CancellationToken cancellationToken = default);

    Task<Issue?> GetIssue(
        string repositoryName,
        int number,
        CancellationToken cancellationToken = default);

    Task AddIssue(
        Issue issue,
        CancellationToken cancellationToken = default);

    void DeleteIssue(Issue issue);

    Task<User?> GetUser(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<User>> GetUsers(
        CancellationToken cancellationToken = default);
}