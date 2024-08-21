// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Shared.Exceptions;
using Pyro.Domain.Shared.Models;

namespace Pyro.Domain.Issues;

public class GitRepository : Entity
{
    private readonly List<IssueStatus> issueStatuses = [];

    public required string Name { get; init; }

    public IReadOnlyList<Label> Labels { get; init; } = new List<Label>();

    public IReadOnlyList<IssueStatus> IssueStatuses
        => issueStatuses;

    public Label? GetLabel(Guid labelId)
        => Labels.FirstOrDefault(x => x.Id == labelId);

    public IssueStatus? GetIssueStatus(Guid statusId)
        => IssueStatuses.FirstOrDefault(x => x.Id == statusId);

    public IssueStatus AddIssueStatus(string name, int color)
    {
        if (issueStatuses.Any(x => x.Name == name))
            throw new DomainException($"The status (Name: {name}) already exists");

        var status = new IssueStatus
        {
            Name = name,
            Repository = this,
            Color = color,
        };
        issueStatuses.Add(status);

        return status;
    }

    public void DeleteIssueStatus(IssueStatus status)
    {
        status.RemoveTransitions();

        issueStatuses.Remove(status);
    }

    public IReadOnlyList<IssueStatusTransition> GetTransitions()
        => IssueStatuses.SelectMany(x => x.FromTransitions)
            .Concat(IssueStatuses.SelectMany(x => x.ToTransitions))
            .DistinctBy(x => x.Id)
            .ToList();

    public IssueStatusTransition? GetTransition(Guid id)
        => GetTransitions().FirstOrDefault(x => x.Id == id);

    public void DeleteTransition(IssueStatusTransition transition)
    {
        foreach (var status in IssueStatuses)
            status.RemoveTransition(transition);
    }
}