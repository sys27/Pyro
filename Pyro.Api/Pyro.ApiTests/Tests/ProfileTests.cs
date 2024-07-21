// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Contracts.Requests;
using Pyro.Contracts.Responses;

namespace Pyro.ApiTests.Tests;

public class ProfileTests
{
    [Test]
    public async Task UpdateGetProfile()
    {
        var updateRequest = new UpdateUserProfileRequest(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString());
        await Api.Put("/api/profile", updateRequest);

        var profile = await Api.Get<UserProfileResponse>("/api/profile");

        Assert.That(profile, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(profile.Name, Is.EqualTo(updateRequest.Name));
            Assert.That(profile.Status, Is.EqualTo(updateRequest.Status));
        });
    }
}