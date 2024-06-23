// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.GitRepositories;

namespace Pyro.Domain.Git;

public interface IGitService
{
    Task InitializeRepository(
        GitRepository repository,
        CancellationToken cancellationToken = default);

    DirectoryView GetDirectoryView(GitRepository repository);
}