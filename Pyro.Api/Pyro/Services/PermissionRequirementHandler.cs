// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authorization;
using Pyro.Domain.Core;

namespace Pyro.Services;

internal class PermissionRequirementHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly ICurrentUserProvider currentUserProvider;

    public PermissionRequirementHandler(ICurrentUserProvider currentUserProvider)
    {
        this.currentUserProvider = currentUserProvider;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated is false)
        {
            context.Fail();
        }
        else
        {
            var currentUser = currentUserProvider.GetCurrentUser();
            if (!currentUser.HasPermission(requirement.Permission))
                context.Fail();
            else
                context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

internal class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }

    public string Permission { get; }
}