// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Shared.Entities;

namespace Pyro.Domain.Issues;

public class IssueComment : Entity
{
    public required string Content { get; set; }

    public required Issue Issue { get; init; }

    public required User Author { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }
}