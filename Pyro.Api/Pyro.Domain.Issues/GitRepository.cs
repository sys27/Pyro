// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Shared.Models;

namespace Pyro.Domain.Issues;

public class GitRepository : Entity
{
    public required string Name { get; init; }

    public IReadOnlyList<Tag> Tags { get; init; } = new List<Tag>();

    public Tag? GetTag(Guid tagId)
        => Tags.FirstOrDefault(x => x.Id == tagId);
}