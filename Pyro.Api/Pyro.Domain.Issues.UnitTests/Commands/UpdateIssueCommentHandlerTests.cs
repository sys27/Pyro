// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using NSubstitute;
using Pyro.Domain.Issues.Commands;
using Pyro.Domain.Shared;
using Pyro.Domain.Shared.Exceptions;
using Pyro.Domain.Shared.Models;

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
        const string repositoryName = "repo";
        const int issueNumber = 1;
        var currentUser = new CurrentUser(Guid.NewGuid(), "User", [], []);
        var author = new User(currentUser.Id, currentUser.Login);
        var issue = new Issue
        {
            Id = Guid.NewGuid(),
            IssueNumber = issueNumber,
            Title = "title",
            Repository = new GitRepository
            {
                Id = Guid.NewGuid(),
                Name = "test",
            },
            Author = author,
            CreatedAt = DateTimeOffset.Now,
        };
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
        const string repositoryName = "repo";
        const int issueNumber = 1;
        var currentUser = new CurrentUser(Guid.NewGuid(), "User", [], []);
        var author = new User(currentUser.Id, currentUser.Login);
        var issue = new Issue
        {
            Id = Guid.NewGuid(),
            IssueNumber = issueNumber,
            Title = "title",
            Repository = new GitRepository
            {
                Id = Guid.NewGuid(),
                Name = "test",
            },
            Author = author,
            CreatedAt = DateTimeOffset.Now,
        };
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
        const string repositoryName = "repo";
        const int issueNumber = 1;
        var currentUser = new CurrentUser(Guid.NewGuid(), "User", [], []);
        var author = new User(currentUser.Id, currentUser.Login);
        var issue = new Issue
        {
            Id = Guid.NewGuid(),
            IssueNumber = issueNumber,
            Title = "title",
            Repository = new GitRepository
            {
                Id = Guid.NewGuid(),
                Name = "test",
            },
            Author = author,
            CreatedAt = DateTimeOffset.Now,
        };
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