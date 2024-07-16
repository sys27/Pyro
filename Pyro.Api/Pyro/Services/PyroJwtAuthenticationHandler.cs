// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Text.Encodings.Web;
using JWT;
using JWT.Extensions.AspNetCore;
using JWT.Extensions.AspNetCore.Factories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Pyro.Services;

internal class PyroJwtAuthenticationHandler : AuthenticationHandler<JwtAuthenticationOptions>
{
    private readonly IJwtDecoder jwtDecoder;
    private readonly IIdentityFactory identityFactory;
    private readonly ITicketFactory ticketFactory;

    public PyroJwtAuthenticationHandler(
        IOptionsMonitor<JwtAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IJwtDecoder jwtDecoder,
        IIdentityFactory identityFactory,
        ITicketFactory ticketFactory)
        : base(options, logger, encoder)
    {
        this.jwtDecoder = jwtDecoder;
        this.identityFactory = identityFactory;
        this.ticketFactory = ticketFactory;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        AuthenticateResult result;

        var header = Request.Headers.Authorization.ToString();
        if (!string.IsNullOrWhiteSpace(header))
        {
            result = GetAuthenticationResult(header);
        }
        else
        {
            var token = Request.Query["access_token"].ToString();
            result = DecodeToken(token);
        }

        return Task.FromResult(result);
    }

    private AuthenticateResult GetAuthenticationResult(string? header)
    {
        if (string.IsNullOrEmpty(header))
        {
            Logger.LogInformation("No Authorization header found, returning no result");
            return AuthenticateResult.NoResult();
        }

        if (!header.StartsWith(Scheme.Name, StringComparison.OrdinalIgnoreCase))
        {
            Logger.LogInformation("Incorrect scheme found, returning no result");
            return AuthenticateResult.NoResult();
        }

        var token = header[Scheme.Name.Length..].Trim();

        return DecodeToken(token);
    }

    private AuthenticateResult DecodeToken(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            Logger.LogInformation("Empty token found, returning no result");
            return AuthenticateResult.NoResult();
        }

        try
        {
            var payload = jwtDecoder.DecodeToObject(
                Options.PayloadType,
                token,
                Options.Keys,
                Options.VerifySignature);
            var identity = identityFactory.CreateIdentity(Options.PayloadType, payload);
            var ticket = ticketFactory.CreateTicket(identity, Scheme);

            return AuthenticateResult.Success(ticket);
        }
#pragma warning disable CA1031
        catch (Exception e)
#pragma warning restore CA1031
        {
            Logger.LogError(e, "Error decoding token");
            return AuthenticateResult.Fail(e);
        }
    }
}