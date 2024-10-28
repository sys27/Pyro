// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Shared.Entities;

namespace Pyro.Domain.Issues;

public class IssueStatus : Entity
{
    private readonly List<IssueStatusTransition> fromTransitions = [];
    private readonly List<IssueStatusTransition> toTransitions = [];

    public required string Name { get; set; }

    public required int Color { get; set; }

    public required GitRepository Repository { get; init; }

    public bool IsDisabled { get; set; }

    public IReadOnlyList<IssueStatusTransition> FromTransitions
        => fromTransitions;

    public IReadOnlyList<IssueStatusTransition> ToTransitions
        => toTransitions;

    public void RemoveTransitions()
    {
        fromTransitions.Clear();
        toTransitions.Clear();
    }

    public IssueStatusTransition? GetTransition(Guid id)
        => fromTransitions.FirstOrDefault(x => x.Id == id);

    public IssueStatusTransition AddTransition(IssueStatus toStatus)
    {
        var translation = new IssueStatusTransition
        {
            From = this,
            To = toStatus,
        };
        fromTransitions.Add(translation);
        toStatus.toTransitions.Add(translation);

        return translation;
    }

    public void RemoveTransition(IssueStatusTransition transition)
    {
        fromTransitions.Remove(transition);
        transition.To.toTransitions.Remove(transition);
    }

    public bool CanTransitionTo(IssueStatus status)
        => Id == status.Id || fromTransitions.Any(x => x.To.Id == status.Id);
}