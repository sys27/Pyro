// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Pyro.Domain.Core.Models;

namespace Pyro.Domain.GitRepositories;

/// <summary>
/// Represents a Git repository entity.
/// </summary>
public class GitRepository : DomainEntity
{
    private static Regex regexName = new Regex("^[a-zA-Z0-9-_]+$", RegexOptions.Compiled);

    private readonly string name;

    public Guid Id { get; init; } = Guid.NewGuid();

    public required string Name
    {
        get => name;

        [MemberNotNull(nameof(name))]
        init
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException("Name cannot be empty.");

            if (value.Length > 20)
                throw new InvalidOperationException("Name is too long.");

            if (!regexName.IsMatch(value))
                throw new InvalidOperationException("Name can only contain letters, numbers, hyphens, and underscores.");

            if (name != value)
                PublishEvent(new GitRepositoryNameChanged(name, value));

            name = value;
        }
    }
}