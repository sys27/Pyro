// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Domain.Issues.UnitTests.Models;

public class IssueTests
{
    [Test]
    public void AssignToTest()
    {
        var user = new User(Guid.NewGuid(), "user");

        var issue = new Issue
        {
            Id = Guid.NewGuid(),
            IssueNumber = 1,
            Title = "title",
            Repository = new GitRepository("test"),
            Author = user,
            CreatedAt = DateTimeOffset.Now,
        };
        issue.AssignTo(user);

        Assert.That(issue.Assignee, Is.EqualTo(user));
    }
}