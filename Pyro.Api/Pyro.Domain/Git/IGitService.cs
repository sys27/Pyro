// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.GitRepositories;

namespace Pyro.Domain.Git;

public interface IGitService
{
    Task InitializeRepository(
        GitRepository gitRepo,
        CancellationToken cancellationToken = default);

    IReadOnlyList<BranchItem> GetBranches(GitRepository gitRepo);

    TreeView GetTreeView(GitRepository gitRepo, string? branchOrPath = null);

    GitFile GetFile(GitRepository gitRepo, string branchAndPath);
}