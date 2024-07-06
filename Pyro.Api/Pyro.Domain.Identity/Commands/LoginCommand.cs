// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Pyro.Domain.Identity.Models;

namespace Pyro.Domain.Identity.Commands;

public record LoginCommand(string Login, string Password) : IRequest<LoginResult>;

public readonly struct LoginResult : IEquatable<LoginResult>
{
    private LoginResult(bool isSuccess, JwtTokenPair? tokenPair)
    {
        IsSuccess = isSuccess;
        TokenPair = tokenPair;
    }

    public static LoginResult Success(JwtTokenPair jwtTokenPair)
        => new LoginResult(true, jwtTokenPair);

    public static LoginResult Fail()
        => new LoginResult(false, null);

    public static bool operator ==(LoginResult left, LoginResult right)
        => left.Equals(right);

    public static bool operator !=(LoginResult left, LoginResult right)
        => !left.Equals(right);

    public bool Equals(LoginResult other)
        => IsSuccess == other.IsSuccess && Equals(TokenPair, other.TokenPair);

    public override bool Equals(object? obj)
        => obj is LoginResult other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(IsSuccess, TokenPair);

    [MemberNotNullWhen(true, nameof(TokenPair))]
    public bool IsSuccess { get; }

    public JwtTokenPair? TokenPair { get; }
}

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty()
            .MaximumLength(32);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(50);
    }
}

public class LoginHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly ILogger<LoginHandler> logger;
    private readonly IUserRepository repository;
    private readonly IPasswordService passwordService;
    private readonly TokenService tokenService;

    public LoginHandler(
        ILogger<LoginHandler> logger,
        IUserRepository repository,
        IPasswordService passwordService,
        TokenService tokenService)
    {
        this.logger = logger;
        this.repository = repository;
        this.passwordService = passwordService;
        this.tokenService = tokenService;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await repository.GetUserByLogin(request.Login, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("Login attempt. User with login '{Login}' not found", request.Login);

            return LoginResult.Fail();
        }

        if (user.IsLocked)
        {
            logger.LogWarning("Login attempt. User with login '{Login}' is locked", request.Login);

            return LoginResult.Fail();
        }

        if (!passwordService.VerifyPassword(request.Password, user.Password, user.Salt))
        {
            logger.LogWarning("Login attempt. User with login '{Login}' provided invalid password", request.Login);

            return LoginResult.Fail();
        }

        var tokenPair = await tokenService.GenerateTokenPair(user);
        var token = new AuthenticationToken
        {
            TokenId = tokenPair.RefreshToken.Id,
            ExpiresAt = tokenPair.RefreshToken.ExpiresAt,
            UserId = user.Id,
            User = user,
        };
        user.AddToken(token);

        return LoginResult.Success(tokenPair);
    }
}