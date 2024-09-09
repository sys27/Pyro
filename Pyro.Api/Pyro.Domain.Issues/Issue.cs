// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Pyro.Domain.Issues.DomainEvents;
using Pyro.Domain.Shared.Exceptions;
using Pyro.Domain.Shared.Models;

namespace Pyro.Domain.Issues;

public class Issue : DomainEntity
{
    private readonly List<IssueComment> comments = [];
    private readonly List<Label> labels = [];
    private readonly List<IssueChangeLog> changeLogs = [];
    private string title;
    private IssueStatus status;

    private Issue()
    {
    }

    public int IssueNumber { get; init; }

    public required string Title
    {
        get => title;
        [MemberNotNull(nameof(title))]
        init => title = value;
    }

    public required IssueStatus Status
    {
        get => status;
        [MemberNotNull(nameof(status))]
        init => SetStatus(value);
    }

    [MemberNotNull(nameof(status))]
    private void SetStatus(IssueStatus value)
    {
        if (value.IsDisabled)
            throw new DomainException($"The issue status (Id: {value.Id}) is disabled");

        PublishEvent(new IssueStatusChanged(this, Status, value));

        status = value;
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

    public IReadOnlyList<IssueChangeLog> ChangeLogs
        => changeLogs;

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

        PublishEvent(new IssueAssigneeChanged(this, Assignee, assignee));

        Assignee = assignee;
    }

    public void AddLabel(Label label)
    {
        if (labels.Any(x => x.Name == label.Name))
            return;

        if (label.IsDisabled)
            throw new DomainException($"The label (Id: {label.Id}) is disabled");

        ThrowIfLocked();

        labels.Add(label);

        PublishEvent(new IssueLabelAdded(this, label));
    }

    public void RemoveLabel(Label label)
    {
        ThrowIfLocked();

        if (labels.Remove(label))
            PublishEvent(new IssueLabelRemoved(this, label));
    }

    public void UpdateLabels(IReadOnlyList<Label> newLabels)
    {
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

        SetStatus(issueStatus);
    }

    public void UpdateTitle(string newTitle)
    {
        if (Title == newTitle)
            return;

        ThrowIfLocked();

        PublishEvent(new IssueTitleChanged(this, title, newTitle));

        title = newTitle;
    }

    public void Lock()
    {
        if (IsLocked)
            return;

        IsLocked = true;

        PublishEvent(new IssueLocked(this));
    }

    public void Unlock()
    {
        if (!IsLocked)
            return;

        IsLocked = false;

        PublishEvent(new IssueUnlocked(this));
    }

    private void ThrowIfLocked()
    {
        if (IsLocked)
            throw new DomainException($"The issue (Id: {Id}) is locked");
    }

    public void AddChangeLog(IssueChangeLog changeLog)
        => changeLogs.Add(changeLog);

    public sealed class Builder
    {
        private readonly TimeProvider timeProvider;
        private string? title;
        private int issueNumber;
        private IssueStatus? status;
        private GitRepository? repository;
        private User? author;
        private string? initialComment;

        public Builder(TimeProvider timeProvider)
            => this.timeProvider = timeProvider;

        public Builder WithTitle(string title)
        {
            this.title = title;

            return this;
        }

        public Builder WithIssueNumber(int issueNumber)
        {
            this.issueNumber = issueNumber;

            return this;
        }

        public Builder WithStatus(IssueStatus status)
        {
            this.status = status;

            return this;
        }

        public Builder WithRepository(GitRepository repository)
        {
            this.repository = repository;

            return this;
        }

        public Builder WithAuthor(User author)
        {
            this.author = author;

            return this;
        }

        public Builder WithInitialComment(string initialComment)
        {
            this.initialComment = initialComment;

            return this;
        }

        public Issue Build()
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new InvalidOperationException("The title is not set");

            if (status is null)
                throw new InvalidOperationException("The status is not set");

            if (repository is null)
                throw new InvalidOperationException("The repository is not set");

            if (author is null)
                throw new InvalidOperationException("The author is not set");

            if (string.IsNullOrWhiteSpace(initialComment))
                throw new InvalidOperationException("The initial comment is not set");

            var issue = new Issue
            {
                Title = title,
                IssueNumber = issueNumber,
                Status = status,
                RepositoryId = repository.Id,
                Author = author,
                CreatedAt = timeProvider.GetUtcNow(),
            };
            issue.AddComment(initialComment, author, issue.CreatedAt);

            return issue;
        }
    }
}