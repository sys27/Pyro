// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Pyro.Domain.Core.Exceptions;
using Pyro.Domain.Core.Models;

namespace Pyro.Domain.GitRepositories;

/// <summary>
/// Represents a Git repository entity.
/// </summary>
public class GitRepository : DomainEntity
{
    private readonly string name;
    private readonly string? description;
    private readonly string defaultBranch;

    public Guid Id { get; init; }

    public required string Name
    {
        get => name;

        [MemberNotNull(nameof(name))]
        init
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainValidationException("Name cannot be empty.");

            if (value.Length > 20)
                throw new DomainValidationException("Name is too long.");

            if (!Regexes.GetRepositoryNameRegex().IsMatch(value))
                throw new DomainValidationException("Name can only contain letters, numbers, hyphens, and underscores.");

            if (name != value)
                PublishEvent(new GitRepositoryNameChanged(name, value));

            name = value;
        }
    }

    public string? Description
    {
        get => description;
        init
        {
            if (value?.Length > 250)
                throw new DomainValidationException("Description is too long.");

            description = value;
        }
    }

    public required string DefaultBranch
    {
        get => defaultBranch;

        [MemberNotNull(nameof(defaultBranch))]
        init
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainValidationException("Default Branch cannot be empty.");

            if (value.Length > 50)
                throw new DomainValidationException("Default Branch is too long.");

            defaultBranch = value;
        }
    }
}