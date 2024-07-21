// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Contracts.Responses;

namespace Pyro.ApiTests.Tests;

public class PermissionsTests
{
    [Test]
    public async Task GetPermissions()
    {
        var result = await Api.Get<IReadOnlyList<PermissionResponse>>("/api/permissions");

        Assert.That(result, Is.Not.Empty);
    }
}