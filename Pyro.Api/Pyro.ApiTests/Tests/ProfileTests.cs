// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Bogus;
using Pyro.ApiTests.Clients;
using Pyro.Contracts.Requests;
using Pyro.Contracts.Requests.Identity;

namespace Pyro.ApiTests.Tests;

public class ProfileTests
{
    private Faker faker;
    private PyroClient client;
    private IdentityClient identityClient;

    [OneTimeSetUp]
    public async Task SetUp()
    {
        faker = new Faker();
        client = new PyroClient(Api.BaseAddress);
        identityClient = client.Share<IdentityClient>();
        await client.Login();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await client.Logout();
        identityClient.Dispose();
        client.Dispose();
    }

    [Test]
    public async Task UpdateGetProfile()
    {
        var updateRequest = new UpdateUserProfileRequest(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString());
        await client.UpdateProfile(updateRequest);

        var profile = await client.GetProfile();

        Assert.That(profile, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(profile.Name, Is.EqualTo(updateRequest.Name));
            Assert.That(profile.Status, Is.EqualTo(updateRequest.Status));
        });
    }

    [Test]
    public async Task GetProfileOfNewlyCreatedUser()
    {
        var login = faker.Internet.Email();
        var request = new CreateUserRequest(
            login,
            ["Admin"]);
        await identityClient.CreateUser(request);

        var message = Api.Smtp.WaitForMessage(x => x.To == login) ??
                      throw new InvalidOperationException("The message was not found.");
        var token = message.GetToken();
        var password = faker.Random.Hash();
        var activateUserRequest = new ActivateUserRequest(token, password);
        await identityClient.ActivateUser(activateUserRequest);

        using var newUserClient = new PyroClient(Api.BaseAddress);

        await newUserClient.Login(request.Login, password);

        var profile = await newUserClient.GetProfile();

        Assert.That(profile, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(profile.Name, Is.EqualTo(request.Login));
            Assert.That(profile.Status, Is.Null);
        });

        await newUserClient.Logout();
    }
}