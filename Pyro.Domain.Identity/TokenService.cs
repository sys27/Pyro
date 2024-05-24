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

    public TokenService(TimeProvider timeProvider, IJwtEncoder jwtEncoder, IJwtDecoder jwtDecoder)
    {
        this.timeProvider = timeProvider;
        this.jwtEncoder = jwtEncoder;
        this.jwtDecoder = jwtDecoder;
    }

    public TokenPair GenerateTokenPair(User user)
    {
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken(user);

        return new TokenPair(accessToken, refreshToken);
    }

    public Token GenerateAccessToken(User user)
    {
        var tokenId = Guid.NewGuid();
        var currentDate = timeProvider.GetUtcNow();
        var accessTokenExpiration = currentDate.AddMinutes(5);
        var roles = user.Roles.Select(x => x.Name).ToArray();
        var permissions = user.Roles.SelectMany(x => x.Permissions).Select(x => x.Name).Distinct().ToArray();
        var claims = new Dictionary<string, object>
        {
            { "jti", tokenId.ToString() },
            { "iat", currentDate.ToUnixTimeSeconds() },
            { "exp", accessTokenExpiration.ToUnixTimeSeconds() },
            { "sub", user.Id.ToString() },
            { "email", user.Email },
            { "roles", roles },
            { "permissions", permissions },
        };

        // TODO:
        var token = jwtEncoder.Encode(headers, claims, "secret");

        return new Token(tokenId, token, accessTokenExpiration);
    }

    private Token GenerateRefreshToken(User user)
    {
        var tokenId = Guid.NewGuid();
        var currentDate = timeProvider.GetUtcNow();
        var refreshTokenExpiration = currentDate.AddMonths(6);
        var claims = new Dictionary<string, object>
        {
            { "jti", tokenId.ToString() },
            { "iat", currentDate.ToUnixTimeSeconds() },
            { "exp", refreshTokenExpiration.ToUnixTimeSeconds() },
            { "sub", user.Id.ToString() },
            { "email", user.Email },
        };

        // TODO:
        var refreshToken = jwtEncoder.Encode(headers, claims, "secret");

        return new Token(tokenId, refreshToken, refreshTokenExpiration);
    }

    public JwtToken DecodeTokenId(string token)
        => jwtDecoder.DecodeToObject<JwtToken>(token, "secret");
}