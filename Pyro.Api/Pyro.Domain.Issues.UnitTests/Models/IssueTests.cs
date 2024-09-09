// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using NSubstitute;
using Pyro.Domain.Issues.DomainEvents;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Issues.UnitTests.Models;

public class IssueTests
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

    [Test]
    public void AssignToTest()
    {
        var user = new User(Guid.NewGuid(), "user");
        var issue = GetIssue();
        issue.AssignTo(user);

        Assert.Multiple(() =>
        {
            Assert.That(issue.Assignee, Is.EqualTo(user));
            Assert.That(issue.DomainEvents, Has.One.EqualTo(new IssueAssigneeChanged(issue, null, user)));
        });
    }

    [Test]
    public void AddExistingLabel()
    {
        var issue = GetIssue();

        var label = new Label
        {
            Id = Guid.NewGuid(),
            Name = "tag",
            Color = 0,
        };
        issue.AddLabel(label);
        issue.AddLabel(label);

        Assert.Multiple(() =>
        {
            Assert.That(issue.Labels, Has.Count.EqualTo(1));
            Assert.That(issue.Labels, Has.One.EqualTo(label));
            Assert.That(issue.DomainEvents, Has.One.EqualTo(new IssueLabelAdded(issue, label)));
        });
    }

    [Test]
    public void AddDisabledLabel()
    {
        var issue = GetIssue();
        var label = new Label
        {
            Id = Guid.NewGuid(),
            Name = "tag",
            Color = 0,
            IsDisabled = true,
        };

        Assert.Throws<DomainException>(() => issue.AddLabel(label));
    }

    [Test]
    public void RemoveLabel()
    {
        var issue = GetIssue();
        var label = new Label
        {
            Id = Guid.NewGuid(),
            Name = "tag",
            Color = 0,
        };
        issue.AddLabel(label);
        issue.RemoveLabel(label);

        Assert.Multiple(() =>
        {
            Assert.That(issue.Labels, Is.Empty);
            Assert.That(issue.DomainEvents, Has.One.EqualTo(new IssueLabelRemoved(issue, label)));
        });
    }

    [Test]
    public void UpdateLabels()
    {
        var issue = GetIssue();

        var label = new Label
        {
            Id = Guid.NewGuid(),
            Name = "tag1",
            Color = 0,
        };
        issue.AddLabel(label);
        issue.AddLabel(new Label
        {
            Id = Guid.NewGuid(),
            Name = "tag2",
            Color = 0,
        });

        var newLabels = new List<Label>
        {
            label,
            new Label
            {
                Id = Guid.NewGuid(),
                Name = "tag3",
                Color = 0,
            },
        };
        issue.UpdateLabels(newLabels);

        Assert.That(issue.Labels, Has.Count.EqualTo(2));
        Assert.That(issue.Labels, Has.One.EqualTo(label));
    }

    [Test]
    public void TransitionToWithIncorrectRepositoryTest()
    {
        var issue = GetIssue();

        Assert.Throws<DomainException>(() => issue.TransitionTo(Guid.NewGuid(), new GitRepository
        {
            Name = "repo2",
        }));
    }

    [Test]
    public void TransitionToTheSameStatusTest()
    {
        var repository = new GitRepository
        {
            Name = "test",
        };
        var issue = GetIssue();

        var statusId = issue.Status.Id;
        issue.TransitionTo(statusId, repository);

        Assert.That(issue.Status.Id, Is.EqualTo(statusId));
    }

    [Test]
    public void TransitionToIncorrectStatusTest()
    {
        var issue = GetIssue();
        var repository = new GitRepository
        {
            Id = issue.RepositoryId,
            Name = "test",
        };

        Assert.Throws<NotFoundException>(() => issue.TransitionTo(Guid.NewGuid(), repository));
    }

    [Test]
    public void TransitionToWithoutTransitionTest()
    {
        var user = new User(Guid.NewGuid(), "user");
        var repository = new GitRepository
        {
            Name = "test",
        };
        var open = repository.AddIssueStatus("Open", 0);
        var done = repository.AddIssueStatus("Done", 0);

        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider
            .GetUtcNow()
            .Returns(DateTimeOffset.Now);
        var issue = new Issue.Builder(timeProvider)
            .WithTitle("title")
            .WithStatus(open)
            .WithRepository(repository)
            .WithAuthor(user)
            .WithInitialComment("comment")
            .Build();

        Assert.Throws<DomainException>(() => issue.TransitionTo(done.Id, repository));
    }

    [Test]
    public void TransitionToTest()
    {
        var user = new User(Guid.NewGuid(), "user");
        var repository = new GitRepository
        {
            Name = "test",
        };
        var open = repository.AddIssueStatus("Open", 0);
        var done = repository.AddIssueStatus("Done", 0);
        open.AddTransition(done);

        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider
            .GetUtcNow()
            .Returns(DateTimeOffset.Now);
        var issue = new Issue.Builder(timeProvider)
            .WithTitle("title")
            .WithStatus(open)
            .WithRepository(repository)
            .WithAuthor(user)
            .WithInitialComment("comment")
            .Build();

        issue.TransitionTo(done.Id, repository);

        Assert.Multiple(() =>
        {
            Assert.That(issue.Status.Id, Is.EqualTo(done.Id));
            Assert.That(issue.DomainEvents, Has.One.EqualTo(new IssueStatusChanged(issue, open, done)));
        });
    }

    [Test]
    public void LockIssue()
    {
        var issue = GetIssue();

        issue.Lock();

        Assert.Multiple(() =>
        {
            Assert.That(issue.IsLocked, Is.True);
            Assert.That(issue.DomainEvents, Has.One.EqualTo(new IssueLocked(issue)));
        });
    }

    [Test]
    public void UnlockIssue()
    {
        var issue = GetIssue();

        issue.Lock();
        issue.Unlock();

        Assert.Multiple(() =>
        {
            Assert.That(issue.IsLocked, Is.False);
            Assert.That(issue.DomainEvents, Has.One.EqualTo(new IssueUnlocked(issue)));
        });
    }

    private static IEnumerable<TestCaseData> GetDataForUpdateLockedIssue()
    {
        yield return new TestCaseData(new Action<Issue>(issue => issue.AddComment("test", new User(Guid.NewGuid(), "user"), DateTimeOffset.Now)))
            .SetName("AddComment");

        yield return new TestCaseData(new Action<Issue>(issue => issue.DeleteComment(null!)))
            .SetName("DeleteComment");

        yield return new TestCaseData(new Action<Issue>(issue => issue.AssignTo(new User(Guid.NewGuid(), "user"))))
            .SetName("AssignTo");

        yield return new TestCaseData(new Action<Issue>(issue => issue.AddLabel(new Label { Id = Guid.NewGuid(), Name = "tag", Color = 0 })))
            .SetName("AddLabel");

        yield return new TestCaseData(new Action<Issue>(issue => issue.RemoveLabel(new Label { Id = Guid.NewGuid(), Name = "tag", Color = 0 })))
            .SetName("RemoveLabel");

        yield return new TestCaseData(new Action<Issue>(issue => issue.TransitionTo(Guid.NewGuid(), null!)))
            .SetName("TransitionTo");
    }

    [Test]
    [TestCaseSource(nameof(GetDataForUpdateLockedIssue))]
    public void UpdateLockedIssue(Action<Issue> update)
    {
        var issue = GetIssue();

        issue.Lock();

        Assert.Throws<DomainException>(() => update(issue));
    }

    [Test]
    public void UpdateTitle()
    {
        const string oldTitle = "title";
        const string newTitle = "title 2";

        var issue = GetIssue();

        issue.UpdateTitle(newTitle);

        Assert.Multiple(() =>
        {
            Assert.That(issue.Title, Is.EqualTo(newTitle));
            Assert.That(issue.DomainEvents, Has.One.EqualTo(new IssueTitleChanged(issue, oldTitle, newTitle)));
        });
    }
}