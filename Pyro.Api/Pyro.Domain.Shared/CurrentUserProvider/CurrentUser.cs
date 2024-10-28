// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Domain.Shared.CurrentUserProvider;

public record CurrentUser(
    Guid Id,
    string Login,
    IEnumerable<string> Roles,
    IEnumerable<string> Permissions)
{
    public bool HasPermission(string permission)
        => Permissions.Contains(permission);
}