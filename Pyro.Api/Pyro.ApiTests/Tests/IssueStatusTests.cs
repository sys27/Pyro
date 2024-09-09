// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Bogus;
using Pyro.ApiTests.Clients;
using Pyro.Contracts.Requests;
using Pyro.Contracts.Requests.Issues;
using Pyro.Contracts.Responses.Issues;

namespace Pyro.ApiTests.Tests;

public class IssueStatusTests
{
    private Faker faker;
    private PyroClient pyroClient;
    private IssueClient issueClient;
    private string repositoryName;

    [OneTimeSetUp]
    public async Task Setup()
    {
        faker = new Faker();
        pyroClient = new PyroClient(Api.BaseAddress);
        issueClient = pyroClient.Share<IssueClient>();
        await issueClient.Login();

        var createRequest = new CreateGitRepositoryRequest(
            faker.Random.Hash(),
            faker.Lorem.Sentence(),
            "master");
        var repository = await pyroClient.CreateGitRepository(createRequest) ??
                         throw new Exception("Repository not created");

        repositoryName = repository.Name;
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
        var status = await CreateIssueStatus();
        status = await UpdateIssueStatus(status.Id);
        status = await GetIssueStatus(status.Id);

        var transition = await CreateIssueStatusTransition(status.Id);
        await DeleteIssueStatusTransition(transition.Id);

        await DisableIssueStatus(status.Id);
        await EnableIssueStatus(status.Id);
    }

    private async Task<IssueStatusResponse> CreateIssueStatus()
    {
        var request = new CreateIssueStatusRequest(
            faker.Random.Hash(),
            ColorRequest.FromHex(faker.Internet.Color()));
        var status = await issueClient.CreateIssueStatus(repositoryName, request);

        Assert.That(status, Is.Not.Null);
        Assert.That(status.Name, Is.EqualTo(request.Name));

        return status;
    }

    private async Task<IssueStatusResponse> UpdateIssueStatus(Guid id)
    {
        var request = new UpdateIssueStatusRequest(
            faker.Random.Hash(),
            ColorRequest.FromHex(faker.Internet.Color()));
        var status = await issueClient.UpdateIssueStatus(repositoryName, id, request);

        Assert.That(status, Is.Not.Null);
        Assert.That(status.Name, Is.EqualTo(request.Name));

        return status;
    }

    private async Task<IssueStatusResponse> GetIssueStatus(Guid id)
    {
        var status = await issueClient.GetIssueStatus(repositoryName, id);

        Assert.That(status, Is.Not.Null);

        return status;
    }

    private async Task DisableIssueStatus(Guid id)
    {
        await issueClient.DisableIssueStatus(repositoryName, id);
        var status = await issueClient.GetIssueStatus(repositoryName, id);

        Assert.That(status, Is.Not.Null);
        Assert.That(status.IsDisabled, Is.True);
    }

    private async Task EnableIssueStatus(Guid id)
    {
        await issueClient.EnableIssueStatus(repositoryName, id);
        var status = await issueClient.GetIssueStatus(repositoryName, id);

        Assert.That(status, Is.Not.Null);
        Assert.That(status.IsDisabled, Is.False);
    }

    private async Task<IssueStatusTransitionResponse> CreateIssueStatusTransition(Guid fromId)
    {
        var toStatus = await CreateIssueStatus();

        var request = new CreateIssueStatusTransitionRequest(fromId, toStatus.Id);
        var transition = await issueClient.CreateIssueStatusTransition(repositoryName, request);

        Assert.That(transition, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(transition.From.Id, Is.EqualTo(fromId));
            Assert.That(transition.To.Id, Is.EqualTo(toStatus.Id));
        });

        return transition;
    }

    private async Task DeleteIssueStatusTransition(Guid id)
    {
        await issueClient.DeleteIssueStatusTransition(repositoryName, id);

        var transitions = await issueClient.GetIssueStatusTransitions(repositoryName);

        Assert.That(transitions, Has.None.Matches<IssueStatusTransitionResponse>(x => x.Id == id));
    }
}