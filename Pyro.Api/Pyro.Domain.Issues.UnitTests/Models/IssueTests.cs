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
    public void AddExistingTag()
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

        var tag = new Tag
        {
            Id = Guid.NewGuid(),
            Name = "tag",
            Color = 0,
        };
        issue.AddTag(tag);
        issue.AddTag(tag);

        Assert.That(issue.Tags, Has.Count.EqualTo(1));
        Assert.That(issue.Tags, Has.One.EqualTo(tag));
    }
}