// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Bogus;
using Pyro.ApiTests.Clients;
using Pyro.Contracts.Requests;
using Pyro.Contracts.Requests.Issues;
using Pyro.Contracts.Responses;
using Pyro.Contracts.Responses.Issues;

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
        var repositoryName = await CreateRepository();
        var number = await CreateIssue(repositoryName);
        await UpdateIssue(repositoryName, number);
        await LockIssue(repositoryName, number);
        await UnlockIssue(repositoryName, number);
        await GetIssue(repositoryName, number);

        var commentId = await CreateComment(repositoryName, number);
        await UpdateComment(repositoryName, number, commentId);
        await DeleteComment(repositoryName, number, commentId);
        await GetCommentsAfterCommentDelete(repositoryName, number);

        await DeleteIssue(repositoryName, number);
        await GetIssuesAfterIssueDelete(repositoryName, number);
    }

    private async Task<string> CreateRepository()
    {
        var createRequest = new CreateGitRepositoryRequest(
            faker.Random.Hash(),
            faker.Lorem.Sentence(),
            "master");
        var repository = await pyroClient.CreateGitRepository(createRequest) ??
                         throw new Exception("Repository not created");

        for (var i = 0; i < 3; i++)
        {
            var createLabelRequest = new CreateLabelRequest(
                faker.Random.Hash(),
                ColorRequest.FromHex(faker.Internet.Color()));
            _ = await pyroClient.CreateLabel(repository.Name, createLabelRequest) ??
                throw new Exception("Label not created");
        }

        var statusIds = new List<Guid>(3);
        for (var i = 0; i < 3; i++)
        {
            var createStatusRequest = new CreateIssueStatusRequest(
                faker.Random.Hash(),
                ColorRequest.FromHex(faker.Internet.Color()));
            var status = await issueClient.CreateIssueStatus(repository.Name, createStatusRequest) ??
                         throw new Exception("Status not created");

            foreach (var statusId in statusIds)
            {
                var createTransitionRequest = new CreateIssueStatusTransitionRequest(statusId, status.Id);
                _ = await issueClient.CreateIssueStatusTransition(repository.Name, createTransitionRequest) ??
                    throw new Exception("Transition not created");

                createTransitionRequest = new CreateIssueStatusTransitionRequest(status.Id, statusId);
                _ = await issueClient.CreateIssueStatusTransition(repository.Name, createTransitionRequest) ??
                    throw new Exception("Transition not created");
            }

            statusIds.Add(status.Id);
        }

        return repository.Name;
    }

    private async Task<LabelResponse> GetRandomLabel(string repositoryName)
    {
        var labels = await pyroClient.GetLabels(repositoryName);
        if (labels is null || labels.Count == 0)
            throw new Exception("Labels not found");

        return new Randomizer().ArrayElement(labels.ToArray());
    }

    private async Task<IssueStatusResponse> GetRandomStatus(string repositoryName)
    {
        var statuses = await issueClient.GetIssueStatuses(repositoryName);
        if (statuses is null || statuses.Count == 0)
            throw new Exception("Statuses not found");

        return new Randomizer().ArrayElement(statuses.ToArray());
    }

    private async Task<int> CreateIssue(string repositoryName)
    {
        var label = await GetRandomLabel(repositoryName);
        var status = await GetRandomStatus(repositoryName);

        var createRequest = new CreateIssueRequest(
            faker.Lorem.Sentence(),
            null,
            status.Id,
            [label.Id]);
        var issue = await issueClient.CreateIssue(repositoryName, createRequest);

        Assert.That(issue, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(issue.Title, Is.EqualTo(createRequest.Title));
            Assert.That(issue.Assignee?.Id, Is.EqualTo(createRequest.AssigneeId));
            Assert.That(issue.Labels, Has.Count.EqualTo(1));
            Assert.That(issue.Labels, Has.One.EqualTo(label));
            Assert.That(issue.Status.Id, Is.EqualTo(status.Id));
        });

        return issue.IssueNumber;
    }

    private async Task UpdateIssue(string repositoryName, int number)
    {
        var label = await GetRandomLabel(repositoryName);
        var status = await GetRandomStatus(repositoryName);

        var updateRequest = new UpdateIssueRequest(
            faker.Lorem.Sentence(),
            Guid.Parse("F9BA057A-35B0-4D10-8326-702D8F7EC966"),
            status.Id,
            [label.Id]);
        var issue = await issueClient.UpdateIssue(repositoryName, number, updateRequest);

        Assert.That(issue, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(issue.Title, Is.EqualTo(updateRequest.Title));
            Assert.That(issue.Assignee?.Id, Is.EqualTo(updateRequest.AssigneeId));
            Assert.That(issue.Labels, Has.Count.EqualTo(1));
            Assert.That(issue.Labels, Has.One.EqualTo(label));
            Assert.That(issue.Status.Id, Is.EqualTo(status.Id));
        });
    }

    private async Task LockIssue(string repositoryName, int number)
    {
        await issueClient.LockIssue(repositoryName, number);

        var issue = await issueClient.GetIssue(repositoryName, number);

        Assert.That(issue, Is.Not.Null);
        Assert.That(issue.IsLocked, Is.True);
    }

    private async Task UnlockIssue(string repositoryName, int number)
    {
        await issueClient.UnlockIssue(repositoryName, number);

        var issue = await issueClient.GetIssue(repositoryName, number);

        Assert.That(issue, Is.Not.Null);
        Assert.That(issue.IsLocked, Is.False);
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