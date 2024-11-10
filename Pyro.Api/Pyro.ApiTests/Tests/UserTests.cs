// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Bogus;
using Pyro.ApiTests.Clients;
using Pyro.Contracts.Requests.Identity;

namespace Pyro.ApiTests.Tests;

public class UserTests
{
    private Faker faker;
    private IdentityClient client;

    [OneTimeSetUp]
    public async Task SetUp()
    {
        faker = new Faker();
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
        const string login = "pyro@localhost.local";
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
        var login = faker.Internet.Email();
        var createRequest = new CreateUserRequest(
            login,
            ["Admin"]);
        await client.CreateUser(createRequest);

        var message = Api.Smtp.WaitForMessage(x => x.To == login) ??
                      throw new InvalidOperationException("The message was not found.");
        var token = message.GetToken();
        var activateUserRequest = new ActivateUserRequest(token, faker.Random.Hash());
        await client.ActivateUser(activateUserRequest);

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
    public async Task ChangePassword()
    {
        var login = faker.Internet.Email();
        var createRequest = new CreateUserRequest(
            login,
            ["Admin"]);
        await client.CreateUser(createRequest);

        var message = Api.Smtp.WaitForMessage(x => x.To == login) ??
                      throw new InvalidOperationException("The message was not found.");
        var token = message.GetToken();
        var password = faker.Random.Hash();
        var activateUserRequest = new ActivateUserRequest(token, password);
        await client.ActivateUser(activateUserRequest);

        using var identityClient = new IdentityClient(Api.BaseAddress);
        await identityClient.Login(login, password);

        var newPassword = faker.Random.Hash();
        var changePasswordRequest = new ChangePasswordRequest(password, newPassword);
        await identityClient.ChangePassword(changePasswordRequest);
        await identityClient.Logout();

        await identityClient.Login(login, newPassword);
        var user = identityClient.GetUser(login);

        Assert.That(user, Is.Not.Null);
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