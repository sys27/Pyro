// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pyro.Domain.Identity;
using Pyro.Domain.Identity.Models;
using Pyro.Infrastructure.Identity.DataAccess;

namespace Pyro.Infrastructure.Identity;

public class SigningKeyService : ISigningKeyService
{
    private readonly IServiceProvider serviceProvider;
    private readonly TimeProvider timeProvider;
    private readonly IDataProtector protector;

    private readonly Lazy<Task<string[]>> keys;

    public SigningKeyService(
        IServiceProvider serviceProvider,
        IDataProtectionProvider dataProtectionProvider,
        TimeProvider timeProvider)
    {
        this.serviceProvider = serviceProvider;
        this.timeProvider = timeProvider;
        this.protector = dataProtectionProvider.CreateProtector("SigningKey");

        keys = new Lazy<Task<string[]>>(GetOrCreateKeys, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    private async Task<string[]> GetOrCreateKeys()
    {
        // TODO: rotate keys
        // TODO: remove old keys
        using var scope = serviceProvider.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

        var signingKeys = await dbContext
            .Set<SigningKey>()
            .ToListAsync();

        if (signingKeys.Count == 0)
        {
            var key = protector.Protect(Guid.NewGuid().ToString());
            var signingKey = new SigningKey
            {
                Key = key,
                CreatedAt = timeProvider.GetUtcNow(),
            };
            await dbContext.AddAsync(signingKey);
            await dbContext.SaveChangesAsync();

            return [key];
        }

        var keys = signingKeys
            .Select(x => protector.Unprotect(x.Key))
            .ToArray();

        return keys;
    }

    public Task<string[]> GetKeys()
        => keys.Value;
}