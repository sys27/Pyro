// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Bogus;
using Pyro.ApiTests.Clients;
using Pyro.Contracts.Requests.Identity;

namespace Pyro.ApiTests.Tests;

public class ResetPasswordTests
{
    private Faker faker;
    private IdentityClient client;
    private string login;
    private string email;

    [OneTimeSetUp]
    public async Task SetUp()
    {
        faker = new Faker();
        client = new IdentityClient(Api.BaseAddress);
        await client.Login();

        login = faker.Random.Hash(32);
        email = faker.Internet.Email();
        var createUserRequest = new CreateUserRequest(login, email, ["User"]);
        var user = await client.CreateUser(createUserRequest);
        Assert.That(user, Is.Not.Null);

        var message = Api.Smtp.WaitForMessage(x => x.To == email) ??
                      throw new InvalidOperationException("The message was not found.");
        var token = message.GetToken();
        var password = faker.Random.Hash();
        var activateUserRequest = new ActivateUserRequest(token, password);
        await client.ActivateUser(activateUserRequest);
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        client.Dispose();
    }

    [Test]
    public async Task Tests()
    {
        await client.ForgotPassword(new ForgotPasswordRequest(login));
        var message = Api.Smtp.WaitForMessage(x => x.To == email) ??
                      throw new InvalidOperationException("The message was not found.");
        var token = message.GetToken();

        var password = faker.Random.Hash();
        await client.ResetPassword(new ResetPasswordRequest(token, password));

        await client.Logout();
        await client.Login(login, password);
    }
}