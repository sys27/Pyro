// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using NSubstitute;
using Pyro.Domain.Issues.Commands;
using Pyro.Domain.Shared;
using Pyro.Domain.Shared.Exceptions;
using Pyro.Domain.Shared.Models;

namespace Pyro.Domain.Issues.UnitTests.Commands;

public class CreateIssueHandlerTests
{
    [Test]
    public void MissingRepository()
    {
        var currentUser = new CurrentUser(Guid.NewGuid(), "TestUser", [], []);
        var command = new CreateIssue("repo", "title", null, []);

        var currentUserProvider = Substitute.For<ICurrentUserProvider>();
        currentUserProvider
            .GetCurrentUser()
            .Returns(currentUser);
        var issueRepository = Substitute.For<IIssueRepository>();
        issueRepository
            .GetUser(currentUser.Id, Arg.Any<CancellationToken>())
            .Returns((User?)null);
        var gitRepositoryRepository = Substitute.For<IGitRepositoryRepository>();
        gitRepositoryRepository
            .GetRepository(command.RepositoryName, Arg.Any<CancellationToken>())
            .Returns((GitRepository?)null);
        var timeProvider = Substitute.For<TimeProvider>();

        var handler = new CreateIssueHandler(currentUserProvider, issueRepository, gitRepositoryRepository, timeProvider);

        Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command));
    }

    [Test]
    public void IncorrectCurrentUser()
    {
        var currentUser = new CurrentUser(Guid.NewGuid(), "TestUser", [], []);
        var command = new CreateIssue("repo", "title", null, []);

        var currentUserProvider = Substitute.For<ICurrentUserProvider>();
        currentUserProvider
            .GetCurrentUser()
            .Returns(currentUser);
        var issueRepository = Substitute.For<IIssueRepository>();
        issueRepository
            .GetUser(currentUser.Id, Arg.Any<CancellationToken>())
            .Returns((User?)null);
        var gitRepositoryRepository = Substitute.For<IGitRepositoryRepository>();
        gitRepositoryRepository
            .GetRepository(command.RepositoryName, Arg.Any<CancellationToken>())
            .Returns(new GitRepository
            {
                Id = Guid.NewGuid(),
                Name = command.RepositoryName,
            });
        var timeProvider = Substitute.For<TimeProvider>();

        var handler = new CreateIssueHandler(currentUserProvider, issueRepository, gitRepositoryRepository, timeProvider);

        Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command));
    }

    private static IEnumerable<TestCaseData> CreateIssueAndAssignToUserCases()
    {
        yield return new TestCaseData(new User(Guid.NewGuid(), "Assignee"))
            .SetName("Assignee is not null");

        yield return new TestCaseData(null)
            .SetName("Assignee is null");
    }

    [Test]
    [TestCaseSource(nameof(CreateIssueAndAssignToUserCases))]
    public async Task CreateIssueAndAssignToUser(User? assignee)
    {
        var currentUser = new CurrentUser(Guid.NewGuid(), "TestUser", [], []);
        var author = new User(currentUser.Id, currentUser.Login);
        var now = DateTimeOffset.Now;
        var repository = new GitRepository
        {
            Id = Guid.NewGuid(),
            Name = "repo",
            Labels =
            [
                new Label
                {
                    Id = Guid.NewGuid(),
                    Name = "Tag 1",
                    Color = 0,
                },
                new Label
                {
                    Id = Guid.NewGuid(),
                    Name = "Tag 2",
                    Color = 0,
                }
            ],
        };
        var command = new CreateIssue(repository.Name, "title", assignee?.Id, [repository.Labels[0].Id, Guid.NewGuid()]);

        var currentUserProvider = Substitute.For<ICurrentUserProvider>();
        currentUserProvider
            .GetCurrentUser()
            .Returns(currentUser);
        var issueRepository = Substitute.For<IIssueRepository>();
        issueRepository
            .GetUser(author.Id, Arg.Any<CancellationToken>())
            .Returns(author);
        if (assignee is not null)
        {
            issueRepository
                .GetUser(assignee.Id, Arg.Any<CancellationToken>())
                .Returns(assignee);
        }

        var gitRepositoryRepository = Substitute.For<IGitRepositoryRepository>();
        gitRepositoryRepository
            .GetRepository(command.RepositoryName, Arg.Any<CancellationToken>())
            .Returns(repository);
        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider
            .GetUtcNow()
            .Returns(now);

        var handler = new CreateIssueHandler(currentUserProvider, issueRepository, gitRepositoryRepository, timeProvider);
        var issue = await handler.Handle(command);

        Assert.Multiple(() =>
        {
            Assert.That(issue.Title, Is.EqualTo(command.Title));
            Assert.That(issue.Repository.Name, Is.EqualTo(command.RepositoryName));
            Assert.That(issue.Author, Is.EqualTo(author));
            Assert.That(issue.CreatedAt, Is.EqualTo(now));
            Assert.That(issue.Assignee, Is.EqualTo(assignee));
            Assert.That(issue.Labels.Count, Is.EqualTo(1));
            Assert.That(issue.Labels[0], Is.EqualTo(repository.Labels[0]));
        });
    }
}