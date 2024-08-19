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
            Repository = new GitRepository
            {
                Id = Guid.NewGuid(),
                Name = "test",
            },
            Author = user,
            CreatedAt = DateTimeOffset.Now,
        };
        issue.AssignTo(user);

        Assert.That(issue.Assignee, Is.EqualTo(user));
    }

    [Test]
    public void AddExistingLabel()
    {
        var user = new User(Guid.NewGuid(), "user");
        var issue = new Issue
        {
            Id = Guid.NewGuid(),
            IssueNumber = 1,
            Title = "title",
            Repository = new GitRepository
            {
                Id = Guid.NewGuid(),
                Name = "test",
            },
            Author = user,
            CreatedAt = DateTimeOffset.Now,
        };

        var label = new Label
        {
            Id = Guid.NewGuid(),
            Name = "tag",
            Color = 0,
        };
        issue.AddLabel(label);
        issue.AddLabel(label);

        Assert.That(issue.Labels, Has.Count.EqualTo(1));
        Assert.That(issue.Labels, Has.One.EqualTo(label));
    }
}