// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Identity.Models;
using Pyro.Domain.Identity.Queries;

namespace Pyro.Domain.Identity;

public interface IUserRepository
{
    Task<IReadOnlyList<User>> GetUsers(
        GetUsers request,
        CancellationToken cancellationToken = default);

    Task<User?> GetUserById(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<User?> GetUserByLogin(
        string login,
        CancellationToken cancellationToken = default);

    Task<User?> GetUserByToken(
        string token,
        CancellationToken cancellationToken = default);

    Task<User> AddUser(User user, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Role>> GetRolesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Permission>> GetPermissionsAsync(CancellationToken cancellationToken = default);
}