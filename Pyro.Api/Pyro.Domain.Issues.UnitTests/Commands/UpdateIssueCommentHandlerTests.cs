// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using NSubstitute;
using Pyro.Domain.Issues.Commands;
using Pyro.Domain.Shared.CurrentUserProvider;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Issues.UnitTests.Commands;

public class UpdateIssueCommentHandlerTests
{
    [Test]
    public void MissingIssue()
    {
        var command = new UpdateIssueComment("repo", 1, Guid.NewGuid(), "content");

        var currentUserProvider = Substitute.For<ICurrentUserProvider>();
        var repository = Substitute.For<IIssueRepository>();
        repository
            .GetIssue(command.RepositoryName, command.IssueNumber, Arg.Any<CancellationToken>())
            .Returns((Issue?)null);

        var handler = new UpdateIssueCommentHandler(currentUserProvider, repository);

        Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command));
    }

    [Test]
    public void MissingIssueComment()
    {
        var now = DateTimeOffset.Now;
        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider
            .GetUtcNow()
            .Returns(now);

        const string repositoryName = "repo";
        const int issueNumber = 1;
        var currentUser = new CurrentUser(Guid.NewGuid(), "User", [], []);
        var author = new User(currentUser.Id, currentUser.Login);
        var gitRepository = new GitRepository
        {
            Id = Guid.NewGuid(),
            Name = "test",
        };
        var issueStatus = new IssueStatus
        {
            Name = "Open",
            Color = 0,
            Repository = gitRepository,
        };
        var issue = new Issue.Builder(timeProvider)
            .WithTitle("title")
            .WithIssueNumber(issueNumber)
            .WithStatus(issueStatus)
            .WithRepository(gitRepository)
            .WithAuthor(author)
            .WithInitialComment("comment")
            .Build();
        var comment = issue.AddComment("test", author, DateTimeOffset.Now);
        var command = new UpdateIssueComment(repositoryName, issueNumber, Guid.NewGuid(), "content");

        var currentUserProvider = Substitute.For<ICurrentUserProvider>();
        var repository = Substitute.For<IIssueRepository>();
        repository
            .GetIssue(command.RepositoryName, command.IssueNumber, Arg.Any<CancellationToken>())
            .Returns(issue);

        var handler = new UpdateIssueCommentHandler(currentUserProvider, repository);

        Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command));
    }

    [Test]
    public void CurrentUserCantUpdateIssueComment()
    {
        var now = DateTimeOffset.Now;
        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider
            .GetUtcNow()
            .Returns(now);

        const string repositoryName = "repo";
        const int issueNumber = 1;
        var currentUser = new CurrentUser(Guid.NewGuid(), "User", [], []);
        var author = new User(currentUser.Id, currentUser.Login);
        var gitRepository = new GitRepository
        {
            Name = "test",
        };
        var issueStatus = new IssueStatus
        {
            Name = "Open",
            Color = 0,
            Repository = gitRepository,
        };
        var issue = new Issue.Builder(timeProvider)
            .WithTitle("title")
            .WithIssueNumber(issueNumber)
            .WithStatus(issueStatus)
            .WithRepository(gitRepository)
            .WithAuthor(author)
            .WithInitialComment("comment")
            .Build();
        var comment = issue.AddComment("test", new User(Guid.NewGuid(), "test"), DateTimeOffset.Now);
        var command = new UpdateIssueComment(repositoryName, issueNumber, comment.Id, "content");

        var currentUserProvider = Substitute.For<ICurrentUserProvider>();
        currentUserProvider
            .GetCurrentUser()
            .Returns(currentUser);
        var repository = Substitute.For<IIssueRepository>();
        repository
            .GetIssue(command.RepositoryName, command.IssueNumber, Arg.Any<CancellationToken>())
            .Returns(issue);

        var handler = new UpdateIssueCommentHandler(currentUserProvider, repository);

        Assert.ThrowsAsync<DomainException>(() => handler.Handle(command));
    }

    [Test]
    public async Task UpdateIssueComment()
    {
        var now = DateTimeOffset.Now;
        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider
            .GetUtcNow()
            .Returns(now);

        const string repositoryName = "repo";
        const int issueNumber = 1;
        var currentUser = new CurrentUser(Guid.NewGuid(), "User", [], []);
        var author = new User(currentUser.Id, currentUser.Login);
        var gitRepository = new GitRepository
        {
            Name = "test",
        };
        var issueStatus = new IssueStatus
        {
            Name = "Open",
            Color = 0,
            Repository = gitRepository,
        };
        var issue = new Issue.Builder(timeProvider)
            .WithTitle("title")
            .WithIssueNumber(issueNumber)
            .WithStatus(issueStatus)
            .WithRepository(gitRepository)
            .WithAuthor(author)
            .WithInitialComment("comment")
            .Build();
        var comment = issue.AddComment("test", author, DateTimeOffset.Now);
        var command = new UpdateIssueComment(repositoryName, issueNumber, comment.Id, "content");

        var currentUserProvider = Substitute.For<ICurrentUserProvider>();
        currentUserProvider
            .GetCurrentUser()
            .Returns(currentUser);
        var repository = Substitute.For<IIssueRepository>();
        repository
            .GetIssue(command.RepositoryName, command.IssueNumber, Arg.Any<CancellationToken>())
            .Returns(issue);

        var handler = new UpdateIssueCommentHandler(currentUserProvider, repository);
        var updatedComment = await handler.Handle(command);

        Assert.That(updatedComment, Is.Not.Null);
        Assert.That(updatedComment.Content, Is.EqualTo(command.Content));
    }
}