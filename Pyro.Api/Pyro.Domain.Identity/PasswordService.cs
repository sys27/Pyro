// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Security.Cryptography;
using System.Text;
using Pyro.Domain.Identity.Models;

namespace Pyro.Domain.Identity;

public class PasswordService : IPasswordService
{
    private const int HashSize = 64;
    private const int SaltSize = 16;
    private const int IterationsCount = 200000;
    private const int PasswordLength = 64;

    private readonly TimeProvider timeProvider;

    public PasswordService(TimeProvider timeProvider)
    {
        this.timeProvider = timeProvider;
    }

    private static byte[] GetPasswordHash(string password, byte[] salt)
    {
        if (salt.Length != SaltSize)
            throw new ArgumentException($"Salt size must be {SaltSize} bytes.", nameof(salt));

        var passwordBytes = Encoding.UTF8.GetBytes(password);
        using var pbkdf2 = new Rfc2898DeriveBytes(passwordBytes, salt, IterationsCount, HashAlgorithmName.SHA512);
        var passwordHash = pbkdf2.GetBytes(HashSize);

        return passwordHash;
    }

    public (byte[] PasswordHash, byte[] Salt) GeneratePasswordHash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var passwordHash = GetPasswordHash(password, salt);

        return (passwordHash, salt);
    }

    public string GeneratePassword()
    {
        var bytes = RandomNumberGenerator.GetBytes(PasswordLength);
        var base64 = Convert.ToBase64String(bytes);

        return base64;
    }

    public bool VerifyPassword(string password, IReadOnlyList<byte> passwordHash, IReadOnlyList<byte> salt)
    {
        var passwordHashToVerify = GetPasswordHash(password, [..salt]);

        return passwordHashToVerify.AsSpan().SequenceEqual([..passwordHash]);
    }

    private static string GetToken()
    {
        using var rng = RandomNumberGenerator.Create();

        Span<byte> bytes = stackalloc byte[16];
        rng.GetBytes(bytes);

        return Convert.ToBase64String(bytes);
    }

    public OneTimePassword GenerateOneTimePasswordFor(User user)
    {
        var token = GetToken();
        var expiresAt = timeProvider.GetUtcNow().AddDays(1); // TODO: config
        var otp = new OneTimePassword
        {
            Token = token,
            ExpiresAt = expiresAt,
            Purpose = OneTimePasswordPurpose.UserRegistration,
            User = user,
        };
        user.AddOneTimePassword(otp);

        return otp;
    }

    public OneTimePassword GeneratePasswordResetTokenFor(User user)
    {
        var token = GetToken();
        var expiresAt = timeProvider.GetUtcNow().AddMinutes(5); // TODO: config
        var otp = new OneTimePassword
        {
            Token = token,
            ExpiresAt = expiresAt,
            Purpose = OneTimePasswordPurpose.PasswordReset,
            User = user,
        };
        user.AddOneTimePassword(otp);

        return otp;
    }
}