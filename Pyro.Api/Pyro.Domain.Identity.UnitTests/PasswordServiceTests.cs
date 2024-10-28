// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Identity.Models;

namespace Pyro.Domain.Identity.UnitTests;

public class PasswordServiceTests
{
    [Test]
    public void GenerateOneTimePasswordForTest()
    {
        var user = new User { Login = "test" };

        var passwordService = new PasswordService(TimeProvider.System);
        var otp = passwordService.GenerateOneTimePasswordFor(user);

        Assert.Multiple(() =>
        {
            Assert.That(user.OneTimePasswords, Has.Count.EqualTo(1));
            Assert.That(user.OneTimePasswords[0], Is.EqualTo(otp));

            Assert.That(otp.Token, Is.Not.Empty);
            Assert.That(otp.ExpiresAt, Is.GreaterThan(TimeProvider.System.GetUtcNow()));
            Assert.That(otp.Purpose, Is.EqualTo(OneTimePasswordPurpose.UserRegistration));
            Assert.That(otp.User, Is.EqualTo(user));
        });
    }
}