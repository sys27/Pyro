// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Security.Cryptography;
using System.Text;

namespace Pyro.Domain.Identity;

public class PasswordService
{
    private const int HashSize = 64;
    private const int SaltSize = 16;
    private const int IterationsCount = 200000;

    private static byte[] GetPasswordHash(string password, byte[] salt)
    {
        if (salt.Length != SaltSize)
            throw new ArgumentException($"Salt size must be {SaltSize} bytes.", nameof(salt));

        var passwordBytes = Encoding.UTF8.GetBytes(password);
        using var pbkdf2 = new Rfc2898DeriveBytes(passwordBytes, salt, IterationsCount, HashAlgorithmName.SHA512);
        var passwordHash = pbkdf2.GetBytes(HashSize);

        return passwordHash;
    }

    public (byte[] Password, byte[] Salt) GeneratePasswordHash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var passwordHash = GetPasswordHash(password, salt);

        return (passwordHash, salt);
    }

    public bool VerifyPassword(string password, byte[] passwordHash, byte[] salt)
    {
        var passwordHashToVerify = GetPasswordHash(password, salt);

        return passwordHashToVerify.AsSpan().SequenceEqual(passwordHash);
    }
}