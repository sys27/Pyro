// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Identity.Models;

namespace Pyro.Domain.Identity;

public interface ITokenService
{
    Task<JwtTokenPair> GenerateTokenPair(User user);
    Task<Token> GenerateAccessToken(User user);
    Task<JwtToken> DecodeTokenId(string token);
}