// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Pyro.Domain.Shared.Entities;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Identity.Models;

public class User : DomainEntity
{
    public static readonly Guid PyroUser = Guid.Parse("F9BA057A-35B0-4D10-8326-702D8F7EC966");

    private readonly List<Role> roles = [];
    private readonly List<AuthenticationToken> authenticationTokens = [];
    private readonly List<AccessToken> accessTokens = [];
    private readonly List<OneTimePassword> oneTimePasswords = [];

    private byte[] password = [];
    private byte[] salt = [];
    private DateTimeOffset passwordExpiresAt;

    public static User Create(string login, string email, byte[] password, byte[] salt)
    {
        var user = new User
        {
            Login = login,
            Password = password,
            Salt = salt,
            PasswordExpiresAt = DateTimeOffset.MinValue,
            IsLocked = true,
            DisplayName = login,
            Email = email,
        };
        user.PublishEvent(new UserCreated(user.Id, login));

        return user;
    }

    public required string Login { get; init; }

    public IReadOnlyList<byte> Password
    {
        get => password;

        [MemberNotNull(nameof(password))]
        set
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
        set
        {
            if (value is null)
                throw new DomainValidationException("Salt cannot be null.");

            if (value.Count != 16)
                throw new DomainValidationException("Salt must be 16 bytes long.");

            salt = [..value];
        }
    }

    public DateTimeOffset PasswordExpiresAt
    {
        get => passwordExpiresAt;
        init => passwordExpiresAt = value;
    }

    public bool IsLocked { get; private set; }

    public IReadOnlyList<Role> Roles
        => roles;

    public IReadOnlyList<AuthenticationToken> AuthenticationTokens
        => authenticationTokens;

    public IReadOnlyList<AccessToken> AccessTokens
        => accessTokens;

    public IReadOnlyList<OneTimePassword> OneTimePasswords
        => oneTimePasswords;

    public required string DisplayName { get; set; }

    public required string Email { get; set; }

    public void Lock()
    {
        IsLocked = true;

        ClearAuthenticationTokens();
    }

    public void Unlock()
        => IsLocked = false;

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

    private void ChangePassword(TimeProvider timeProvider, IPasswordService passwordService, string newPassword)
    {
        var (newPasswordHash, newSalt) = passwordService.GeneratePasswordHash(newPassword);
        Password = newPasswordHash;
        Salt = newSalt;
        passwordExpiresAt = timeProvider.GetUtcNow().AddDays(90); // TODO: config

        foreach (var authenticationToken in authenticationTokens)
            authenticationToken.Invalidate(timeProvider);

        foreach (var accessToken in accessTokens)
            accessToken.Invalidate(timeProvider);

        foreach (var oneTimePassword in oneTimePasswords)
            oneTimePassword.Invalidate(timeProvider);
    }

    public void ChangePassword(
        TimeProvider timeProvider,
        IPasswordService passwordService,
        string oldPassword,
        string newPassword)
    {
        if (!passwordService.VerifyPassword(oldPassword, password, salt))
            throw new DomainException("The old password is incorrect.");

        ChangePassword(timeProvider, passwordService, newPassword);
    }

    public void AddOneTimePassword(OneTimePassword oneTimePassword)
    {
        if (oneTimePasswords.Any(x => x.Token == oneTimePassword.Token))
            return;

        oneTimePasswords.Add(oneTimePassword);
    }

    public OneTimePassword? GetOneTimePassword(string token)
        => oneTimePasswords.FirstOrDefault(x => x.Token == token);

    public OneTimePassword? GetRegistrationOneTimePassword()
        => oneTimePasswords.FirstOrDefault(x => x.IsUserRegistration);

    private void DeleteOneTimePassword(OneTimePassword oneTimePassword)
        => oneTimePasswords.Remove(oneTimePassword);

    public void Activate(
        TimeProvider timeProvider,
        IPasswordService passwordService,
        OneTimePassword oneTimePassword,
        string newPassword)
    {
        if (oneTimePassword is null)
            throw new ArgumentNullException(nameof(oneTimePassword));

        if (oneTimePassword.ExpiresAt < timeProvider.GetUtcNow())
            throw new DomainException($"The token '{oneTimePassword.Token}' is expired");

        if (!IsLocked)
            throw new DomainException($"The user '{Login}' is already activated.");

        ChangePassword(timeProvider, passwordService, newPassword);
        Unlock();
        DeleteOneTimePassword(oneTimePassword);
    }

    public void ResetPassword(
        TimeProvider timeProvider,
        IPasswordService passwordService,
        OneTimePassword oneTimePassword,
        string newPassword)
    {
        if (oneTimePassword is null)
            throw new ArgumentNullException(nameof(oneTimePassword));

        if (oneTimePassword.ExpiresAt < timeProvider.GetUtcNow())
            throw new DomainException($"The token '{oneTimePassword.Token}' is expired");

        if (IsLocked)
            throw new DomainException($"The user '{Login}' is already activated.");

        ChangePassword(timeProvider, passwordService, newPassword);
        DeleteOneTimePassword(oneTimePassword);
    }
}