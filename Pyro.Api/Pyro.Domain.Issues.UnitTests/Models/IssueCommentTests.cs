// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using NSubstitute;

namespace Pyro.Domain.Issues.UnitTests.Models;

public class IssueCommentTests
{
    private static Issue GetIssue()
    {
        var user = new User(Guid.NewGuid(), "user");
        var gitRepository = new GitRepository
        {
            Name = "test",
        };
        var issueStatus = new IssueStatus
        {
            Name = "status",
            Color = 0,
            Repository = gitRepository,
        };

        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider
            .GetUtcNow()
            .Returns(DateTimeOffset.Now);
        var issue = new Issue.Builder(timeProvider)
            .WithTitle("title")
            .WithStatus(issueStatus)
            .WithRepository(gitRepository)
            .WithAuthor(user)
            .WithInitialComment("comment")
            .Build();

        return issue;
    }

    [Test]
    public void AddCommentTest()
    {
        const string content = "text";
        var createdAt = DateTimeOffset.Now;
        var user = new User(Guid.NewGuid(), "user");
        var issue = GetIssue();
        var comment = issue.AddComment(content, user, createdAt);

        Assert.Multiple(() =>
        {
            Assert.That(issue.Comments, Has.Count.EqualTo(2));

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

        var issue = GetIssue();
        var comment = issue.AddComment("text", user, DateTimeOffset.Now);
        issue.DeleteComment(comment);

        Assert.That(issue.Comments, Has.Count.EqualTo(1));
    }
}