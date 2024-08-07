// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using NSubstitute;
using Pyro.Domain.Issues.Commands;
using Pyro.Domain.Shared;
using Pyro.Domain.Shared.Exceptions;
using Pyro.Domain.Shared.Models;

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
        var repository = Substitute.For<IIssueRepository>();
        repository
            .GetUser(currentUser.Id, Arg.Any<CancellationToken>())
            .Returns((User?)null);
        var timeProvider = Substitute.For<TimeProvider>();

        var handler = new CreateIssueCommentHandler(repository, currentUserProvider, timeProvider);

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
        var repository = Substitute.For<IIssueRepository>();
        repository
            .GetUser(currentUser.Id, Arg.Any<CancellationToken>())
            .Returns(author);
        repository
            .GetIssue(command.RepositoryName, command.IssueNumber, Arg.Any<CancellationToken>())
            .Returns((Issue?)null);
        var timeProvider = Substitute.For<TimeProvider>();

        var handler = new CreateIssueCommentHandler(repository, currentUserProvider, timeProvider);

        Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command));
    }

    [Test]
    public async Task CreateComment()
    {
        var currentUser = new CurrentUser(Guid.NewGuid(), "TestUser", [], []);
        var author = new User(currentUser.Id, currentUser.Login);
        var command = new CreateIssueComment("test", 1, "text");
        var issue = new Issue
        {
            Id = Guid.NewGuid(),
            IssueNumber = command.IssueNumber,
            Title = "title",
            Repository = new GitRepository(command.RepositoryName),
            Author = author,
            CreatedAt = DateTimeOffset.Now,
        };
        var now = DateTimeOffset.Now;

        var currentUserProvider = Substitute.For<ICurrentUserProvider>();
        currentUserProvider
            .GetCurrentUser()
            .Returns(currentUser);
        var repository = Substitute.For<IIssueRepository>();
        repository
            .GetUser(currentUser.Id, Arg.Any<CancellationToken>())
            .Returns(author);
        repository
            .GetIssue(command.RepositoryName, command.IssueNumber, Arg.Any<CancellationToken>())
            .Returns(issue);
        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider
            .GetUtcNow()
            .Returns(now);

        var handler = new CreateIssueCommentHandler(repository, currentUserProvider, timeProvider);

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