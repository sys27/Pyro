// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;
using Pyro.Domain.Shared;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Issues.DomainEvents;

public class IssueChangeLogHandler :
    INotificationHandler<IssueLocked>,
    INotificationHandler<IssueUnlocked>,
    INotificationHandler<IssueStatusChanged>,
    INotificationHandler<IssueLabelAdded>,
    INotificationHandler<IssueLabelRemoved>,
    INotificationHandler<IssueAssigneeChanged>,
    INotificationHandler<IssueTitleChanged>
{
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly TimeProvider timeProvider;
    private readonly IGitRepositoryRepository repository;

    public IssueChangeLogHandler(
        ICurrentUserProvider currentUserProvider,
        TimeProvider timeProvider,
        IGitRepositoryRepository repository)
    {
        this.currentUserProvider = currentUserProvider;
        this.timeProvider = timeProvider;
        this.repository = repository;
    }

    private async Task<User> GetAuthor(CancellationToken cancellationToken)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        var author = await repository.GetUser(currentUser.Id, cancellationToken) ??
                     throw new NotFoundException($"The user (Id: {currentUser.Id}) not found");

        return author;
    }

    public async Task Handle(IssueLocked notification, CancellationToken cancellationToken = default)
    {
        var author = await GetAuthor(cancellationToken);
        var issue = notification.Issue;
        var changeLog = new IssueLockChangeLog(false, true)
        {
            Issue = issue,
            Author = author,
            CreatedAt = timeProvider.GetUtcNow(),
        };

        issue.AddChangeLog(changeLog);
    }

    public async Task Handle(IssueUnlocked notification, CancellationToken cancellationToken = default)
    {
        var author = await GetAuthor(cancellationToken);
        var issue = notification.Issue;
        var changeLog = new IssueLockChangeLog(true, false)
        {
            Issue = issue,
            Author = author,
            CreatedAt = timeProvider.GetUtcNow(),
        };

        issue.AddChangeLog(changeLog);
    }

    public async Task Handle(IssueStatusChanged notification, CancellationToken cancellationToken = default)
    {
        var author = await GetAuthor(cancellationToken);
        var issue = notification.Issue;
        var changeLog = new IssueStatusChangeLog(notification.OldStatus, notification.NewStatus)
        {
            Issue = issue,
            Author = author,
            CreatedAt = timeProvider.GetUtcNow(),
        };

        issue.AddChangeLog(changeLog);
    }

    public async Task Handle(IssueLabelAdded notification, CancellationToken cancellationToken = default)
    {
        var author = await GetAuthor(cancellationToken);
        var issue = notification.Issue;
        var changeLog = new IssueLabelChangeLog(null, notification.Label)
        {
            Issue = issue,
            Author = author,
            CreatedAt = timeProvider.GetUtcNow(),
        };

        issue.AddChangeLog(changeLog);
    }

    public async Task Handle(IssueLabelRemoved notification, CancellationToken cancellationToken = default)
    {
        var author = await GetAuthor(cancellationToken);
        var issue = notification.Issue;
        var changeLog = new IssueLabelChangeLog(notification.Label, null)
        {
            Issue = issue,
            Author = author,
            CreatedAt = timeProvider.GetUtcNow(),
        };

        issue.AddChangeLog(changeLog);
    }

    public async Task Handle(IssueAssigneeChanged notification, CancellationToken cancellationToken = default)
    {
        var author = await GetAuthor(cancellationToken);
        var issue = notification.Issue;
        var changeLog = new IssueAssigneeChangeLog(notification.OldAssignee, notification.NewAssignee)
        {
            Issue = issue,
            Author = author,
            CreatedAt = timeProvider.GetUtcNow(),
        };

        issue.AddChangeLog(changeLog);
    }

    public async Task Handle(IssueTitleChanged notification, CancellationToken cancellationToken = default)
    {
        var author = await GetAuthor(cancellationToken);
        var issue = notification.Issue;
        var changeLog = new IssueTitleChangeLog(notification.OldTitle, notification.NewTitle)
        {
            Issue = issue,
            Author = author,
            CreatedAt = timeProvider.GetUtcNow(),
        };

        issue.AddChangeLog(changeLog);
    }
}