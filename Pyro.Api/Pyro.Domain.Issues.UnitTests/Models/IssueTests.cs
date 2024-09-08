// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Issues.UnitTests.Models;

public class IssueTests
{
    [Test]
    public void AssignToTest()
    {
        var user = new User(Guid.NewGuid(), "user");

        var gitRepository = new GitRepository
        {
            Id = Guid.NewGuid(),
            Name = "test",
        };
        var issue = new Issue
        {
            Id = Guid.NewGuid(),
            IssueNumber = 1,
            Title = "title",
            Status = new IssueStatus
            {
                Id = Guid.NewGuid(),
                Name = "status",
                Color = 0,
                Repository = gitRepository,
            },
            RepositoryId = gitRepository.Id,
            Author = user,
            CreatedAt = DateTimeOffset.Now,
        };
        issue.AssignTo(user);

        Assert.That(issue.Assignee, Is.EqualTo(user));
    }

    [Test]
    public void AddExistingLabel()
    {
        var user = new User(Guid.NewGuid(), "user");
        var gitRepository = new GitRepository
        {
            Id = Guid.NewGuid(),
            Name = "test",
        };
        var issue = new Issue
        {
            Id = Guid.NewGuid(),
            IssueNumber = 1,
            Title = "title",
            Status = new IssueStatus
            {
                Id = Guid.NewGuid(),
                Name = "status",
                Color = 0,
                Repository = gitRepository,
            },
            RepositoryId = gitRepository.Id,
            Author = user,
            CreatedAt = DateTimeOffset.Now,
        };

        var label = new Label
        {
            Id = Guid.NewGuid(),
            Name = "tag",
            Color = 0,
        };
        issue.AddLabel(label);
        issue.AddLabel(label);

        Assert.That(issue.Labels, Has.Count.EqualTo(1));
        Assert.That(issue.Labels, Has.One.EqualTo(label));
    }

    [Test]
    public void RemoveLabel()
    {
        var gitRepository = new GitRepository
        {
            Id = Guid.NewGuid(),
            Name = "test",
        };
        var issue = new Issue
        {
            Id = Guid.NewGuid(),
            IssueNumber = 1,
            Title = "title",
            Status = new IssueStatus
            {
                Id = Guid.NewGuid(),
                Name = "status",
                Color = 0,
                Repository = gitRepository,
            },
            RepositoryId = gitRepository.Id,
            Author = new User(Guid.NewGuid(), "user"),
            CreatedAt = DateTimeOffset.Now,
        };

        var label = new Label
        {
            Id = Guid.NewGuid(),
            Name = "tag",
            Color = 0,
        };
        issue.AddLabel(label);
        issue.RemoveLabel(label);

        Assert.That(issue.Labels, Is.Empty);
    }

    [Test]
    public void UpdateLabels()
    {
        var user = new User(Guid.NewGuid(), "user");
        var gitRepository = new GitRepository
        {
            Id = Guid.NewGuid(),
            Name = "test",
        };
        var issue = new Issue
        {
            Id = Guid.NewGuid(),
            IssueNumber = 1,
            Title = "title",
            Status = new IssueStatus
            {
                Id = Guid.NewGuid(),
                Name = "status",
                Color = 0,
                Repository = gitRepository,
            },
            RepositoryId = gitRepository.Id,
            Author = user,
            CreatedAt = DateTimeOffset.Now,
        };

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
        var repository = new GitRepository
        {
            Name = "test",
        };
        var issue = new Issue
        {
            IssueNumber = 1,
            Title = "title",
            Status = new IssueStatus
            {
                Name = "status",
                Color = 0,
                Repository = repository,
            },
            RepositoryId = repository.Id,
            Author = new User(Guid.NewGuid(), "user"),
            CreatedAt = DateTimeOffset.Now,
        };

        Assert.Throws<DomainException>(() => issue.TransitionTo(Guid.NewGuid(), new GitRepository { Name = "repo2" }));
    }

