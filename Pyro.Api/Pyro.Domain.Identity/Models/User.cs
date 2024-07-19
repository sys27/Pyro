// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Pyro.Domain.Shared.Exceptions;
using Pyro.Domain.Shared.Models;

namespace Pyro.Domain.Identity.Models;

public class User : DomainEntity
{
    public static readonly Guid PyroUser = Guid.Parse("F9BA057A-35B0-4D10-8326-702D8F7EC966");

    private readonly List<Role> roles = [];
    private readonly List<AuthenticationToken> authenticationTokens = [];
    private readonly List<AccessToken> accessTokens = [];

    private byte[] password = [];
    private byte[] salt = [];

    public static User Create(string login, byte[] password, byte[] salt)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Login = login,
            Password = password,
            Salt = salt,
        };
        user.PublishEvent(new UserCreated(user.Id));

        return user;
    }

    public Guid Id { get; init; }

    public required string Login { get; init; }

    public IReadOnlyList<byte> Password
    {
        get => password;

        [MemberNotNull(nameof(password))]
        init
        {
            if (value is null)
                throw new DomainValidationException("Password cannot be null.");

            if (value.Count != 64)
                throw new DomainValidationException("Password must be 64 bytes long.");

            password = [..value];
        }
    }

    public IReadOnlyList<byte> Salt
    {
        get => salt;

        [MemberNotNull(nameof(salt))]
        init
        {
            if (value is null)
                throw new DomainValidationException("Salt cannot be null.");

            if (value.Count != 16)
                throw new DomainValidationException("Salt must be 16 bytes long.");

            salt = [..value];
        }
    }

    public bool IsLocked { get; private set; }

    public IReadOnlyList<Role> Roles
        => roles;

    public IReadOnlyList<AuthenticationToken> AuthenticationTokens
        => authenticationTokens;

    public IReadOnlyList<AccessToken> AccessTokens
        => accessTokens;

    public void Lock()
        => IsLocked = true;

    public void AddRole(Role role)
    {
        if (roles.Any(x => x.Name == role.Name))
            return;

        roles.Add(role);
    }

    public void ClearRoles()
        => roles.Clear();

    public void AddAuthenticationToken(AuthenticationToken token)
    {
        if (authenticationTokens.Any(x => x.TokenId == token.TokenId))
            return;

        authenticationTokens.Add(token);
    }

    public AuthenticationToken? GetAuthenticationToken(Guid tokenId)
        => authenticationTokens.FirstOrDefault(x => x.TokenId == tokenId);

    public void ClearAuthenticationTokens()
        => authenticationTokens.Clear();

    public void AddAccessToken(AccessToken token)
    {
        if (accessTokens.Any(x => x.Name == token.Name))
            return;

        accessTokens.Add(token);
    }

    public bool ValidateAccessToken(IPasswordService passwordService, TimeProvider timeProvider, string token)
    {
        var now = timeProvider.GetUtcNow();

        return accessTokens.Any(accessToken =>
            accessToken.ExpiresAt > now &&
            passwordService.VerifyPassword(token, accessToken.Token, accessToken.Salt));
    }

    public void DeleteAccessToken(string name)
    {
        var accessToken = accessTokens.FirstOrDefault(x => x.Name == name) ??
                          throw new NotFoundException("Access token not found");

        accessTokens.Remove(accessToken);
    }
}