// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;

namespace Pyro.Domain.Identity;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityDomain(this IServiceCollection services)
        => services
            .AddSingleton<PasswordService>()
            .AddSingleton<TokenService>();
}