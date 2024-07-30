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
    public void IncorrectCurrentUser()
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

        var handler = new CreateIssueHandler(currentUserProvider, repository, timeProvider);

        var command = new CreateIssue("repo", "title", null);

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

        var currentUserProvider = Substitute.For<ICurrentUserProvider>();
        currentUserProvider
            .GetCurrentUser()
            .Returns(currentUser);
        var repository = Substitute.For<IIssueRepository>();
        repository
            .GetUser(author.Id, Arg.Any<CancellationToken>())
            .Returns(author);
        if (assignee is not null)
        {
            repository
                .GetUser(assignee.Id, Arg.Any<CancellationToken>())
                .Returns(assignee);
        }

        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider
            .GetUtcNow()
            .Returns(now);

        var handler = new CreateIssueHandler(currentUserProvider, repository, timeProvider);

        var command = new CreateIssue("repo", "title", assignee?.Id);
        var issue = await handler.Handle(command);

        Assert.Multiple(() =>
        {
            Assert.That(issue.Title, Is.EqualTo(command.Title));
            Assert.That(issue.Repository.Name, Is.EqualTo(command.RepositoryName));
            Assert.That(issue.Author, Is.EqualTo(author));
            Assert.That(issue.CreatedAt, Is.EqualTo(now));
            Assert.That(issue.Assignee, Is.EqualTo(assignee));
        });
    }
}