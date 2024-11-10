// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.FileProviders;

namespace Pyro.Extensions;

internal static class SpaExtensions
{
    public static IServiceCollection AddSpa(this IServiceCollection services)
    {
        var fileServerOptions = new FileServerOptions
        {
            EnableDefaultFiles = true,
            EnableDirectoryBrowsing = false,
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
            DefaultFilesOptions =
            {
                DefaultFileNames = ["index.html"],
            },
        };
        services.AddSingleton(fileServerOptions);
        services.AddSingleton(fileServerOptions.DefaultFilesOptions);
        services.AddSingleton(fileServerOptions.StaticFileOptions);

        return services;
    }

    public static IApplicationBuilder UseSpa(this IApplicationBuilder app)
    {
        app.Use((context, next) =>
        {
            if (context.GetEndpoint() is not null)
                return next(context);

            var options = context.RequestServices.GetRequiredService<FileServerOptions>();
            var fileProvider = options.FileProvider ??
                               throw new InvalidOperationException("Missing FileProvider.");
            if (!fileProvider.GetFileInfo(context.Request.Path).Exists)
                context.Request.Path = "/index.html";

            return next(context);
        });
        app.UseDefaultFiles();
        app.UseStaticFiles();

        return app;
    }
}