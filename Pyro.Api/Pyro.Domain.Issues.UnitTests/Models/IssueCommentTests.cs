// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Domain.Issues.UnitTests.Models;

public class IssueCommentTests
{
    [Test]
    public void AddCommentTest()
    {
        const string content = "text";
        var createdAt = DateTimeOffset.Now;
        var user = new User(Guid.NewGuid(), "user");

        var gitRepository = new GitRepository
        {
            Id = Guid.NewGuid(),
            Name = "test",
        };
        var issue = new Issue
        {
            Id = Guid.NewGuid(),
            IssueNumber = 1,
            Title = "title",
            Status = new IssueStatus
            {
                Id = Guid.NewGuid(),
                Name = "status",
                Color = 0,
                Repository = gitRepository,
            },
            RepositoryId = gitRepository.Id,
            Author = user,
            CreatedAt = DateTimeOffset.Now,
        };
        var comment = issue.AddComment(content, user, createdAt);

        Assert.Multiple(() =>
        {
            Assert.That(issue.Comments, Has.Count.EqualTo(1));

            Assert.That(comment.Content, Is.EqualTo(content));
            Assert.That(comment.Author, Is.EqualTo(user));
            Assert.That(comment.Issue, Is.EqualTo(issue));
            Assert.That(comment.CreatedAt, Is.EqualTo(createdAt));
        });
    }

    [Test]
    public void DeleteCommentTest()
    {
        var user = new User(Guid.NewGuid(), "user");

        var gitRepository = new GitRepository
        {
            Id = Guid.NewGuid(),
            Name = "test",
        };
        var issue = new Issue
        {
            Id = Guid.NewGuid(),
            IssueNumber = 1,
            Title = "title",
            Status = new IssueStatus
            {
                Id = Guid.NewGuid(),
                Name = "status",
                Color = 0,
                Repository = gitRepository,
            },
            RepositoryId = gitRepository.Id,
            Author = user,
            CreatedAt = DateTimeOffset.Now,
        };
        var comment = issue.AddComment("text", user, DateTimeOffset.Now);
        issue.DeleteComment(comment);

        Assert.That(issue.Comments, Is.Empty);
    }
}