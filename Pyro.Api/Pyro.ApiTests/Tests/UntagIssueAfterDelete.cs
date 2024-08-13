// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Bogus;
using Pyro.ApiTests.Clients;
using Pyro.Contracts.Requests;
using Pyro.Contracts.Requests.Issues;

namespace Pyro.ApiTests.Tests;

public class UntagIssueAfterDelete
{
    private Faker faker;
    private PyroClient pyroClient;
    private IssueClient issueClient;
    private string repositoryName;
    private Guid tagId;

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

        var createTagRequest = new CreateTagRequest(
            faker.Lorem.Word(),
            ColorRequest.FromHex(faker.Internet.Color()));
        var tag = await pyroClient.CreateTag(repository.Name, createTagRequest) ??
                  throw new Exception("Tag not created");

        tagId = tag.Id;

        var createIssueRequest = new CreateIssueRequest(
            faker.Lorem.Word(),
            null,
            [tagId]);
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
    public async Task UntagTest()
    {
        await pyroClient.DeleteTag(repositoryName, tagId);

        var issues = await issueClient.GetIssues(repositoryName);

        Assert.That(issues, Is.Not.Empty);
        Assert.That(issues, Has.All.Property("Tags").Empty);
    }
}