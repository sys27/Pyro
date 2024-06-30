// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Pyro.Domain.Core.Models;

namespace Pyro.Domain.Identity.Models;

public class User : DomainEntity
{
    public static readonly Guid PyroUser = Guid.Parse("F9BA057A-35B0-4D10-8326-702D8F7EC966");

    private readonly List<Role> roles = [];
    private readonly List<AuthenticationToken> tokens = [];

    private string login;
    private IReadOnlyList<byte> password;
    private IReadOnlyList<byte> salt;

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

    public required string Login
    {
        get => login;
        [MemberNotNull(nameof(login))]
        init
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Login cannot be null or empty.", nameof(value));

            // TODO: Add validation
            login = value;
        }
    }

    public required IReadOnlyList<byte> Password
    {
        get => password;
        [MemberNotNull(nameof(password))]
        init
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            if (value.Count != 64)
                throw new ArgumentException("Password must be 64 bytes long.", nameof(value));

            password = value;
        }
    }

    public required IReadOnlyList<byte> Salt
    {
        get => salt;
        [MemberNotNull(nameof(salt))]
        init
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            if (value.Count != 16)
                throw new ArgumentException("Salt must be 16 bytes long.", nameof(value));

            salt = value;
        }
    }

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

    public void ClearRoles()
        => roles.Clear();

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