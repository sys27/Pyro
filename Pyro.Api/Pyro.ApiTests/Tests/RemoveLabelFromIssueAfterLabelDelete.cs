// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Bogus;
using Pyro.ApiTests.Clients;
using Pyro.Contracts.Requests;
using Pyro.Contracts.Requests.Issues;

namespace Pyro.ApiTests.Tests;

public class RemoveLabelFromIssueAfterLabelDelete
{
    private Faker faker;
    private PyroClient pyroClient;
    private IssueClient issueClient;
    private string repositoryName;
    private Guid labelId;

    [OneTimeSetUp]
    public async Task SetUp()
    {
        faker = new Faker();
        pyroClient = new PyroClient(Api.BaseAddress);
        issueClient = pyroClient.Share<IssueClient>();
        await issueClient.Login();

        var createRequest = new CreateGitRepositoryRequest(
            faker.Lorem.Word(),
            faker.Lorem.Sentence(),
            "master");
        var repository = await pyroClient.CreateGitRepository(createRequest) ??
                         throw new Exception("Repository not created");

        repositoryName = repository.Name;

        var createLabelRequest = new CreateLabelRequest(
            faker.Lorem.Word(),
            ColorRequest.FromHex(faker.Internet.Color()));
        var label = await pyroClient.CreateLabel(repository.Name, createLabelRequest) ??
                    throw new Exception("Label not created");

        labelId = label.Id;

        var createIssueRequest = new CreateIssueRequest(
            faker.Lorem.Word(),
            null,
            [labelId]);
        var issue = await issueClient.CreateIssue(repository.Name, createIssueRequest) ??
                    throw new Exception("Issue not created");
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await issueClient.Logout();
        issueClient.Dispose();
        pyroClient.Dispose();
    }

    [Test]
    public async Task RemoveLabelTest()
    {
        await pyroClient.DeleteLabel(repositoryName, labelId);

        var issues = await issueClient.GetIssues(repositoryName);

        Assert.That(issues, Is.Not.Empty);
        Assert.That(issues, Has.All.Property("Labels").Empty);
    }
}