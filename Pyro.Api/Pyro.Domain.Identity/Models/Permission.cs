// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Domain.Identity.Models;

public class Permission
{
    public const string RepositoryView = "repository.view";
    public const string RepositoryEdit = "repository.edit";
    public const string RepositoryManage = "repository.manage";

    public const string UserView = "user.view";
    public const string UserEdit = "user.edit";
    public const string UserManage = "user.manage";

    private readonly List<Role> roles = [];

    // TODO: hide id
    public Guid Id { get; init; }

    public required string Name { get; init; }

    public IReadOnlyCollection<Role> Roles
        => roles;
}