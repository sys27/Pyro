// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Domain.Identity.Models;

public record TokenPair(Token AccessToken, Token RefreshToken);

public record Token(Guid Id, string Value, DateTimeOffset ExpiresAt);