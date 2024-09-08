// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Pyro.Domain.Shared.Exceptions;
using Pyro.Domain.Shared.Models;

namespace Pyro.Domain.Issues;

public class Issue : Entity
{
    private readonly List<IssueComment> comments = [];
    private readonly List<Label> labels = [];
    private IssueStatus status;

    public int IssueNumber { get; init; }

    public required string Title { get; set; }

    public required IssueStatus Status
    {
        get => status;
        [MemberNotNull(nameof(status))]
        init => status = value;
    }

    public required Guid RepositoryId { get; init; }

    public required User Author { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }

    public User? Assignee { get; private set; }

    public bool IsLocked { get; private set; }

    public IReadOnlyList<IssueComment> Comments
        => comments;

    public IReadOnlyList<Label> Labels
        => labels;

    public IssueComment? GetComment(Guid commentId)
        => comments.FirstOrDefault(x => x.Id == commentId);

    public IssueComment AddComment(string content, User author, DateTimeOffset createdAt)
    {
        ThrowIfLocked();

        var comment = new IssueComment
        {
            Issue = this,
            Content = content,
            Author = author,
            CreatedAt = createdAt,
        };

        comments.Add(comment);

        return comment;
    }

    public void DeleteComment(IssueComment comment)
    {
        ThrowIfLocked();

        comments.Remove(comment);
    }

    public void AssignTo(User? assignee)
    {
        if (assignee?.Id == Assignee?.Id)
            return;

        ThrowIfLocked();

        Assignee = assignee;
    }

    public void AddLabel(Label label)
    {
        if (labels.Any(x => x.Name == label.Name))
            return;

        ThrowIfLocked();

        labels.Add(label);
    }

    public void RemoveLabel(Label label)
    {
        ThrowIfLocked();

        labels.Remove(label);
    }

    private void ClearLabels()
    {
        ThrowIfLocked();

        labels.Clear();
    }

    public void UpdateLabels(IReadOnlyList<Label> newLabels)
    {
        if (newLabels.Count == 0)
        {
            ClearLabels();
            return;
        }

        var labelsToRemove = labels.Except(newLabels).ToList();
        foreach (var label in labelsToRemove)
            RemoveLabel(label);

        var labelsToAdd = newLabels.Except(labels).ToList();
        foreach (var label in labelsToAdd)
            AddLabel(label);
    }

    public void TransitionTo(Guid statusId, GitRepository repository)
    {
        if (Status.Id == statusId)
            return;

        ThrowIfLocked();

        if (repository.Id != RepositoryId)
            throw new DomainException("The issue does not belong to the repository");

        var issueStatus = repository.GetIssueStatus(statusId) ??
                          throw new NotFoundException($"The issue status (Id: {statusId}) not found");

        if (!Status.CanTransitionTo(issueStatus))
            throw new DomainException($"The issue cannot be transitioned from '{Status.Name}' to '{issueStatus.Name}'");

        status = issueStatus;
    }

    public void Lock()
        => IsLocked = true;

    public void Unlock()
        => IsLocked = false;

    private void ThrowIfLocked()
    {
        if (IsLocked)
            throw new DomainException($"The issue (Id: {Id}) is locked");
    }
}