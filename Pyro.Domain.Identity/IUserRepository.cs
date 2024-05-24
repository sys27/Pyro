// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Identity.Models;

namespace Pyro.Domain.Identity;

public interface IUserRepository
{
    Task<User?> GetUserById(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<User?> GetUserByEmail(
        string email,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Role>> GetRolesAsync(CancellationToken cancellationToken);

    Task<IEnumerable<Permission>> GetPermissionsAsync(CancellationToken cancellationToken);
}