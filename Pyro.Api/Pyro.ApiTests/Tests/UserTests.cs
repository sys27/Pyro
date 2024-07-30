// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Net;
using Pyro.Contracts.Requests.Identity;
using Pyro.Contracts.Responses.Identity;

namespace Pyro.ApiTests.Tests;

public class UserTests
{
    [Test]
    public async Task GetUsers()
    {
        var result = await Api.Get<IReadOnlyList<UserResponse>>("/api/users");

        Assert.That(result, Is.Not.Empty);
    }

    [Test]
    public async Task GetUserByLogin()
    {
        const string login = "pyro";
        var result = await Api.Get<UserResponse>($"/api/users/{login}");

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Login, Is.EqualTo(login));
    }

    [Test]
    public async Task GetMissingUserByLogin()
    {
        const string login = "missing";
        using var response = await Api.Get($"/api/users/{login}");

        Assert.That(response, Is.Not.Null);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task CreateGetUpdateUser()
    {
        var createRequest = new CreateUserRequest(
            Guid.NewGuid().ToString().Replace("-", string.Empty),
            "password",
            ["Admin"]);
        await Api.Post("/api/users", createRequest);

        var user = await Api.Get<UserResponse>($"/api/users/{createRequest.Login}");

        Assert.That(user, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(user.Login, Is.EqualTo(createRequest.Login));
            Assert.That(user.IsLocked, Is.False);
            Assert.That(user.Roles.Select(x => x.Name), Is.EquivalentTo(createRequest.Roles));
        });

        var updateRequest = new UpdateUserRequest(["User"]);
        user = await Api.Put<UserResponse>($"/api/users/{createRequest.Login}", updateRequest);

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
        var token = await Api.Post<AccessTokenResponse>("/api/users/access-tokens", createRequest);

        Assert.That(token, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(token.Name, Is.EqualTo(createRequest.Name));
            Assert.That(token.ExpiresAt, Is.EqualTo(createRequest.ExpiresAt));
        });

        var tokens = await Api.Get<IReadOnlyList<AccessTokenResponse>>("/api/users/access-tokens");

        Assert.That(tokens, Is.Not.Empty);
        Assert.That(tokens, Contains.Item(token));

        await Api.Delete($"/api/users/access-tokens/{token.Name}");

        tokens = await Api.Get<IReadOnlyList<AccessTokenResponse>>("/api/users/access-tokens");

        Assert.That(tokens, Is.Empty);
    }
}