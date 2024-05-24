// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Domain.GitRepositories;

public interface IGitRepositoryRepository
{
    Task<GitRepository?> GetGitRepository(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<GitRepository?> GetGitRepository(
        string name,
        CancellationToken cancellationToken = default);

    Task<GitRepository> AddGitRepository(
        GitRepository gitRepository,
        CancellationToken cancellationToken = default);
}