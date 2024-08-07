// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Bogus;
using Pyro.ApiTests.Clients;
using Pyro.Contracts.Requests;
using Pyro.Contracts.Responses;

namespace Pyro.ApiTests.Tests;

public class GitRepositoryTests
{
    private PyroClient client;

    [OneTimeSetUp]
    public async Task SetUp()
    {
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
    }

    private async Task<string> CreateGitRepository()
    {
        var createRequest = new CreateGitRepositoryRequest(
            new Faker().Lorem.Word(),
            new Faker().Lorem.Sentence(),
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
}