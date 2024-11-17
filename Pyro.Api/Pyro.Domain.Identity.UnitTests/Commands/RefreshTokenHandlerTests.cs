// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using NSubstitute;
using Pyro.Domain.Identity.Commands;
using Pyro.Domain.Identity.Models;

namespace Pyro.Domain.Identity.UnitTests.Commands;

public class RefreshTokenHandlerTests
{
    [Test]
    public async Task RefreshTokenWithInvalidUser()
    {
        var command = new RefreshToken("token");
        var jwtToken = new JwtToken
        {
            TokenId = Guid.NewGuid(),
            IssuedAt = 0,
            ExpiresAt = 0,
            UserId = Guid.NewGuid(),
            Login = "test",
        };

        var logger = Substitute.For<ILogger<RefreshTokenHandler>>();
        var userRepository = Substitute.For<IUserRepository>();
        var tokenService = Substitute.For<ITokenService>();
        tokenService
            .DecodeTokenId(command.Token)
            .Returns(jwtToken);
        var timeProvider = Substitute.For<TimeProvider>();
        var handler = new RefreshTokenHandler(logger, userRepository, tokenService, timeProvider);

        var result = await handler.Handle(command);

        Assert.That(result, Is.EqualTo(RefreshTokenResult.Fail()));
    }

    [Test]
    public async Task RefreshTokenWithLockedUser()
    {
        var command = new RefreshToken("token");
        var user = new User
        {
            Login = "test",
            Profile = new UserProfile { Name = "test" },
        };
        user.Lock();

        var jwtToken = new JwtToken
        {
            TokenId = Guid.NewGuid(),
            IssuedAt = 0,
            ExpiresAt = 0,
            UserId = user.Id,
            Login = user.Login,
        };

        var logger = Substitute.For<ILogger<RefreshTokenHandler>>();
        var userRepository = Substitute.For<IUserRepository>();
        userRepository
            .GetUserById(jwtToken.UserId)
            .Returns(user);
        var tokenService = Substitute.For<ITokenService>();
        tokenService
            .DecodeTokenId(command.Token)
            .Returns(jwtToken);
        var timeProvider = Substitute.For<TimeProvider>();
        var handler = new RefreshTokenHandler(logger, userRepository, tokenService, timeProvider);

        var result = await handler.Handle(command);

        Assert.That(result, Is.EqualTo(RefreshTokenResult.Fail()));
    }

    [Test]
    public async Task RefreshTokenWithExpiredPassword()
    {
        var currentDate = DateTimeOffset.UtcNow;
        var command = new RefreshToken("token");
        var user = new User
        {
            Login = "test",
            Profile = new UserProfile { Name = "test" },
            PasswordExpiresAt = currentDate.AddDays(-1),
        };
        var jwtToken = new JwtToken
        {
            TokenId = Guid.NewGuid(),
            IssuedAt = 0,
            ExpiresAt = 0,
            UserId = user.Id,
            Login = user.Login,
        };

        var logger = Substitute.For<ILogger<RefreshTokenHandler>>();
        var userRepository = Substitute.For<IUserRepository>();
        userRepository
            .GetUserById(jwtToken.UserId)
            .Returns(user);
        var tokenService = Substitute.For<ITokenService>();
        tokenService
            .DecodeTokenId(command.Token)
            .Returns(jwtToken);
        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider.GetUtcNow().Returns(currentDate);
        var handler = new RefreshTokenHandler(logger, userRepository, tokenService, timeProvider);

        var result = await handler.Handle(command);

        Assert.That(result, Is.EqualTo(RefreshTokenResult.Fail()));
    }

    [Test]
    public async Task RefreshTokenWithoutAuthToken()
    {
        var command = new RefreshToken("token");
        var user = new User
        {
            Login = "test",
            Profile = new UserProfile { Name = "test" },
        };
        var jwtToken = new JwtToken
        {
            TokenId = Guid.NewGuid(),
            IssuedAt = 0,
            ExpiresAt = 0,
            UserId = user.Id,
            Login = user.Login,
        };

        var logger = Substitute.For<ILogger<RefreshTokenHandler>>();
        var userRepository = Substitute.For<IUserRepository>();
        userRepository
            .GetUserById(jwtToken.UserId)
            .Returns(user);
        var tokenService = Substitute.For<ITokenService>();
        tokenService
            .DecodeTokenId(command.Token)
            .Returns(jwtToken);
        var timeProvider = Substitute.For<TimeProvider>();
        var handler = new RefreshTokenHandler(logger, userRepository, tokenService, timeProvider);

        var result = await handler.Handle(command);

        Assert.That(result, Is.EqualTo(RefreshTokenResult.Fail()));
    }

    [Test]
    public async Task RefreshTokenWithExpiredToken()
    {
        var currentDate = new DateTimeOffset(new DateTime(2024, 01, 01));
        var command = new RefreshToken("token");
        var user = new User
        {
            Login = "test",
            Profile = new UserProfile { Name = "test" },
        };
        var authToken = AuthenticationToken.Create(Guid.NewGuid(), user, currentDate.AddMonths(-1));
        user.AddAuthenticationToken(authToken);
        var jwtToken = new JwtToken
        {
            TokenId = authToken.TokenId,
            IssuedAt = 0,
            ExpiresAt = authToken.ExpiresAt.ToUnixTimeSeconds(),
            UserId = user.Id,
            Login = user.Login,
        };

        var logger = Substitute.For<ILogger<RefreshTokenHandler>>();
        var userRepository = Substitute.For<IUserRepository>();
        userRepository
            .GetUserById(jwtToken.UserId)
            .Returns(user);
        var tokenService = Substitute.For<ITokenService>();
        tokenService
            .DecodeTokenId(command.Token)
            .Returns(jwtToken);
        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider
            .GetUtcNow()
            .Returns(currentDate);
        var handler = new RefreshTokenHandler(logger, userRepository, tokenService, timeProvider);

        var result = await handler.Handle(command);

        Assert.That(result, Is.EqualTo(RefreshTokenResult.Fail()));
    }

    [Test]
    public async Task RefreshToken()
    {
        var currentDate = new DateTimeOffset(new DateTime(2024, 01, 01));
        var command = new RefreshToken("token");
        var user = new User
        {
            Login = "test",
            Profile = new UserProfile { Name = "test" },
            PasswordExpiresAt = currentDate.AddDays(90),
        };
        var authToken = AuthenticationToken.Create(Guid.NewGuid(), user, currentDate.AddMonths(1));
        user.AddAuthenticationToken(authToken);
        var jwtToken = new JwtToken
        {
            TokenId = authToken.TokenId,
            IssuedAt = 0,
            ExpiresAt = authToken.ExpiresAt.ToUnixTimeSeconds(),
            UserId = user.Id,
            Login = user.Login,
        };
        var newToken = new Token(Guid.NewGuid(), "new_token", DateTimeOffset.UtcNow);

        var logger = Substitute.For<ILogger<RefreshTokenHandler>>();
        var userRepository = Substitute.For<IUserRepository>();
        userRepository
            .GetUserById(jwtToken.UserId)
            .Returns(user);
        var tokenService = Substitute.For<ITokenService>();
        tokenService
            .DecodeTokenId(command.Token)
            .Returns(jwtToken);
        tokenService
            .GenerateAccessToken(user)
            .Returns(newToken);
        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider
            .GetUtcNow()
            .Returns(currentDate);
        var handler = new RefreshTokenHandler(logger, userRepository, tokenService, timeProvider);

        var result = await handler.Handle(command);

        Assert.That(result, Is.EqualTo(RefreshTokenResult.Success(newToken.Value)));
    }
}