// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Shared.Exceptions;
using Pyro.Domain.Shared.Models;

namespace Pyro.Domain.GitRepositories;

/// <summary>
/// Represents a Git repository entity.
/// </summary>
public class GitRepository : DomainEntity
{
    private readonly List<Label> labels = [];

    public required string Name { get; init; }

    public string? Description { get; init; }

    public required string DefaultBranch { get; init; }

    public GitRepositoryStatus Status { get; private set; } = GitRepositoryStatus.New;

    public bool IsNew
        => Status == GitRepositoryStatus.New;

    public IReadOnlyList<Label> Labels
        => labels;

    public void MarkAsInitialized()
        => Status = GitRepositoryStatus.Initialized;

    public Label? GetLabel(Guid id)
        => labels.FirstOrDefault(x => x.Id == id);

    public Label AddLabel(string name, int color)
    {
        if (labels.Any(x => x.Name == name))
            throw new DomainException($"Label with name '{name}' already exists.");

        var label = new Label
        {
            Name = name,
            Color = color,
            GitRepository = this,
        };
        labels.Add(label);

        return label;
    }

    public void RemoveLabel(Guid id)
    {
        var label = labels.FirstOrDefault(x => x.Id == id) ??
                    throw new DomainException($"The label (Id: {id}) does not exist.");

        labels.Remove(label);

        PublishEvent(new LabelDeleted(label.Id));
    }
}