// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using NSubstitute;
using Pyro.Domain.Identity.Models;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Identity.UnitTests.Models;

/// <summary>
/// Tests for User class.
/// </summary>
public class UserTests
{
    [Test]
    public void AddRoleToUser()
    {
        var user = new User { Login = "test" };
        var role = new Role
        {
            Name = "Test Role",
        };
        user.AddRole(role);

        Assert.That(user.Roles, Has.Member(role));
    }

    [Test]
    public void AddExistingRoleToUser()
    {
        var user = new User { Login = "test" };
        var role = new Role
        {
            Name = "Test Role",
        };
        user.AddRole(role);
        user.AddRole(role);

        Assert.That(user.Roles, Has.Count.EqualTo(1));
        Assert.That(user.Roles, Has.Member(role));
    }

    [Test]
    public void RemoveAllRolesFromUser()
    {
        var user = new User { Login = "test" };
        var role = new Role
        {
            Name = "Test Role",
        };
        user.AddRole(role);
        user.ClearRoles();

        Assert.That(user.Roles, Is.Empty);
    }

    [Test]
    public void AddAuthTokenToUser()
    {
        var user = new User { Login = "test" };
        var token = AuthenticationToken.Create(Guid.NewGuid(), user, DateTimeOffset.UtcNow);
        user.AddAuthenticationToken(token);

        Assert.That(user.AuthenticationTokens, Has.Member(token));
    }

    [Test]
    public void GetAuthTokenFromUser()
    {
        var user = new User { Login = "test" };
        var token = AuthenticationToken.Create(Guid.NewGuid(), user, DateTimeOffset.UtcNow);
        user.AddAuthenticationToken(token);
        var result = user.GetAuthenticationToken(token.TokenId);

        Assert.That(result, Is.EqualTo(token));
    }

    [Test]
    public void AddAccessTokenToUser()
    {
        var user = new User { Login = "test" };
        var token = new AccessToken
        {
            Name = "Test Token",
            Token = [],
            Salt = [],
            ExpiresAt = DateTimeOffset.UtcNow,
        };
        user.AddAccessToken(token);
        user.AddAccessToken(token);

        Assert.That(user.AccessTokens, Has.Count.EqualTo(1));
        Assert.That(user.AccessTokens, Has.Member(token));
    }

    [Test]
    public void DeleteAccessTokenFromUser()
    {
        var user = new User { Login = "test" };
        var token = new AccessToken
        {
            Name = "Test Token",
            Token = [],
            Salt = [],
            ExpiresAt = DateTimeOffset.UtcNow,
        };
        user.AddAccessToken(token);
        user.DeleteAccessToken(token.Name);

        Assert.That(user.AccessTokens, Is.Empty);
    }

    [Test]
    public void ValidateAccessToken()
    {
        var currentDate = new DateTimeOffset(new DateTime(2024, 01, 01));
        var user = new User { Login = "test" };
        var token = new AccessToken
        {
            Name = "Test Token",
            Token = [],
            Salt = [],
            ExpiresAt = currentDate.AddMonths(1),
        };
        user.AddAccessToken(token);

        var passwordService = Substitute.For<IPasswordService>();
        passwordService.VerifyPassword(string.Empty, token.Token, token.Salt).Returns(true);

        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider.GetUtcNow().Returns(currentDate);

        var result = user.ValidateAccessToken(passwordService, timeProvider, string.Empty);

        Assert.That(result, Is.True);
    }

    [Test]
    public void ValidateExpiredAccessToken()
    {
        var currentDate = new DateTimeOffset(new DateTime(2024, 01, 01));
        var user = new User { Login = "test" };
        var token = new AccessToken
        {
            Name = "Test Token",
            Token = [],
            Salt = [],
            ExpiresAt = currentDate.AddMonths(-1),
        };
        user.AddAccessToken(token);

        var passwordService = Substitute.For<IPasswordService>();
        passwordService.VerifyPassword(string.Empty, token.Token, token.Salt).Returns(true);

        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider.GetUtcNow().Returns(currentDate);

        var result = user.ValidateAccessToken(passwordService, timeProvider, string.Empty);

        Assert.That(result, Is.False);
    }

    [Test]
    public void LockUser()
    {
        var user = new User { Login = "test" };
        user.AddAuthenticationToken(new AuthenticationToken());

        user.Lock();

        Assert.Multiple(() =>
        {
            Assert.That(user.IsLocked, Is.True);
            Assert.That(user.AuthenticationTokens, Is.Empty);
        });
    }

    [Test]
    public void UnlockUser()
    {
        var user = new User { Login = "test" };

        user.Unlock();

        Assert.That(user.IsLocked, Is.False);
    }

    [Test]
    public void ActivateUserIsNull()
    {
        var user = new User { Login = "test" };
        var timeProvider = Substitute.For<TimeProvider>();

        Assert.Throws<ArgumentNullException>(() => user.Activate(timeProvider, null!, [], []));
    }

    [Test]
    public void ActivateUserWithExpiredToken()
    {
        var currentTime = DateTimeOffset.UtcNow;

        var user = new User { Login = "test" };
        var oneTimePassword = new OneTimePassword
        {
            Token = "token",
            ExpiresAt = currentTime.AddDays(-1),
            Purpose = OneTimePasswordPurpose.UserRegistration,
            User = user,
        };
        user.AddOneTimePassword(oneTimePassword);

        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider
            .GetUtcNow()
            .Returns(currentTime);

        Assert.Throws<DomainException>(() => user.Activate(timeProvider, oneTimePassword, [], []));
    }

    [Test]
    public void ActivateLockedUser()
    {
        var currentTime = DateTimeOffset.UtcNow;

        var user = new User { Login = "test" };
        var oneTimePassword = new OneTimePassword
        {
            Token = "token",
            ExpiresAt = currentTime.AddDays(1),
            Purpose = OneTimePasswordPurpose.UserRegistration,
            User = user,
        };
        user.AddOneTimePassword(oneTimePassword);

        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider
            .GetUtcNow()
            .Returns(currentTime);

        Assert.Throws<DomainException>(() => user.Activate(timeProvider, oneTimePassword, [], []));
    }

    [Test]
    public void ActivateUser()
    {
        var currentTime = DateTimeOffset.UtcNow;

        var user = new User { Login = "test" };
        user.Lock();

        var oneTimePassword = new OneTimePassword
        {
            Token = "token",
            ExpiresAt = currentTime.AddDays(1),
            Purpose = OneTimePasswordPurpose.UserRegistration,
            User = user,
        };
        user.AddOneTimePassword(oneTimePassword);

        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider
            .GetUtcNow()
            .Returns(currentTime);

        user.Activate(timeProvider, oneTimePassword, [], []);

        Assert.Multiple(() =>
        {
            Assert.That(user.IsLocked, Is.False);
            Assert.That(user.OneTimePasswords, Is.Empty);
        });
    }
}