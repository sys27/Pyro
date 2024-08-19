// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Shared.Models;

namespace Pyro.Domain.Issues;

public class GitRepository : Entity
{
    public required string Name { get; init; }

    public IReadOnlyList<Label> Labels { get; init; } = new List<Label>();

    public Label? GetLabel(Guid labelId)
        => Labels.FirstOrDefault(x => x.Id == labelId);
}