    [Test]
    public void TransitionToTheSameStatusTest()
    {
        var repository = new GitRepository
        {
            Name = "test",
        };
        var issue = new Issue
        {
            IssueNumber = 1,
            Title = "title",
            Status = new IssueStatus
            {
                Name = "status",
                Color = 0,
                Repository = repository,
            },
            RepositoryId = repository.Id,
            Author = new User(Guid.NewGuid(), "user"),
            CreatedAt = DateTimeOffset.Now,
        };

        var statusId = issue.Status.Id;
        issue.TransitionTo(statusId, repository);

        Assert.That(issue.Status.Id, Is.EqualTo(statusId));
    }

    [Test]
    public void TransitionToIncorrectStatusTest()
    {
        var repository = new GitRepository
        {
            Name = "test",
        };
        var issue = new Issue
        {
            IssueNumber = 1,
            Title = "title",
            Status = new IssueStatus
            {
                Name = "status",
                Color = 0,
                Repository = repository,
            },
            RepositoryId = repository.Id,
            Author = new User(Guid.NewGuid(), "user"),
            CreatedAt = DateTimeOffset.Now,
        };

        Assert.Throws<NotFoundException>(() => issue.TransitionTo(Guid.NewGuid(), repository));
    }

    [Test]
    public void TransitionToWithoutTransitionTest()
    {
        var repository = new GitRepository
        {
            Name = "test",
        };
        var open = repository.AddIssueStatus("Open", 0);
        var done = repository.AddIssueStatus("Done", 0);

        var issue = new Issue
        {
            IssueNumber = 1,
            Title = "title",
            Status = open,
            RepositoryId = repository.Id,
            Author = new User(Guid.NewGuid(), "user"),
            CreatedAt = DateTimeOffset.Now,
        };

        Assert.Throws<DomainException>(() => issue.TransitionTo(done.Id, repository));
    }

    [Test]
    public void TransitionToTest()
    {
        var repository = new GitRepository
        {
            Name = "test",
        };
        var open = repository.AddIssueStatus("Open", 0);
        var done = repository.AddIssueStatus("Done", 0);
        open.AddTransition(done);

        var issue = new Issue
        {
            IssueNumber = 1,
            Title = "title",
            Status = open,
            RepositoryId = repository.Id,
            Author = new User(Guid.NewGuid(), "user"),
            CreatedAt = DateTimeOffset.Now,
        };

        issue.TransitionTo(done.Id, repository);

        Assert.That(issue.Status.Id, Is.EqualTo(done.Id));
    }

    [Test]
    public void LockIssue()
    {
        var gitRepository = new GitRepository
        {
            Id = Guid.NewGuid(),
            Name = "test",
        };
        var issue = new Issue
        {
            Id = Guid.NewGuid(),
            IssueNumber = 1,
            Title = "title",
            Status = new IssueStatus
            {
                Id = Guid.NewGuid(),
                Name = "status",
                Color = 0,
                Repository = gitRepository,
            },
            RepositoryId = gitRepository.Id,
            Author = new User(Guid.NewGuid(), "user"),
            CreatedAt = DateTimeOffset.Now,
        };

        issue.Lock();

        Assert.That(issue.IsLocked, Is.True);
    }

    [Test]
    public void UnlockIssue()
    {
        var gitRepository = new GitRepository
        {
            Id = Guid.NewGuid(),
            Name = "test",
        };
        var issue = new Issue
        {
            Id = Guid.NewGuid(),
            IssueNumber = 1,
            Title = "title",
            Status = new IssueStatus
            {
                Id = Guid.NewGuid(),
                Name = "status",
                Color = 0,
                Repository = gitRepository,
            },
            RepositoryId = gitRepository.Id,
            Author = new User(Guid.NewGuid(), "user"),
            CreatedAt = DateTimeOffset.Now,
        };

        issue.Lock();
        issue.Unlock();

        Assert.That(issue.IsLocked, Is.False);
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
        var gitRepository = new GitRepository
        {
            Id = Guid.NewGuid(),
            Name = "test",
        };
        var issue = new Issue
        {
            Id = Guid.NewGuid(),
            IssueNumber = 1,
            Title = "title",
            Status = new IssueStatus
            {
                Id = Guid.NewGuid(),
                Name = "status",
                Color = 0,
                Repository = gitRepository,
            },
            RepositoryId = gitRepository.Id,
            Author = new User(Guid.NewGuid(), "user"),
            CreatedAt = DateTimeOffset.Now,
        };

        issue.Lock();

        Assert.Throws<DomainException>(() => update(issue));
    }
}