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
        var command = new UpdateIssue("repo", 1, "title", null, []);

        var currentUserProvider = Substitute.For<ICurrentUserProvider>();
        var issueRepository = Substitute.For<IIssueRepository>();
        issueRepository
            .GetIssue(command.RepositoryName, command.IssueNumber, Arg.Any<CancellationToken>())
            .Returns((Issue?)null);
        var gitRepositoryRepository = Substitute.For<IGitRepositoryRepository>();

        var handler = new UpdateIssueHandler(currentUserProvider, issueRepository, gitRepositoryRepository);

        Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command));
    }

    [Test]
    public void MissingRepository()
    {
        var currentUser = new CurrentUser(Guid.NewGuid(), "User", [], []);
        var command = new UpdateIssue("repo", 1, "title", null, []);
        var issue = new Issue
        {
            Id = Guid.NewGuid(),
            IssueNumber = command.IssueNumber,
            Title = "title",
            Repository = new GitRepository
            {
                Id = Guid.NewGuid(),
                Name = "test",
            },
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
        var gitRepositoryRepository = Substitute.For<IGitRepositoryRepository>();
        gitRepositoryRepository
            .GetRepository(command.RepositoryName, Arg.Any<CancellationToken>())
            .Returns((GitRepository?)null);

        var handler = new UpdateIssueHandler(currentUserProvider, repository, gitRepositoryRepository);

        Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command));
    }

    [Test]
    public void CurrentUserCantUpdateIssue()
    {
        var currentUser = new CurrentUser(Guid.NewGuid(), "User", [], []);
        var command = new UpdateIssue("repo", 1, "title", null, []);
        var gitRepository = new GitRepository
        {
            Id = Guid.NewGuid(),
            Name = "test",
        };
        var issue = new Issue
        {
            Id = Guid.NewGuid(),
            IssueNumber = command.IssueNumber,
            Title = "title",
            Repository = gitRepository,
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
        var gitRepositoryRepository = Substitute.For<IGitRepositoryRepository>();
        gitRepositoryRepository
            .GetRepository(command.RepositoryName, Arg.Any<CancellationToken>())
            .Returns(gitRepository);

        var handler = new UpdateIssueHandler(currentUserProvider, repository, gitRepositoryRepository);

        Assert.ThrowsAsync<DomainException>(() => handler.Handle(command));
    }

    [Test]
    public async Task UpdateIssue()
    {
        var currentUser = new CurrentUser(Guid.NewGuid(), "User", [], []);
        var author = new User(currentUser.Id, currentUser.Login);
        var gitRepository = new GitRepository
        {
            Id = Guid.NewGuid(),
            Name = "test",
            Tags =
            [
                new Tag
                {
                    Id = Guid.NewGuid(),
                    Name = "Tag 1",
                    Color = 0,
                },
                new Tag
                {
                    Id = Guid.NewGuid(),
                    Name = "Tag 2",
                    Color = 0,
                },
                new Tag
                {
                    Id = Guid.NewGuid(),
                    Name = "Tag 3",
                    Color = 0,
                },
            ],
        };
        var issue = new Issue
        {
            Id = Guid.NewGuid(),
            IssueNumber = 1,
            Title = "title",
            Repository = gitRepository,
            Author = author,
            CreatedAt = DateTimeOffset.Now,
        };
        issue.AddTag(gitRepository.Tags[0]);
        var command = new UpdateIssue(
            gitRepository.Name,
            issue.IssueNumber,
            "updated",
            null,
            [gitRepository.Tags[1].Id]);

        var currentUserProvider = Substitute.For<ICurrentUserProvider>();
        currentUserProvider
            .GetCurrentUser()
            .Returns(currentUser);
        var repository = Substitute.For<IIssueRepository>();
        repository
            .GetIssue(command.RepositoryName, command.IssueNumber, Arg.Any<CancellationToken>())
            .Returns(issue);
        var gitRepositoryRepository = Substitute.For<IGitRepositoryRepository>();
        gitRepositoryRepository
            .GetRepository(command.RepositoryName, Arg.Any<CancellationToken>())
            .Returns(gitRepository);

        var handler = new UpdateIssueHandler(currentUserProvider, repository, gitRepositoryRepository);

        var updatedIssue = await handler.Handle(command);

        Assert.That(updatedIssue, Is.Not.Null);
        Assert.That(updatedIssue.Title, Is.EqualTo(command.Title));
        Assert.That(updatedIssue.Tags, Has.Count.EqualTo(1));
        Assert.That(updatedIssue.Tags[0], Is.EqualTo(gitRepository.Tags[1]));
    }
}