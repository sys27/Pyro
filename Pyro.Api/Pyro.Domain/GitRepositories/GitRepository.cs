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
    private readonly List<Tag> tags = [];

    public required string Name { get; init; }

    public string? Description { get; init; }

    public required string DefaultBranch { get; init; }

    public GitRepositoryStatus Status { get; private set; } = GitRepositoryStatus.New;

    public bool IsNew
        => Status == GitRepositoryStatus.New;

    public IReadOnlyList<Tag> Tags
        => tags;

    public void MarkAsInitialized()
        => Status = GitRepositoryStatus.Initialized;

    public Tag? GetTag(Guid id)
        => tags.FirstOrDefault(x => x.Id == id);

    public Tag AddTag(string name, int color)
    {
        if (tags.Any(x => x.Name == name))
            throw new DomainException($"Tag with name '{name}' already exists.");

        var tag = new Tag
        {
            Name = name,
            Color = color,
            GitRepository = this,
        };
        tags.Add(tag);

        return tag;
    }

    public void RemoveTag(Guid id)
    {
        var tag = tags.FirstOrDefault(x => x.Id == id) ??
                  throw new DomainException($"The tag (Id: {id}) does not exist.");

        tags.Remove(tag);

        PublishEvent(new TagDeleted(tag.Id));
    }
}