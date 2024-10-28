// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Shared.Entities;

namespace Pyro.Domain.Issues;

public class IssueStatusTransition : Entity
{
    public required IssueStatus From { get; init; }

    public required IssueStatus To { get; init; }
}