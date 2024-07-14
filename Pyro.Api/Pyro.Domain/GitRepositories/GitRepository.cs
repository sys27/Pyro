// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Shared.Models;

namespace Pyro.Domain.GitRepositories;

/// <summary>
/// Represents a Git repository entity.
/// </summary>
public class GitRepository : DomainEntity
{
    public Guid Id { get; init; }

    public required string Name { get; init; }

    public string? Description { get; init; }

    public required string DefaultBranch { get; init; }
}