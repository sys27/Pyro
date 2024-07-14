// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Domain.Identity.Models;

public class Role
{
    public const string Admin = "Admin";

    private readonly List<Permission> permissions = [];
    private readonly List<User> users = [];

    public Guid Id { get; init; }

    public required string Name { get; init; }

    public IReadOnlyCollection<Permission> Permissions
        => permissions;

    public IReadOnlyCollection<User> Users
        => users;

    public void AddPermission(Permission permission)
    {
        if (permissions.Any(x => x.Name == permission.Name))
            return;

        permissions.Add(permission);
    }
}