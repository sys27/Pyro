// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Domain.Identity.Models;

public class User
{
    private readonly List<Role> roles = [];
    private readonly List<AuthenticationToken> tokens = [];

    public Guid Id { get; init; } = Guid.NewGuid();

    public required string Email { get; init; }

    public required byte[] Password { get; init; }

    public required byte[] Salt { get; init; }

    public bool IsLocked { get; private set; }

    public IReadOnlyCollection<Role> Roles
        => roles;

    public IReadOnlyCollection<AuthenticationToken> Tokens
        => tokens;

    public void Lock()
        => IsLocked = true;

    public void AddRole(Role role)
    {
        if (roles.Any(x => x.Name == role.Name))
            return;

        roles.Add(role);
    }

    public void AddToken(AuthenticationToken token)
    {
        if (tokens.Any(x => x.TokenId == token.TokenId))
            return;

        tokens.Add(token);
    }

    public AuthenticationToken? GetToken(Guid tokenId)
        => tokens.FirstOrDefault(x => x.TokenId == tokenId);

    public void ClearTokens()
        => tokens.Clear();
}