// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Pyro.Domain.Identity;
using Pyro.Extensions;

namespace Pyro.Services;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IPasswordService passwordService;
    private readonly IUserRepository userRepository;
    private readonly TimeProvider timeProvider;

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IPasswordService passwordService,
        IUserRepository userRepository,
        TimeProvider timeProvider)
        : base(options, logger, encoder)
    {
        this.passwordService = passwordService;
        this.userRepository = userRepository;
        this.timeProvider = timeProvider;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorization = Request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(authorization))
        {
            Response.Headers.WWWAuthenticate = "Basic realm=\"Pyro\"";

            Logger.LogInformation("Authorization header is missing");
            return AuthenticateResult.NoResult();
        }

        if (!authorization.StartsWith(AuthExtensions.BasicAuthenticationScheme, StringComparison.OrdinalIgnoreCase))
        {
            Logger.LogInformation("Authorization header is not Basic");
            return AuthenticateResult.NoResult();
        }

        var encodedCredentials = authorization["Basic ".Length..].Trim();
        var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
        var parts = decodedCredentials.Split(':', 2);
        if (parts.Length != 2)
        {
            Logger.LogInformation("Invalid credentials format");
            return AuthenticateResult.Fail("Invalid credentials format");
        }

        var username = parts[0];
        var password = parts[1];
        var user = await userRepository.GetUserByLogin(username, Context.RequestAborted);
        if (user is null || !user.ValidateAccessToken(passwordService, timeProvider, password))
        {
            Logger.LogInformation("Invalid username or password");
            return AuthenticateResult.Fail("Invalid username or password");
        }

        if (user.IsLocked)
        {
            Logger.LogInformation("User is locked");
            return AuthenticateResult.Fail("User is locked");
        }

        if (user.PasswordExpiresAt < timeProvider.GetUtcNow())
        {
            Logger.LogWarning("The password of '{User}' user is expired", user.Login);

            return AuthenticateResult.Fail("Password is expired");
        }

        // TODO: implement shared logic with TokenService?
        var roles = user.Roles.Select(x => x.Name);
        var permissions = user.Roles.SelectMany(x => x.Permissions).Select(x => x.Name).Distinct();
        var claims = new List<Claim>
        {
            new Claim("sub", user.Id.ToString()),
            new Claim("login", user.Login),
        };
        claims.AddRange(roles.Select(role => new Claim("roles", role)));
        claims.AddRange(permissions.Select(permission => new Claim("permissions", permission)));

        var identity = new ClaimsIdentity(claims, Scheme.Name, "login", "roles");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}