// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using JWT;
using JWT.Algorithms;
using JWT.Extensions.AspNetCore;
using JWT.Extensions.AspNetCore.Factories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Pyro.Domain.Identity;
using Pyro.Domain.Shared.CurrentUserProvider;
using Pyro.Infrastructure.DataAccess;
using Pyro.Services;

namespace Pyro.Extensions;

internal static class AuthExtensions
{
    public const string BasicAuthenticationScheme = "Basic";

    public static IServiceCollection AddAuth(this IServiceCollection services)
    {
        services.AddDataProtection()
            .SetApplicationName("Pyro")
            .PersistKeysToDbContext<PyroDbContext>();

        services.AddSingleton(_ => new ValidationParameters
        {
            ValidateSignature = true,
            ValidateIssuedTime = true,
            ValidateExpirationTime = true,
            TimeMargin = 30,
        });
        services.AddJwtEncoder<HMACSHA512Algorithm>();
        services.AddJwtDecoder<GenericAlgorithmFactory<HMACSHA512Algorithm>>();
        services.AddSingleton<IIdentityFactory, PyroClaimsIdentityFactory>();

        services.ConfigureOptions<JwtAuthenticationConfigureOptions>();
        services.AddAuthentication(JwtAuthenticationDefaults.AuthenticationScheme)
            .AddJwt()
            .AddBasicAuthentication();
        services.AddAuthorization();

        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, PermissionRequirementHandler>();
        services.AddSingleton<ICurrentUserProvider, CurrentUserProvider>();

        return services;
    }

    private static AuthenticationBuilder AddBasicAuthentication(this AuthenticationBuilder builder)
        => builder.AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(BasicAuthenticationScheme, null);

    private static AuthenticationBuilder AddJwt(this AuthenticationBuilder builder)
    {
        builder.Services.AddJwtDecoder();
        builder.Services.TryAddSingleton<IDateTimeProvider, SystemClockDatetimeProvider>();

        builder.Services.TryAddSingleton<IIdentityFactory, ClaimsIdentityFactory>();
        builder.Services.TryAddSingleton<ITicketFactory, DefaultTicketFactory>();

        return builder.AddScheme<JwtAuthenticationOptions, PyroJwtAuthenticationHandler>(
            JwtAuthenticationDefaults.AuthenticationScheme,
            null);
    }

    private sealed class JwtAuthenticationConfigureOptions : IPostConfigureOptions<JwtAuthenticationOptions>
    {
        private readonly ISigningKeyService signingKeyService;

        public JwtAuthenticationConfigureOptions(ISigningKeyService signingKeyService)
        {
            this.signingKeyService = signingKeyService;
        }

        public void PostConfigure(string? name, JwtAuthenticationOptions options)
        {
            // TODO:
            var keys = signingKeyService.GetKeys().Result;

            options.VerifySignature = true;
            options.Keys = keys;
        }
    }

    private sealed class SystemClockDatetimeProvider : IDateTimeProvider
    {
#pragma warning disable CS0618 // Type or member is obsolete
        private readonly ISystemClock clock;

        public SystemClockDatetimeProvider(ISystemClock clock)
            => this.clock = clock;

        public DateTimeOffset GetNow()
            => clock.UtcNow;
#pragma warning restore CS0618 // Type or member is obsolete
    }
}