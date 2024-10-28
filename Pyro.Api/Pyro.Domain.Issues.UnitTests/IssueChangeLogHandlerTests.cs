// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using NSubstitute;
using Pyro.Domain.Issues.DomainEvents;
using Pyro.Domain.Shared.CurrentUserProvider;

namespace Pyro.Domain.Issues.UnitTests;

public class IssueChangeLogHandlerTests
{
    private static Issue GetIssue()
    {
        var user = new User(Guid.NewGuid(), "user");
        var gitRepository = new GitRepository
        {
            Name = "test",
        };
        var issueStatus = new IssueStatus
        {
            Name = "status",
            Color = 0,
            Repository = gitRepository,
        };

        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider
            .GetUtcNow()
            .Returns(DateTimeOffset.Now);
        var issue = new Issue.Builder(timeProvider)
            .WithTitle("title")
            .WithStatus(issueStatus)
            .WithRepository(gitRepository)
            .WithAuthor(user)
            .WithInitialComment("comment")
            .Build();

        return issue;
    }

    private static IssueStatus GetStatus()
        => new IssueStatus
        {
            Name = $"status{Random.Shared.Next()}",
            Color = 0,
            Repository = new GitRepository
            {
                Name = "test",
            },
        };

    [Test]
    public async Task HandleIssueLocked()
    {
        var currentUser = new CurrentUser(Guid.NewGuid(), "user", [], []);
        var user = new User(currentUser.Id, currentUser.Login);
        var currentDateTime = new DateTimeOffset(2024, 01, 01, 01, 00, 00, TimeSpan.Zero);

        var currentUserProvider = Substitute.For<ICurrentUserProvider>();
        currentUserProvider
            .GetCurrentUser()
            .Returns(currentUser);
        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider
            .GetUtcNow()
            .Returns(currentDateTime);
        var repository = Substitute.For<IGitRepositoryRepository>();
        repository
            .GetUser(currentUser.Id, Arg.Any<CancellationToken>())
            .Returns(user);

        var handler = new IssueChangeLogHandler(currentUserProvider, timeProvider, repository);

        var issue = GetIssue();
        var domainEvent = new IssueLocked(issue);
        await handler.Handle(domainEvent);

        Assert.Multiple(() =>
        {
            Assert.That(issue.ChangeLogs, Has.Count.EqualTo(1));
            Assert.That(issue.ChangeLogs[0], Is.InstanceOf<IssueLockChangeLog>());

            var changeLog = (IssueLockChangeLog)issue.ChangeLogs[0];
            Assert.That(changeLog, Is.Not.Null);
            Assert.That(changeLog.OldValue, Is.False);
            Assert.That(changeLog.NewValue, Is.True);
            Assert.That(changeLog.Issue, Is.EqualTo(issue));
            Assert.That(changeLog.Author, Is.EqualTo(user));
            Assert.That(changeLog.CreatedAt, Is.EqualTo(currentDateTime));
        });
    }

    [Test]
    public async Task HandleIssueUnlocked()
    {
        var currentUser = new CurrentUser(Guid.NewGuid(), "user", [], []);
        var user = new User(currentUser.Id, currentUser.Login);
        var currentDateTime = new DateTimeOffset(2024, 01, 01, 01, 00, 00, TimeSpan.Zero);

        var currentUserProvider = Substitute.For<ICurrentUserProvider>();
        currentUserProvider
            .GetCurrentUser()
            .Returns(currentUser);
        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider
            .GetUtcNow()
            .Returns(currentDateTime);
        var repository = Substitute.For<IGitRepositoryRepository>();
        repository
            .GetUser(currentUser.Id, Arg.Any<CancellationToken>())
            .Returns(user);

        var handler = new IssueChangeLogHandler(currentUserProvider, timeProvider, repository);

        var issue = GetIssue();
        var domainEvent = new IssueUnlocked(issue);
        await handler.Handle(domainEvent);

        Assert.Multiple(() =>
        {
            Assert.That(issue.ChangeLogs, Has.Count.EqualTo(1));
            Assert.That(issue.ChangeLogs[0], Is.InstanceOf<IssueLockChangeLog>());

            var changeLog = (IssueLockChangeLog)issue.ChangeLogs[0];
            Assert.That(changeLog, Is.Not.Null);
            Assert.That(changeLog.OldValue, Is.True);
            Assert.That(changeLog.NewValue, Is.False);
            Assert.That(changeLog.Issue, Is.EqualTo(issue));
            Assert.That(changeLog.Author, Is.EqualTo(user));
            Assert.That(changeLog.CreatedAt, Is.EqualTo(currentDateTime));
        });
    }

