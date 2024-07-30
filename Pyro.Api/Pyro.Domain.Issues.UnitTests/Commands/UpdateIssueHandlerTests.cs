// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using NSubstitute;
using Pyro.Domain.Issues.Commands;
using Pyro.Domain.Shared;
using Pyro.Domain.Shared.Exceptions;
using Pyro.Domain.Shared.Models;

namespace Pyro.Domain.Issues.UnitTests.Commands;

public class UpdateIssueHandlerTests
{
    [Test]
    public void MissingIssue()
    {
        var command = new UpdateIssue("repo", 1, "title", null);

        var currentUserProvider = Substitute.For<ICurrentUserProvider>();
        var repository = Substitute.For<IIssueRepository>();
        repository
            .GetIssue(command.RepositoryName, command.IssueNumber, Arg.Any<CancellationToken>())
            .Returns((Issue?)null);

        var handler = new UpdateIssueHandler(currentUserProvider, repository);

        Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command));
    }

    [Test]
    public void CurrentUserCantUpdateIssue()
    {
        var currentUser = new CurrentUser(Guid.NewGuid(), "User", [], []);
        var command = new UpdateIssue("repo", 1, "title", null);
        var issue = new Issue
        {
            Id = Guid.NewGuid(),
            IssueNumber = command.IssueNumber,
            Title = "title",
            Repository = new GitRepository(command.RepositoryName),
            Author = new User(Guid.NewGuid(), "test"),
            CreatedAt = DateTimeOffset.Now,
        };

        var currentUserProvider = Substitute.For<ICurrentUserProvider>();
        currentUserProvider
            .GetCurrentUser()
            .Returns(currentUser);
        var repository = Substitute.For<IIssueRepository>();
        repository
            .GetIssue(command.RepositoryName, command.IssueNumber, Arg.Any<CancellationToken>())
            .Returns(issue);

        var handler = new UpdateIssueHandler(currentUserProvider, repository);

        Assert.ThrowsAsync<DomainException>(() => handler.Handle(command));
    }

    [Test]
    public async Task UpdateIssue()
    {
        var currentUser = new CurrentUser(Guid.NewGuid(), "User", [], []);
        var author = new User(currentUser.Id, currentUser.Login);
        var command = new UpdateIssue("repo", 1, "title", null);
        var issue = new Issue
        {
            Id = Guid.NewGuid(),
            IssueNumber = command.IssueNumber,
            Title = "title",
            Repository = new GitRepository(command.RepositoryName),
            Author = author,
            CreatedAt = DateTimeOffset.Now,
        };

        var currentUserProvider = Substitute.For<ICurrentUserProvider>();
        currentUserProvider
            .GetCurrentUser()
            .Returns(currentUser);
        var repository = Substitute.For<IIssueRepository>();
        repository
            .GetIssue(command.RepositoryName, command.IssueNumber, Arg.Any<CancellationToken>())
            .Returns(issue);

        var handler = new UpdateIssueHandler(currentUserProvider, repository);

        var updatedIssue = await handler.Handle(command);

        Assert.That(updatedIssue, Is.Not.Null);
        Assert.That(updatedIssue.Title, Is.EqualTo(command.Title));
    }
}