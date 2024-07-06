// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Options;
using Pyro.Infrastructure;
using Pyro.Services;

namespace Pyro.Endpoints;

internal static class GitBackendEndpoints
{
    public static IEndpointRouteBuilder MapGitBackendEndpoints(this IEndpointRouteBuilder app)
    {
        var options = app.ServiceProvider.GetRequiredService<IOptions<GitOptions>>();
        if (options.Value.UseNativeGitBackend)
            return app.MapNativeGitBackendEndpoints();

        return app.MapCsharpBackendEndpoint();
    }

    private static IEndpointRouteBuilder MapNativeGitBackendEndpoints(this IEndpointRouteBuilder app)
    {
        app.Map("/{name}.git/{**path}", async (
                GitBackend backend,
                string name,
                CancellationToken cancellationToken) =>
            await backend.Handle(name, cancellationToken));

        return app;
    }

    private static IEndpointRouteBuilder MapCsharpBackendEndpoint(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/{name}.git");

        group.MapGet("/HEAD", () => { throw new NotImplementedException(); });
        group.MapGet("/info/refs", () => { throw new NotImplementedException(); });
        group.MapGet("/info/alternates", () => { throw new NotImplementedException(); });
        group.MapGet("/info/http-alternates", () => { throw new NotImplementedException(); });
        group.MapGet("/info/packs", () => { throw new NotImplementedException(); });
        group.MapGet("/objects/{hash1:regex([[0-9a-f]]{{2}})}/{hash2:regex([[0-9a-f]]{{38}})}", () => { throw new NotImplementedException(); });
        group.MapGet("/objects/{hash1:regex([[0-9a-f]]{{2}})}/{hash2:regex([[0-9a-f]]{{62}})}", () => { throw new NotImplementedException(); });
        group.MapGet("/objects/pack/pack-{hash:regex([[0-9a-f]]{{40}})}.pack", () => { throw new NotImplementedException(); });
        group.MapGet("/objects/pack/pack-{hash:regex([[0-9a-f]]{{64}})}.pack", () => { throw new NotImplementedException(); });
        group.MapGet("/objects/pack/pack-{hash:regex([[0-9a-f]]{{40}})}.idx", () => { throw new NotImplementedException(); });
        group.MapGet("/objects/pack/pack-{hash:regex([[0-9a-f]]{{64}})}.idx", () => { throw new NotImplementedException(); });

        group.MapPost("/git-upload-pack", () => { throw new NotImplementedException(); });
        group.MapPost("/git-upload-archive", () => { throw new NotImplementedException(); });
        group.MapPost("/git-receive-pack", () => { throw new NotImplementedException(); });

        return app;
    }
}