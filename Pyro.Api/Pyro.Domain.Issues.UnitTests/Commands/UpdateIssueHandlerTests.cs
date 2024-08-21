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
        var command = new UpdateIssue("repo", 1, "title", null, Guid.NewGuid(), []);

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
        var gitRepository = new GitRepository
        {
            Id = Guid.NewGuid(),
            Name = "test",
        };
        gitRepository.AddIssueStatus("Open", 0);
        var command = new UpdateIssue("repo", 1, "title", null, gitRepository.IssueStatuses[0].Id, []);
        var issue = new Issue
        {
            Id = Guid.NewGuid(),
            IssueNumber = command.IssueNumber,
            Title = "title",
            Status = new IssueStatus
            {
                Id = Guid.NewGuid(),
                Name = "Open",
                Color = 0,
                Repository = gitRepository,
            },
            RepositoryId = gitRepository.Id,
            Author = new User(currentUser.Id, currentUser.Login),
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
        var command = new UpdateIssue("repo", 1, "title", null, Guid.NewGuid(), []);
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
            Status = new IssueStatus
            {
                Id = Guid.NewGuid(),
                Name = "Open",
                Color = 0,
                Repository = gitRepository,
            },
            RepositoryId = gitRepository.Id,
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
                },
                new Label
                {
                    Id = Guid.NewGuid(),
                    Name = "Tag 3",
                    Color = 0,
                },
            ],
        };
        var open = gitRepository.AddIssueStatus("Open", 0);
        var done = gitRepository.AddIssueStatus("Done", 0);
        open.AddTransition(done);
        var issue = new Issue
        {
            Id = Guid.NewGuid(),
            IssueNumber = 1,
            Title = "title",
            Status = open,
            RepositoryId = gitRepository.Id,
            Author = author,
            CreatedAt = DateTimeOffset.Now,
        };
        issue.AddLabel(gitRepository.Labels[0]);
        var command = new UpdateIssue(
            gitRepository.Name,
            issue.IssueNumber,
            "updated",
            null,
            done.Id,
            [gitRepository.Labels[1].Id]);

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
        Assert.Multiple(() =>
        {
            Assert.That(updatedIssue.Title, Is.EqualTo(command.Title));
            Assert.That(updatedIssue.Labels, Has.Count.EqualTo(1));
            Assert.That(updatedIssue.Labels[0], Is.EqualTo(gitRepository.Labels[1]));
            Assert.That(updatedIssue.Status.Id, Is.EqualTo(command.StatusId));
        });
    }
}