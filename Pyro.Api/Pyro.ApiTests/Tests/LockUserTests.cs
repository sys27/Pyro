// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Net;
using Bogus;
using Pyro.ApiTests.Clients;
using Pyro.Contracts.Requests.Identity;

namespace Pyro.ApiTests.Tests;

public class LockUserTests
{
    private Faker faker;
    private PyroClient pyroClient;
    private IdentityClient identityClient;
    private string login;
    private string password;

    [OneTimeSetUp]
    public async Task SetUp()
    {
        faker = new Faker();
        pyroClient = new PyroClient(Api.BaseAddress);
        identityClient = pyroClient.Share<IdentityClient>();
        await pyroClient.Login();

        login = faker.Random.Hash(32);
        password = faker.Random.Hash();

        var createUserRequest = new CreateUserRequest(login, password, ["User"]);
        var user = await identityClient.CreateUser(createUserRequest);
        Assert.That(user, Is.Not.Null);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await pyroClient.Logout();
        identityClient.Dispose();
        pyroClient.Dispose();
    }

    [Test]
    public async Task Tests()
    {
        await identityClient.LockUser(login);

        var user = await identityClient.GetUser(login);
        Assert.That(user, Is.Not.Null);
        Assert.That(user.IsLocked, Is.True);

        var ex = Assert.ThrowsAsync<HttpRequestException>(() => identityClient.Login(login, password));
        Assert.That(ex.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        await identityClient.UnlockUser(login);

        user = await identityClient.GetUser(login);
        Assert.That(user, Is.Not.Null);
        Assert.That(user.IsLocked, Is.False);

        await identityClient.Login(login, password);
        Assert.That(identityClient.User, Is.Not.Null);
        Assert.That(identityClient.User.Login, Is.EqualTo(login));
    }
}