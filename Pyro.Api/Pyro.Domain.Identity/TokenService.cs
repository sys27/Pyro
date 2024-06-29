// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using JWT;
using Pyro.Domain.Identity.Models;

namespace Pyro.Domain.Identity;

public class TokenService
{
    private static readonly Dictionary<string, object> headers = [];

    private readonly TimeProvider timeProvider;
    private readonly IJwtEncoder jwtEncoder;
    private readonly IJwtDecoder jwtDecoder;
    private readonly ISigningKeyService signingKeyService;

    public TokenService(
        TimeProvider timeProvider,
        IJwtEncoder jwtEncoder,
        IJwtDecoder jwtDecoder,
        ISigningKeyService signingKeyService)
    {
        this.timeProvider = timeProvider;
        this.jwtEncoder = jwtEncoder;
        this.jwtDecoder = jwtDecoder;
        this.signingKeyService = signingKeyService;
    }

    public async Task<TokenPair> GenerateTokenPair(User user)
    {
        var accessToken = await GenerateAccessToken(user);
        var refreshToken = await GenerateRefreshToken(user);

        return new TokenPair(accessToken, refreshToken);
    }

    public async Task<Token> GenerateAccessToken(User user)
    {
        var tokenId = Guid.NewGuid();
        var currentDate = timeProvider.GetUtcNow();
        var accessTokenExpiration = currentDate.AddMinutes(5);
        var roles = user.Roles.Select(x => x.Name);
        var permissions = user.Roles.SelectMany(x => x.Permissions).Select(x => x.Name).Distinct();
        var jwtToken = new JwtToken
        {
            TokenId = tokenId,
            IssuedAt = currentDate.ToUnixTimeSeconds(),
            ExpiresAt = accessTokenExpiration.ToUnixTimeSeconds(),
            UserId = user.Id,
            Login = user.Login,
            Roles = roles,
            Permissions = permissions,
        };

        var keys = await signingKeyService.GetKeys();
        var token = jwtEncoder.Encode(headers, jwtToken, keys.First());

        return new Token(tokenId, token, accessTokenExpiration);
    }

    private async Task<Token> GenerateRefreshToken(User user)
    {
        var tokenId = Guid.NewGuid();
        var currentDate = timeProvider.GetUtcNow();
        var refreshTokenExpiration = currentDate.AddMonths(6);
        var jwtToken = new JwtToken
        {
            TokenId = tokenId,
            IssuedAt = currentDate.ToUnixTimeSeconds(),
            ExpiresAt = refreshTokenExpiration.ToUnixTimeSeconds(),
            UserId = user.Id,
            Login = user.Login,
        };

        var keys = await signingKeyService.GetKeys();
        var refreshToken = jwtEncoder.Encode(headers, jwtToken, keys.First());

        return new Token(tokenId, refreshToken, refreshTokenExpiration);
    }

    public async Task<JwtToken> DecodeTokenId(string token)
    {
        var keys = await signingKeyService.GetKeys();
        var jwtToken = jwtDecoder.DecodeToObject<JwtToken>(token, keys.First());

        return jwtToken;
    }
}