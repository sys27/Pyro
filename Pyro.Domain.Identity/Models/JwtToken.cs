// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;

namespace Pyro.Domain.Identity.Models;

// TODO: on token creation
public class JwtToken
{
    [JsonPropertyName("jti")]
    public required Guid TokenId { get; init; }

    [JsonPropertyName("iat")]
    public required long IssuedAt { get; init; }

    [JsonPropertyName("exp")]
    public required long ExpiresAt { get; init; }

    [JsonPropertyName("sub")]
    public required Guid UserId { get; init; }

    [JsonPropertyName("login")]
    public required string Login { get; init; }
}