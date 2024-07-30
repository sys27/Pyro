// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pyro.Domain.Identity;
using Pyro.Infrastructure.Identity.DataAccess;
using Pyro.Infrastructure.Shared.DataAccess;

namespace Pyro.Infrastructure.Identity;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<IdentityDbContext>((provider, options) => options
            .UseSqlite(configuration.GetConnectionString("DefaultConnection"))
            .AddInterceptors(provider.GetRequiredService<DomainEventInterceptor>()));
        services.AddScoped<DbContext, IdentityDbContext>(sp => sp.GetRequiredService<IdentityDbContext>());

        return services
            .AddScoped<IUserRepository, UserRepository>()
            .AddSingleton<ISigningKeyService, SigningKeyService>();
    }
}