// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pyro.Domain;
using Pyro.Domain.Core;
using Pyro.Domain.GitRepositories;
using Pyro.Domain.Identity;
using Pyro.Domain.UserProfiles;
using Pyro.Infrastructure.DataAccess;
using Pyro.Infrastructure.Messaging;

namespace Pyro.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<DomainEventInterceptor>();
        services.AddDbContext<PyroDbContext>((provider, options) => options
            .UseSqlite(configuration.GetConnectionString("DefaultConnection"))
            .AddInterceptors(provider.GetRequiredService<DomainEventInterceptor>()));

        return services
            .AddScoped<IBus, Bus>()
            .AddScoped<IGitService, GitService>()
            .AddScoped<IGitRepositoryRepository, GitRepositoryRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IUserProfileRepository, UserProfileRepository>()
            .AddSingleton<ISigningKeyService, SigningKeyService>();
    }
}