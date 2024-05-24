// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Security.Claims;
using JWT.Extensions.AspNetCore;
using JWT.Extensions.AspNetCore.Factories;
using Microsoft.Extensions.Options;

namespace Pyro.Services;

public class PyroClaimsIdentityFactory : ClaimsIdentityFactory
{
    public PyroClaimsIdentityFactory(IOptionsMonitor<JwtAuthenticationOptions> options)
        : base(options)
    {
    }

    protected override IEnumerable<Claim> ReadClaims(Type type, object payload)
    {
        if (payload is not IDictionary<string, object> dictionary)
            throw new InvalidOperationException("Payload is not a dictionary.");

        foreach (var (key, value) in dictionary)
        {
            if (value is IEnumerable<object> enumerable)
                foreach (var item in enumerable)
                    yield return new Claim(key, item.ToString() ?? string.Empty);
            else
                yield return new Claim(key, value.ToString() ?? string.Empty);
        }
    }
}