// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Security.Claims;
using Pyro.Domain.Shared;
using Pyro.Domain.Shared.Models;

namespace Pyro.Services;

public class CurrentUserProvider : ICurrentUserProvider
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public CurrentUserProvider(IHttpContextAccessor httpContextAccessor)
        => this.httpContextAccessor = httpContextAccessor;

    public CurrentUser GetCurrentUser()
    {
        var httpContext = httpContextAccessor.HttpContext ??
                          throw new InvalidOperationException("HttpContext is null. You are probably using the CurrentUserProvider outside of an HTTP request.");

        var user = httpContext.User;
        var userId = Guid.Parse(GetClaim(user, "sub"));
        var login = GetClaim(user, "login");
        var roles = GetClaims(user, "roles").ToList();
        var permissions = GetClaims(user, "permissions").ToList();

        var currentUser = new CurrentUser(userId, login, roles, permissions);

        return currentUser;
    }

    private string GetClaim(ClaimsPrincipal user, string name)
    {
        var claim = user.FindFirst(name);
        if (claim is null)
            throw new InvalidOperationException($"The '{name}' claim was not found.");

        return claim.Value;
    }

    private IEnumerable<string> GetClaims(ClaimsPrincipal user, string name)
    {
        var claims = user.FindAll(name) ??
                     throw new InvalidOperationException($"The '{name}' claim was not found.");

        return claims.Select(c => c.Value);
    }
}