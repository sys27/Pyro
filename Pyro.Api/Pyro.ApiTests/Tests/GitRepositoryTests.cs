// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Bogus;
using Pyro.ApiTests.Clients;
using Pyro.Contracts.Requests;
using Pyro.Contracts.Requests.Issues;
using Pyro.Contracts.Responses;

namespace Pyro.ApiTests.Tests;

public class GitRepositoryTests
{
    private Faker faker;
    private PyroClient client;

    [OneTimeSetUp]
    public async Task SetUp()
    {
        faker = new Faker();
        client = new PyroClient(Api.BaseAddress);
        await client.Login();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await client.Logout();
        client.Dispose();
    }

    [Test]
    public async Task Tests()
    {
        var name = await CreateGitRepository();
        var repository = await GetGitRepository(name);
        await GetBranches(repository.Name);
        await GetTree(repository.Name);
        await GetFile(repository.Name);
        await GetRepositories();

        var tag = await CreateTag(repository.Name);
        tag = await UpdateTag(repository.Name, tag.Id);
        await GetTag(repository.Name, tag.Id);
        await GetTags(repository.Name, tag);
    }

    private async Task<string> CreateGitRepository()
    {
        var createRequest = new CreateGitRepositoryRequest(
            faker.Lorem.Word(),
            faker.Lorem.Sentence(),
            "master");
        var repository = await client.CreateGitRepository(createRequest);

        Assert.That(repository, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(repository.Name, Is.EqualTo(createRequest.Name));
            Assert.That(repository.Description, Is.EqualTo(createRequest.Description));
            Assert.That(repository.DefaultBranch, Is.EqualTo(createRequest.DefaultBranch));
            Assert.That(repository.Status, Is.EqualTo(GitRepositoryStatusResponse.New));
        });

        return repository.Name;
    }

    private async Task<GitRepositoryResponse> GetGitRepository(string name)
    {
        var repository = await client.GetUntil<GitRepositoryResponse>(
            $"/api/repositories/{name}",
            x => x?.Status == GitRepositoryStatusResponse.Initialized);

        Assert.That(repository, Is.Not.Null);
        Assert.That(repository.Name, Is.EqualTo(name));

        return repository;
    }

    private async Task GetBranches(string name)
    {
        var branches = await client.GetBranches(name);

        Assert.That(branches, Is.Not.Empty);
        Assert.That(branches, Has.Count.EqualTo(1));

        var branch = branches[0];
        Assert.Multiple(() =>
        {
            Assert.That(branch.Name, Is.EqualTo("master"));
            Assert.That(branch.IsDefault, Is.True);
        });
    }

    private async Task GetTree(string name)
    {
        var tree = await client.GetTree(name);

        Assert.That(tree, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(tree.Commit.Message, Is.EqualTo("Initial commit"));
            Assert.That(tree.Items, Is.Not.Empty);
            Assert.That(tree.CommitsCount, Is.EqualTo(1));
        });
    }

    private async Task GetFile(string name)
    {
        var file = await client.GetFile($"/api/repositories/{name}/file/master/README.md");

        Assert.That(file, Is.EqualTo($"# {name}"));
    }

    private async Task GetRepositories()
    {
        var repositories = await client.GetGitRepositories();

        Assert.That(repositories, Is.Not.Empty);
    }

    private async Task<TagResponse> CreateTag(string repositoryName)
    {
        var request = new CreateTagRequest(faker.Lorem.Word(), ColorRequest.FromHex(faker.Internet.Color()));
        var tag = await client.CreateTag(repositoryName, request);

        Assert.That(tag, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(tag.Name, Is.EqualTo(request.Name));
            Assert.That(tag.Color.R, Is.EqualTo(request.Color.R));
            Assert.That(tag.Color.G, Is.EqualTo(request.Color.G));
            Assert.That(tag.Color.B, Is.EqualTo(request.Color.B));
        });

        return tag;
    }

    private async Task<TagResponse> UpdateTag(string repositoryName, Guid id)
    {
        var request = new UpdateTagRequest(faker.Lorem.Word(), ColorRequest.FromHex(faker.Internet.Color()));
        var tag = await client.UpdateTag(repositoryName, id, request);

        Assert.That(tag, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(tag.Name, Is.EqualTo(request.Name));
            Assert.That(tag.Color.R, Is.EqualTo(request.Color.R));
            Assert.That(tag.Color.G, Is.EqualTo(request.Color.G));
            Assert.That(tag.Color.B, Is.EqualTo(request.Color.B));
        });

        return tag;
    }

    private async Task GetTag(string repositoryName, Guid id)
    {
        var tag = await client.GetTag(repositoryName, id);

        Assert.That(tag, Is.Not.Null);
        Assert.That(tag.Id, Is.EqualTo(id));
    }

    private async Task GetTags(string repositoryName, TagResponse tag)
    {
        var tags = await client.GetTags(repositoryName);

        Assert.That(tags, Is.Not.Empty);
        Assert.That(tags, Has.Count.EqualTo(1));
        Assert.That(tags, Has.One.EqualTo(tag));
    }
}