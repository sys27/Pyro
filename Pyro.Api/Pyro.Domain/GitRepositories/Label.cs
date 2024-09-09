// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Shared.Models;

namespace Pyro.Domain.GitRepositories;

public class Label : Entity
{
    public required string Name { get; set; }

    public required int Color { get; set; }

    public required GitRepository GitRepository { get; init; }

    public bool IsDisabled { get; set; }
}