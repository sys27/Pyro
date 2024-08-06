// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Shared.Models;

namespace Pyro.Domain.Identity.Models;

public class Permission : Entity
{
    public const string RepositoryView = "repository.view";
    public const string RepositoryEdit = "repository.edit";
    public const string RepositoryManage = "repository.manage";

    public const string UserView = "user.view";
    public const string UserEdit = "user.edit";
    public const string UserManage = "user.manage";

    public const string IssueView = "issue.view";
    public const string IssueEdit = "issue.edit";
    public const string IssueManage = "issue.manage";

    private readonly List<Role> roles = [];

    public required string Name { get; init; }

    public IReadOnlyCollection<Role> Roles
        => roles;
}