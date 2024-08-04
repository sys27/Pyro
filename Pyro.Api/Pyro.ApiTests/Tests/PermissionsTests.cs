// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.ApiTests.Clients;

namespace Pyro.ApiTests.Tests;

public class PermissionsTests
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
    public async Task GetPermissions()
    {
        var result = await client.GetPermissions();

        Assert.That(result, Is.Not.Empty);
    }
}