    [Test]
    public async Task HandleIssueStatusChanged()
    {
        var currentUser = new CurrentUser(Guid.NewGuid(), "user", [], []);
        var user = new User(currentUser.Id, currentUser.Login);
        var currentDateTime = new DateTimeOffset(2024, 01, 01, 01, 00, 00, TimeSpan.Zero);

        var currentUserProvider = Substitute.For<ICurrentUserProvider>();
        currentUserProvider
            .GetCurrentUser()
            .Returns(currentUser);
        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider
            .GetUtcNow()
            .Returns(currentDateTime);
        var repository = Substitute.For<IGitRepositoryRepository>();
        repository
            .GetUser(currentUser.Id, Arg.Any<CancellationToken>())
            .Returns(user);

        var handler = new IssueChangeLogHandler(currentUserProvider, timeProvider, repository);

        var issue = GetIssue();
        var oldStatus = GetStatus();
        var newStatus = GetStatus();
        var domainEvent = new IssueStatusChanged(issue, oldStatus, newStatus);
        await handler.Handle(domainEvent);

        Assert.Multiple(() =>
        {
            Assert.That(issue.ChangeLogs, Has.Count.EqualTo(1));
            Assert.That(issue.ChangeLogs[0], Is.InstanceOf<IssueStatusChangeLog>());

            var changeLog = (IssueStatusChangeLog)issue.ChangeLogs[0];
            Assert.That(changeLog, Is.Not.Null);
            Assert.That(changeLog.OldStatus, Is.EqualTo(oldStatus));
            Assert.That(changeLog.NewStatus, Is.EqualTo(newStatus));
            Assert.That(changeLog.Issue, Is.EqualTo(issue));
            Assert.That(changeLog.Author, Is.EqualTo(user));
            Assert.That(changeLog.CreatedAt, Is.EqualTo(currentDateTime));
        });
    }

    [Test]
    public async Task HandleIssueLabelAdded()
    {
        var currentUser = new CurrentUser(Guid.NewGuid(), "user", [], []);
        var user = new User(currentUser.Id, currentUser.Login);
        var currentDateTime = new DateTimeOffset(2024, 01, 01, 01, 00, 00, TimeSpan.Zero);

        var currentUserProvider = Substitute.For<ICurrentUserProvider>();
        currentUserProvider
            .GetCurrentUser()
            .Returns(currentUser);
        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider
            .GetUtcNow()
            .Returns(currentDateTime);
        var repository = Substitute.For<IGitRepositoryRepository>();
        repository
            .GetUser(currentUser.Id, Arg.Any<CancellationToken>())
            .Returns(user);

        var handler = new IssueChangeLogHandler(currentUserProvider, timeProvider, repository);

        var issue = GetIssue();
        var label = new Label
        {
            Name = "label",
            Color = 0,
        };
        var domainEvent = new IssueLabelAdded(issue, label);
        await handler.Handle(domainEvent);

        Assert.Multiple(() =>
        {
            Assert.That(issue.ChangeLogs, Has.Count.EqualTo(1));
            Assert.That(issue.ChangeLogs[0], Is.InstanceOf<IssueLabelChangeLog>());

            var changeLog = (IssueLabelChangeLog)issue.ChangeLogs[0];
            Assert.That(changeLog, Is.Not.Null);
            Assert.That(changeLog.OldLabel, Is.Null);
            Assert.That(changeLog.NewLabel, Is.EqualTo(label));
            Assert.That(changeLog.Issue, Is.EqualTo(issue));
            Assert.That(changeLog.Author, Is.EqualTo(user));
            Assert.That(changeLog.CreatedAt, Is.EqualTo(currentDateTime));
        });
    }

    [Test]
    public async Task HandleIssueLabelRemoved()
    {
        var currentUser = new CurrentUser(Guid.NewGuid(), "user", [], []);
        var user = new User(currentUser.Id, currentUser.Login);
        var currentDateTime = new DateTimeOffset(2024, 01, 01, 01, 00, 00, TimeSpan.Zero);

        var currentUserProvider = Substitute.For<ICurrentUserProvider>();
        currentUserProvider
            .GetCurrentUser()
            .Returns(currentUser);
        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider
            .GetUtcNow()
            .Returns(currentDateTime);
        var repository = Substitute.For<IGitRepositoryRepository>();
        repository
            .GetUser(currentUser.Id, Arg.Any<CancellationToken>())
            .Returns(user);

        var handler = new IssueChangeLogHandler(currentUserProvider, timeProvider, repository);

        var issue = GetIssue();
        var label = new Label
        {
            Name = "label",
            Color = 0,
        };
        var domainEvent = new IssueLabelRemoved(issue, label);
        await handler.Handle(domainEvent);

        Assert.Multiple(() =>
        {
            Assert.That(issue.ChangeLogs, Has.Count.EqualTo(1));
            Assert.That(issue.ChangeLogs[0], Is.InstanceOf<IssueLabelChangeLog>());

            var changeLog = (IssueLabelChangeLog)issue.ChangeLogs[0];
            Assert.That(changeLog, Is.Not.Null);
            Assert.That(changeLog.OldLabel, Is.EqualTo(label));
            Assert.That(changeLog.NewLabel, Is.Null);
            Assert.That(changeLog.Issue, Is.EqualTo(issue));
            Assert.That(changeLog.Author, Is.EqualTo(user));
            Assert.That(changeLog.CreatedAt, Is.EqualTo(currentDateTime));
        });
    }

