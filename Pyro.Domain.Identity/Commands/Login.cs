// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Pyro.Domain.Identity.Models;

namespace Pyro.Domain.Identity.Commands;

public record Login(string Email, string Password) : IRequest<LoginResult>;

public readonly struct LoginResult : IEquatable<LoginResult>
{
    private LoginResult(bool isSuccess, TokenPair? tokenPair)
    {
        IsSuccess = isSuccess;
        TokenPair = tokenPair;
    }

    public static LoginResult Success(TokenPair tokenPair)
        => new LoginResult(true, tokenPair);

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

    public TokenPair? TokenPair { get; }
}

public class LoginValidator : AbstractValidator<Login>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(50)
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(50);
    }
}

public class LoginHandler : IRequestHandler<Login, LoginResult>
{
    private readonly ILogger<LoginHandler> logger;
    private readonly IUserRepository repository;
    private readonly PasswordService passwordService;
    private readonly TokenService tokenService;

    public LoginHandler(
        ILogger<LoginHandler> logger,
        IUserRepository repository,
        PasswordService passwordService,
        TokenService tokenService)
    {
        this.logger = logger;
        this.repository = repository;
        this.passwordService = passwordService;
        this.tokenService = tokenService;
    }

    public async Task<LoginResult> Handle(Login request, CancellationToken cancellationToken)
    {
        var user = await repository.GetUserByEmail(request.Email, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("Login attempt. User with email '{Email}' not found", request.Email);

            return LoginResult.Fail();
        }

        if (user.IsLocked)
        {
            logger.LogWarning("Login attempt. User with email '{Email}' is locked", request.Email);

            return LoginResult.Fail();
        }

        if (!passwordService.VerifyPassword(request.Password, user.Password, user.Salt))
        {
            logger.LogWarning("Login attempt. User with email '{Email}' provided invalid password", request.Email);

            return LoginResult.Fail();
        }

        var tokenPair = tokenService.GenerateTokenPair(user);
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