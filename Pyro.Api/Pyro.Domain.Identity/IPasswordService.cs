// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Domain.Identity;

public interface IPasswordService
{
    (byte[] PasswordHash, byte[] Salt) GeneratePasswordHash(string password);

    bool VerifyPassword(string password, IReadOnlyList<byte> passwordHash, IReadOnlyList<byte> salt);

    string GeneratePassword();
}