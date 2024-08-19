// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Bogus;
using Pyro.ApiTests.Clients;
using Pyro.Contracts.Requests;
using Pyro.Contracts.Requests.Issues;

namespace Pyro.ApiTests.Tests;

public class IssueTests
{
    private Faker faker;
    private PyroClient pyroClient;
    private IssueClient issueClient;

    [OneTimeSetUp]
    public async Task SetUp()
    {
        faker = new Faker();
        pyroClient = new PyroClient(Api.BaseAddress);
        issueClient = pyroClient.Share<IssueClient>();
        await issueClient.Login();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await issueClient.Logout();
        issueClient.Dispose();
        pyroClient.Dispose();
    }

    [Test]
    public async Task Tests()
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
            faker.Lorem.Word(),
            faker.Lorem.Sentence(),
            "master");
        var repository = await pyroClient.CreateGitRepository(createRequest) ??
                         throw new Exception("Repository not created");

        for (var i = 0; i < 3; i++)
        {
            var createLabelRequest = new CreateLabelRequest(
                faker.Lorem.Word(),
                ColorRequest.FromHex(faker.Internet.Color()));
            _ = await pyroClient.CreateLabel(repository.Name, createLabelRequest) ??
                throw new Exception("Label not created");
        }

        return repository.Name;
    }

    private async Task<int> CreateIssue(string repositoryName)
    {
        var labels = await pyroClient.GetLabels(repositoryName);
        if (labels is null || labels.Count == 0)
            throw new Exception("Labels not found");

        var label = new Randomizer().ArrayElement(labels.ToArray());

        var createRequest = new CreateIssueRequest(
            faker.Lorem.Sentence(),
            null,
            [label.Id]);
        var issue = await issueClient.CreateIssue(repositoryName, createRequest);

        Assert.That(issue, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(issue.Title, Is.EqualTo(createRequest.Title));
            Assert.That(issue.Assignee?.Id, Is.EqualTo(createRequest.AssigneeId));
            Assert.That(issue.Labels, Has.Count.EqualTo(1));
            Assert.That(issue.Labels, Has.One.EqualTo(label));
        });

        return issue.IssueNumber;
    }

    private async Task UpdateIssue(string repositoryName, int number)
    {
        var labels = await pyroClient.GetLabels(repositoryName);
        if (labels is null || labels.Count == 0)
            throw new Exception("Labels not found");

        var label = new Randomizer().ArrayElement(labels.ToArray());

        var updateRequest = new UpdateIssueRequest(
            faker.Lorem.Sentence(),
            Guid.Parse("F9BA057A-35B0-4D10-8326-702D8F7EC966"),
            [label.Id]);
        var issue = await issueClient.UpdateIssue(repositoryName, number, updateRequest);

        Assert.That(issue, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(issue.Title, Is.EqualTo(updateRequest.Title));
            Assert.That(issue.Assignee?.Id, Is.EqualTo(updateRequest.AssigneeId));
            Assert.That(issue.Labels, Has.Count.EqualTo(1));
            Assert.That(issue.Labels, Has.One.EqualTo(label));
        });
    }

    private async Task GetIssue(string name, int number)
    {
        var issue = await issueClient.GetIssue(name, number);

        Assert.That(issue, Is.Not.Null);
        Assert.That(issue.IssueNumber, Is.EqualTo(number));
    }

    private async Task DeleteIssue(string name, int number)
        => await issueClient.DeleteIssue(name, number);

    private async Task GetIssuesAfterIssueDelete(string name, int number)
    {
        var issues = await issueClient.GetIssues(name);

        Assert.That(issues, Is.Not.Null);
        Assert.That(issues.Select(x => x.IssueNumber), Does.Not.Contains(number));
    }

    private async Task<Guid> CreateComment(string name, int number)
    {
        var createRequest = new CreateIssueCommentRequest(faker.Lorem.Sentence());
        var comment = await issueClient.CreateComment(name, number, createRequest);

        Assert.That(comment, Is.Not.Null);
        Assert.That(comment.Content, Is.EqualTo(createRequest.Content));

        return comment.Id;
    }

    private async Task UpdateComment(string name, int number, Guid commentId)
    {
        var updateRequest = new UpdateIssueCommentRequest(faker.Lorem.Sentence());
        var comment = await issueClient.UpdateComment(name, number, commentId, updateRequest);

        Assert.That(comment, Is.Not.Null);
        Assert.That(comment.Content, Is.EqualTo(updateRequest.Content));
    }

    private async Task DeleteComment(string name, int number, Guid id)
        => await issueClient.DeleteComment(name, number, id);

    private async Task GetCommentsAfterCommentDelete(string name, int number)
    {
        var comments = await issueClient.GetComments(name, number);

        Assert.That(comments, Is.Empty);
    }
}