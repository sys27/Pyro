// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.ApiTests.Clients;
using Pyro.Contracts.Requests.Identity;

namespace Pyro.ApiTests.Tests;

public class UserTests
{
    private IdentityClient client;

    [OneTimeSetUp]
    public async Task SetUp()
    {
        client = new IdentityClient(Api.BaseAddress);
        await client.Login();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await client.Logout();
        client.Dispose();
    }

    [Test]
    public async Task GetUsers()
    {
        var result = await client.GetUsers();

        Assert.That(result, Is.Not.Empty);
    }

    [Test]
    public async Task GetUserByLogin()
    {
        const string login = "pyro";
        var result = await client.GetUser(login);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Login, Is.EqualTo(login));
    }

    [Test]
    public async Task GetMissingUserByLogin()
    {
        const string login = "missing";
        var result = await client.GetUser(login);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task CreateGetUpdateUser()
    {
        var createRequest = new CreateUserRequest(
            Guid.NewGuid().ToString().Replace("-", string.Empty),
            "password",
            ["Admin"]);
        await client.CreateUser(createRequest);

        var user = await client.GetUser(createRequest.Login);

        Assert.That(user, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(user.Login, Is.EqualTo(createRequest.Login));
            Assert.That(user.IsLocked, Is.False);
            Assert.That(user.Roles.Select(x => x.Name), Is.EquivalentTo(createRequest.Roles));
        });

        var updateRequest = new UpdateUserRequest(["User"]);
        user = await client.UpdateUser(createRequest.Login, updateRequest);

        Assert.That(user, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(user.Login, Is.EqualTo(createRequest.Login));
            Assert.That(user.IsLocked, Is.False);
            Assert.That(user.Roles.Select(x => x.Name), Is.EquivalentTo(updateRequest.Roles));
        });
    }

    [Test]
    public async Task CreateGetDeleteAccessToken()
    {
        var createRequest = new CreateAccessTokenRequest(
            Guid.NewGuid().ToString(),
            DateTimeOffset.UtcNow.AddMonths(1));
        var token = await client.CreateAccessToken(createRequest);

        Assert.That(token, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(token.Name, Is.EqualTo(createRequest.Name));
            Assert.That(token.ExpiresAt, Is.EqualTo(createRequest.ExpiresAt));
        });

        var tokens = await client.GetAccessTokens();

        Assert.That(tokens, Is.Not.Empty);
        Assert.That(tokens, Contains.Item(token));

        await client.DeleteAccessToken(token.Name);

        tokens = await client.GetAccessTokens();

        Assert.That(tokens, Is.Empty);
    }
}