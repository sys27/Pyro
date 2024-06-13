// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using JWT.Extensions.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Pyro.Services;

internal class PermissionPolicyProvider : DefaultAuthorizationPolicyProvider
{
    public const string PolicyPrefix = "Permission:";

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        : base(options)
    {
    }

    public override Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (!policyName.StartsWith(PolicyPrefix, StringComparison.InvariantCulture))
            return base.GetPolicyAsync(policyName);

        var permission = policyName[PolicyPrefix.Length..];
        var builder = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(JwtAuthenticationDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .AddRequirements(new PermissionRequirement(permission));
        var policy = builder.Build();

        return Task.FromResult<AuthorizationPolicy?>(policy);
    }
}