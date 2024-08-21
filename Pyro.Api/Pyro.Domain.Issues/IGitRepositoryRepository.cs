// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Domain.Issues;

public interface IGitRepositoryRepository
{
    Task<GitRepository?> GetRepository(string name, CancellationToken cancellationToken = default);

    Task<User?> GetUser(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<User>> GetUsers(CancellationToken cancellationToken = default);
}