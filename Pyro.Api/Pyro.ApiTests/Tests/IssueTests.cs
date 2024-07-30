// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Bogus;
using Pyro.Contracts.Requests;
using Pyro.Contracts.Requests.Issues;
using Pyro.Contracts.Responses;
using Pyro.Contracts.Responses.Issues;

namespace Pyro.ApiTests.Tests;

public class IssueTests
{
    [Test]
    public async Task CreateGetUpdateIssue()
    {
        var name = await CreateRepository();
        var number = await CreateIssue(name);
        await UpdateIssue(name, number);
        await GetIssue(name, number);

        var commentId = await CreateComment(name, number);
        await UpdateComment(name, number, commentId);
        await DeleteComment(name, number, commentId);
        await GetCommentsAfterCommentDelete(name, number);

        await DeleteIssue(name, number);
        await GetIssuesAfterIssueDelete(name, number);
    }

    private async Task<string> CreateRepository()
    {
        var createRequest = new CreateGitRepositoryRequest(
            new Faker().Lorem.Word(),
            new Faker().Lorem.Sentence(),
            "master");
        var repository = await Api.Post<GitRepositoryResponse>("/api/repositories", createRequest) ??
                         throw new Exception("Repository not created");

        return repository.Name;
    }

    private async Task<int> CreateIssue(string name)
    {
        var createRequest = new CreateIssueRequest(
            new Faker().Lorem.Sentence(),
            null);
        var issue = await Api.Post<IssueResponse>($"/api/repositories/{name}/issues", createRequest);

        Assert.That(issue, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(issue.Title, Is.EqualTo(createRequest.Title));
            Assert.That(issue.Assignee?.Id, Is.EqualTo(createRequest.AssigneeId));
        });

        return issue.IssueNumber;
    }

    private async Task UpdateIssue(string name, int number)
    {
        var updateRequest = new UpdateIssueRequest(
            new Faker().Lorem.Sentence(),
            Guid.Parse("F9BA057A-35B0-4D10-8326-702D8F7EC966"));
        var issue = await Api.Put<IssueResponse>($"/api/repositories/{name}/issues/{number}", updateRequest);

        Assert.That(issue, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(issue.Title, Is.EqualTo(updateRequest.Title));
            Assert.That(issue.Assignee?.Id, Is.EqualTo(updateRequest.AssigneeId));
        });
    }

    private async Task GetIssue(string name, int number)
    {
        var issue = await Api.Get<IssueResponse>($"/api/repositories/{name}/issues/{number}");

        Assert.That(issue, Is.Not.Null);
        Assert.That(issue.IssueNumber, Is.EqualTo(number));
    }

    private async Task DeleteIssue(string name, int number)
        => await Api.Delete($"/api/repositories/{name}/issues/{number}");

    private async Task GetIssuesAfterIssueDelete(string name, int number)
    {
        var issues = await Api.Get<IReadOnlyList<IssueResponse>>($"/api/repositories/{name}/issues");

        Assert.That(issues, Is.Not.Null);
        Assert.That(issues.Select(x => x.IssueNumber), Does.Not.Contains(number));
    }

    private async Task<Guid> CreateComment(string name, int number)
    {
        var createRequest = new CreateCommentRequest(new Faker().Lorem.Sentence());
        var comment = await Api.Post<IssueCommentResponse>($"/api/repositories/{name}/issues/{number}/comments", createRequest);

        Assert.That(comment, Is.Not.Null);
        Assert.That(comment.Content, Is.EqualTo(createRequest.Content));

        return comment.Id;
    }

    private async Task UpdateComment(string name, int number, Guid commentId)
    {
        var updateRequest = new UpdateCommentRequest(new Faker().Lorem.Sentence());
        var comment = await Api.Put<IssueCommentResponse>($"/api/repositories/{name}/issues/{number}/comments/{commentId}", updateRequest);

        Assert.That(comment, Is.Not.Null);
        Assert.That(comment.Content, Is.EqualTo(updateRequest.Content));
    }

    private async Task DeleteComment(string name, int number, Guid id)
        => await Api.Delete($"/api/repositories/{name}/issues/{number}/comments/{id}");

    private async Task GetCommentsAfterCommentDelete(string name, int number)
    {
        var comments = await Api.Get<IReadOnlyList<IssueCommentResponse>>($"/api/repositories/{name}/issues/{number}/comments");

        Assert.That(comments, Is.Empty);
    }
}