// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Pyro.Domain.Identity.Commands;

public record RefreshToken(string Token) : IRequest<RefreshTokenResult>;

public record RefreshTokenResult(bool IsSuccess, string AccessToken)
{
    public static RefreshTokenResult Success(string refreshToken)
        => new RefreshTokenResult(true, refreshToken);

    public static RefreshTokenResult Fail()
        => new RefreshTokenResult(false, string.Empty);
}

public class RefreshTokenValidator : AbstractValidator<RefreshToken>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty();
    }
}

public class RefreshTokenHandler : IRequestHandler<RefreshToken, RefreshTokenResult>
{
    private readonly ILogger<RefreshTokenHandler> logger;
    private readonly IUserRepository userRepository;
    private readonly TokenService tokenService;

    public RefreshTokenHandler(
        ILogger<RefreshTokenHandler> logger,
        IUserRepository userRepository,
        TokenService tokenService)
    {
        this.logger = logger;
        this.userRepository = userRepository;
        this.tokenService = tokenService;
    }

    public async Task<RefreshTokenResult> Handle(RefreshToken request, CancellationToken cancellationToken)
    {
        // TODO: check expiration date and signature
        var jwtToken = tokenService.DecodeTokenId(request.Token);
        var user = await userRepository.GetUserById(jwtToken.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("The user (Id: {Id}) was not found", jwtToken.UserId);

            return RefreshTokenResult.Fail();
        }

        var token = user.GetToken(jwtToken.TokenId);
        if (token is null)
        {
            logger.LogWarning("The token (Id: {TokenId}) was not found", jwtToken.TokenId);

            return RefreshTokenResult.Fail();
        }

        var newToken = tokenService.GenerateAccessToken(user);

        return RefreshTokenResult.Success(newToken.Value);
    }
}