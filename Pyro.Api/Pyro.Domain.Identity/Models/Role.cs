// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Pyro.Domain.Core.Exceptions;

namespace Pyro.Domain.Identity.Models;

public class Role
{
    public const string Admin = "Admin";

    private readonly List<Permission> permissions = [];
    private readonly List<User> users = [];
    private readonly string name;

    public Guid Id { get; init; }

    public required string Name
    {
        get => name;

        [MemberNotNull(nameof(name))]
        init
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainValidationException("Role name cannot be null or empty.");

            if (value.Length > 50)
                throw new DomainValidationException("Role name must be less than 50 characters.");

            name = value;
        }
    }

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