// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Pyro.Domain.Shared.Email;
using Pyro.Infrastructure.Shared.DataAccess;
using Pyro.Infrastructure.Shared.Email;

namespace Pyro.Infrastructure.Shared;

public static class ServiceCollectionExtensions
{
    public static IHostApplicationBuilder AddSharedInfrastructure(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services
            .AddTransient<DomainEventInterceptor>()
            .AddScoped<UnitOfWork>();

        services.Configure<EmailServiceOptions>(configuration.GetRequiredSection(EmailServiceOptions.Name));
        services.AddSingleton<IValidateOptions<EmailServiceOptions>, EmailServiceOptions>();
        services.AddTransient<ConsoleEmailService>();
        services.AddTransient<EmailService>();
        services.AddTransient<IEmailService>(sp =>
        {
            var emailServiceOptions = sp.GetRequiredService<IOptions<EmailServiceOptions>>();

            return emailServiceOptions.Value.Provider switch
            {
                EmailProviderKind.Console => sp.GetRequiredService<ConsoleEmailService>(),
                EmailProviderKind.Smtp => sp.GetRequiredService<EmailService>(),
                _ => throw new InvalidOperationException($"Invalid email provider: {emailServiceOptions.Value.Provider}"),
            };
        });

        return builder;
    }
}