// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using NSubstitute;
using Pyro.Domain.Issues.Commands;
using Pyro.Domain.Shared.CurrentUserProvider;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Issues.UnitTests.Commands;

public class CreateIssueCommentHandlerTests
{
    [Test]
    public void IncorrectAuthor()
    {
        var currentUser = new CurrentUser(Guid.NewGuid(), "TestUser", [], []);

        var currentUserProvider = Substitute.For<ICurrentUserProvider>();
        currentUserProvider
            .GetCurrentUser()
            .Returns(currentUser);
        var issueRepository = Substitute.For<IIssueRepository>();
        var gitRepositoryRepository = Substitute.For<IGitRepositoryRepository>();
        gitRepositoryRepository
            .GetUser(currentUser.Id, Arg.Any<CancellationToken>())
            .Returns((User?)null);
        var timeProvider = Substitute.For<TimeProvider>();

        var handler = new CreateIssueCommentHandler(
            issueRepository,
            gitRepositoryRepository,
            currentUserProvider,
            timeProvider);

        var command = new CreateIssueComment("test", 1, "text");

        Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command));
    }

    [Test]
    public void MissingIssue()
    {
        var currentUser = new CurrentUser(Guid.NewGuid(), "TestUser", [], []);
        var author = new User(currentUser.Id, currentUser.Login);
        var command = new CreateIssueComment("test", 1, "text");

        var currentUserProvider = Substitute.For<ICurrentUserProvider>();
        currentUserProvider
            .GetCurrentUser()
            .Returns(currentUser);
        var issueRepository = Substitute.For<IIssueRepository>();
        issueRepository
            .GetIssue(command.RepositoryName, command.IssueNumber, Arg.Any<CancellationToken>())
            .Returns((Issue?)null);
        var gitRepositoryRepository = Substitute.For<IGitRepositoryRepository>();
        gitRepositoryRepository
            .GetUser(currentUser.Id, Arg.Any<CancellationToken>())
            .Returns(author);
        var timeProvider = Substitute.For<TimeProvider>();

        var handler = new CreateIssueCommentHandler(
            issueRepository,
            gitRepositoryRepository,
            currentUserProvider,
            timeProvider);

        Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command));
    }

    [Test]
    public async Task CreateComment()
    {
        var now = DateTimeOffset.Now;
        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider
            .GetUtcNow()
            .Returns(now);

        var currentUser = new CurrentUser(Guid.NewGuid(), "TestUser", [], []);
        var author = new User(currentUser.Id, currentUser.Login);
        var command = new CreateIssueComment("test", 1, "text");
        var gitRepository = new GitRepository
        {
            Id = Guid.NewGuid(),
            Name = "test",
        };
        var issueStatus = new IssueStatus
        {
            Id = Guid.NewGuid(),
            Name = "status",
            Color = 0,
            Repository = gitRepository,
        };
        var issue = new Issue.Builder(timeProvider)
            .WithTitle("title")
            .WithStatus(issueStatus)
            .WithRepository(gitRepository)
            .WithAuthor(author)
            .WithInitialComment("comment")
            .Build();

        var currentUserProvider = Substitute.For<ICurrentUserProvider>();
        currentUserProvider
            .GetCurrentUser()
            .Returns(currentUser);
        var issueRepository = Substitute.For<IIssueRepository>();
        issueRepository
            .GetIssue(command.RepositoryName, command.IssueNumber, Arg.Any<CancellationToken>())
            .Returns(issue);
        var gitRepositoryRepository = Substitute.For<IGitRepositoryRepository>();
        gitRepositoryRepository
            .GetUser(currentUser.Id, Arg.Any<CancellationToken>())
            .Returns(author);

        var handler = new CreateIssueCommentHandler(
            issueRepository,
            gitRepositoryRepository,
            currentUserProvider,
            timeProvider);

        var comment = await handler.Handle(command);

        Assert.Multiple(() =>
        {
            Assert.That(comment.Content, Is.EqualTo(command.Content));
            Assert.That(comment.Author, Is.EqualTo(author));
            Assert.That(comment.Issue, Is.EqualTo(issue));
            Assert.That(comment.CreatedAt, Is.EqualTo(now));
        });
    }
}