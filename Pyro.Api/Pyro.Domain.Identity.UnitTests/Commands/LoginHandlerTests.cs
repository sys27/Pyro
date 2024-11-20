// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using NSubstitute;
using Pyro.Domain.Identity.Commands;
using Pyro.Domain.Identity.Models;

namespace Pyro.Domain.Identity.UnitTests.Commands;

public class LoginHandlerTests
{
    [Test]
    public async Task LoginWithInvalidUser()
    {
        var command = new LoginCommand("test", "password");

        var logger = Substitute.For<ILogger<LoginHandler>>();
        var timeProvider = Substitute.For<TimeProvider>();
        var repository = Substitute.For<IUserRepository>();
        var passwordService = Substitute.For<IPasswordService>();
        var tokenService = Substitute.For<ITokenService>();
        var handler = new LoginHandler(logger, timeProvider, repository, passwordService, tokenService);

        var result = await handler.Handle(command);

        Assert.That(result, Is.EqualTo(LoginResult.Fail()));
    }

    [Test]
    public async Task LoginWithLockedUser()
    {
        var command = new LoginCommand("test", "password");
        var user = new User
        {
            Login = "test",
            DisplayName = "test",
            Email = "test@localhost.local",
        };
        user.Lock();

        var logger = Substitute.For<ILogger<LoginHandler>>();
        var timeProvider = Substitute.For<TimeProvider>();
        var repository = Substitute.For<IUserRepository>();
        repository
            .GetUserByLogin(command.Login)
            .Returns(user);
        var passwordService = Substitute.For<IPasswordService>();
        var tokenService = Substitute.For<ITokenService>();
        var handler = new LoginHandler(logger, timeProvider, repository, passwordService, tokenService);

        var result = await handler.Handle(command);

        Assert.That(result, Is.EqualTo(LoginResult.Fail()));
    }

    [Test]
    public async Task LoginWithExpiredPassword()
    {
        var currentDate = DateTimeOffset.UtcNow;
        var command = new LoginCommand("test", "password");
        var user = new User
        {
            Login = "test",
            DisplayName = "test",
            Email = "test@localhost.local",
            PasswordExpiresAt = currentDate.AddDays(-1),
        };

        var logger = Substitute.For<ILogger<LoginHandler>>();
        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider.GetUtcNow().Returns(currentDate);
        var repository = Substitute.For<IUserRepository>();
        repository
            .GetUserByLogin(command.Login)
            .Returns(user);
        var passwordService = Substitute.For<IPasswordService>();
        var tokenService = Substitute.For<ITokenService>();
        var handler = new LoginHandler(logger, timeProvider, repository, passwordService, tokenService);

        var result = await handler.Handle(command);

        Assert.That(result, Is.EqualTo(LoginResult.Fail()));
    }

    [Test]
    public async Task LoginWithInvalidCredentials()
    {
        var command = new LoginCommand("test", "password");
        var user = new User
        {
            Login = "test",
            DisplayName = "test",
            Email = "test@localhost.local",
        };

        var logger = Substitute.For<ILogger<LoginHandler>>();
        var timeProvider = Substitute.For<TimeProvider>();
        var repository = Substitute.For<IUserRepository>();
        repository
            .GetUserByLogin(command.Login)
            .Returns(user);
        var passwordService = Substitute.For<IPasswordService>();
        passwordService
            .VerifyPassword(command.Password, user.Password, user.Salt)
            .Returns(false);
        var tokenService = Substitute.For<ITokenService>();
        var handler = new LoginHandler(logger, timeProvider, repository, passwordService, tokenService);

        var result = await handler.Handle(command);

        Assert.That(result, Is.EqualTo(LoginResult.Fail()));
    }

    [Test]
    public async Task LoginWithValidCredentials()
    {
        var command = new LoginCommand("test", "password");
        var user = new User
        {
            Login = "test",
            DisplayName = "test",
            Email = "test@localhost.local",
        };
        var jwtTokenPair = new JwtTokenPair(
            new Token(Guid.NewGuid(), "access", DateTimeOffset.UtcNow),
            new Token(Guid.NewGuid(), "refresh", DateTimeOffset.UtcNow));

        var logger = Substitute.For<ILogger<LoginHandler>>();
        var timeProvider = Substitute.For<TimeProvider>();
        var repository = Substitute.For<IUserRepository>();
        repository
            .GetUserByLogin(command.Login)
            .Returns(user);
        var passwordService = Substitute.For<IPasswordService>();
        passwordService
            .VerifyPassword(command.Password, user.Password, user.Salt)
            .Returns(true);
        var tokenService = Substitute.For<ITokenService>();
        tokenService
            .GenerateTokenPair(user)
            .Returns(jwtTokenPair);
        var handler = new LoginHandler(logger, timeProvider, repository, passwordService, tokenService);

        var result = await handler.Handle(command);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(LoginResult.Success(jwtTokenPair)));
            Assert.That(user.AuthenticationTokens, Is.Not.Empty);
        });
    }
}