    [Test]
    public async Task HandleIssueAssigneeChanged()
    {
        var currentUser = new CurrentUser(Guid.NewGuid(), "user", [], []);
        var user = new User(currentUser.Id, currentUser.Login);
        var currentDateTime = new DateTimeOffset(2024, 01, 01, 01, 00, 00, TimeSpan.Zero);

        var currentUserProvider = Substitute.For<ICurrentUserProvider>();
        currentUserProvider
            .GetCurrentUser()
            .Returns(currentUser);
        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider
            .GetUtcNow()
            .Returns(currentDateTime);
        var repository = Substitute.For<IGitRepositoryRepository>();
        repository
            .GetUser(currentUser.Id, Arg.Any<CancellationToken>())
            .Returns(user);

        var handler = new IssueChangeLogHandler(currentUserProvider, timeProvider, repository);

        var issue = GetIssue();
        var oldAssignee = new User(Guid.NewGuid(), "xxx");
        var newAssignee = new User(Guid.NewGuid(), "yyy");
        var domainEvent = new IssueAssigneeChanged(issue, oldAssignee, newAssignee);
        await handler.Handle(domainEvent);

        Assert.Multiple(() =>
        {
            Assert.That(issue.ChangeLogs, Has.Count.EqualTo(1));
            Assert.That(issue.ChangeLogs[0], Is.InstanceOf<IssueAssigneeChangeLog>());

            var changeLog = (IssueAssigneeChangeLog)issue.ChangeLogs[0];
            Assert.That(changeLog, Is.Not.Null);
            Assert.That(changeLog.OldAssignee, Is.EqualTo(oldAssignee));
            Assert.That(changeLog.NewAssignee, Is.EqualTo(newAssignee));
            Assert.That(changeLog.Issue, Is.EqualTo(issue));
            Assert.That(changeLog.Author, Is.EqualTo(user));
            Assert.That(changeLog.CreatedAt, Is.EqualTo(currentDateTime));
        });
    }

    [Test]
    public async Task HandleIssueTitleChanged()
    {
        var currentUser = new CurrentUser(Guid.NewGuid(), "user", [], []);
        var user = new User(currentUser.Id, currentUser.Login);
        var currentDateTime = new DateTimeOffset(2024, 01, 01, 01, 00, 00, TimeSpan.Zero);

        var currentUserProvider = Substitute.For<ICurrentUserProvider>();
        currentUserProvider
            .GetCurrentUser()
            .Returns(currentUser);
        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider
            .GetUtcNow()
            .Returns(currentDateTime);
        var repository = Substitute.For<IGitRepositoryRepository>();
        repository
            .GetUser(currentUser.Id, Arg.Any<CancellationToken>())
            .Returns(user);

        var handler = new IssueChangeLogHandler(currentUserProvider, timeProvider, repository);

        var issue = GetIssue();
        const string oldTitle = "xxx";
        const string newTitle = "yyy";
        var domainEvent = new IssueTitleChanged(issue, oldTitle, newTitle);
        await handler.Handle(domainEvent);

        Assert.Multiple(() =>
        {
            Assert.That(issue.ChangeLogs, Has.Count.EqualTo(1));
            Assert.That(issue.ChangeLogs[0], Is.InstanceOf<IssueTitleChangeLog>());

            var changeLog = (IssueTitleChangeLog)issue.ChangeLogs[0];
            Assert.That(changeLog, Is.Not.Null);
            Assert.That(changeLog.OldTitle, Is.EqualTo(oldTitle));
            Assert.That(changeLog.NewTitle, Is.EqualTo(newTitle));
            Assert.That(changeLog.Issue, Is.EqualTo(issue));
            Assert.That(changeLog.Author, Is.EqualTo(user));
            Assert.That(changeLog.CreatedAt, Is.EqualTo(currentDateTime));
        });
    }
}