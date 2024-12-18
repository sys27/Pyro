// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using Pyro.Extensions;
using Pyro.Services;

namespace Pyro.Endpoints;

internal static class GitBackendEndpoints
{
    public static IEndpointRouteBuilder MapGitBackendEndpoint(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/{repositoryName}.git")
            .RequireAuthorization(pb => pb
                .AddAuthenticationSchemes(AuthExtensions.BasicAuthenticationScheme)
                .RequireAuthenticatedUser());

        group.MapGet("/HEAD", async (
            [FromRoute] string repositoryName,
            [FromServices] NativeGitBackend gitBackend,
            CancellationToken cancellationToken) =>
        {
            await gitBackend.GetHead(repositoryName, cancellationToken);
        });
        group.MapGet("/info/refs", async (
            [FromRoute] string repositoryName,
            [FromServices] NativeGitBackend gitBackend,
            CancellationToken cancellationToken) =>
        {
            await gitBackend.GetInfoRefs(repositoryName, cancellationToken);
        });
        group.MapGet("/info/alternates", async (
            [FromRoute] string repositoryName,
            [FromServices] NativeGitBackend gitBackend,
            CancellationToken cancellationToken) =>
        {
            await gitBackend.GetInfoAlternates(repositoryName, cancellationToken);
        });
        group.MapGet("/info/http-alternates", async (
            [FromRoute] string repositoryName,
            [FromServices] NativeGitBackend gitBackend,
            CancellationToken cancellationToken) =>
        {
            await gitBackend.GetInfoHttpAlternates(repositoryName, cancellationToken);
        });
        group.MapGet("/info/packs", async (
            [FromRoute] string repositoryName,
            [FromServices] NativeGitBackend gitBackend,
            CancellationToken cancellationToken) =>
        {
            await gitBackend.GetInfoPacks(repositoryName, cancellationToken);
        });
        group.MapGet("/objects/{hash1:regex([[0-9a-f]]{{2}})}/{hash2:regex([[0-9a-f]]{{38}})}", async (
            [FromRoute] string repositoryName,
            [FromRoute] string hash1,
            [FromRoute] string hash2,
            [FromServices] NativeGitBackend gitBackend,
            CancellationToken cancellationToken) =>
        {
            await gitBackend.GetObjects(repositoryName, hash1, hash2, cancellationToken);
        });
        group.MapGet("/objects/{hash1:regex([[0-9a-f]]{{2}})}/{hash2:regex([[0-9a-f]]{{62}})}", async (
            [FromRoute] string repositoryName,
            [FromRoute] string hash1,
            [FromRoute] string hash2,
            [FromServices] NativeGitBackend gitBackend,
            CancellationToken cancellationToken) =>
        {
            await gitBackend.GetObjects(repositoryName, hash1, hash2, cancellationToken);
        });
        group.MapGet("/objects/pack/pack-{hash:regex([[0-9a-f]]{{40}})}.pack", async (
            [FromRoute] string repositoryName,
            [FromRoute] string hash,
            [FromServices] NativeGitBackend gitBackend,
            CancellationToken cancellationToken) =>
        {
            await gitBackend.GetObjectsPack(repositoryName, hash, cancellationToken);
        });
        group.MapGet("/objects/pack/pack-{hash:regex([[0-9a-f]]{{64}})}.pack", async (
            [FromRoute] string repositoryName,
            [FromRoute] string hash,
            [FromServices] NativeGitBackend gitBackend,
            CancellationToken cancellationToken) =>
        {
            await gitBackend.GetObjectsPack(repositoryName, hash, cancellationToken);
        });
        group.MapGet("/objects/pack/pack-{hash:regex([[0-9a-f]]{{40}})}.idx", async (
            [FromRoute] string repositoryName,
            [FromRoute] string hash,
            [FromServices] NativeGitBackend gitBackend,
            CancellationToken cancellationToken) =>
        {
            await gitBackend.GetObjectsPackIdx(repositoryName, hash, cancellationToken);
        });
        group.MapGet("/objects/pack/pack-{hash:regex([[0-9a-f]]{{64}})}.idx", async (
            [FromRoute] string repositoryName,
            [FromRoute] string hash,
            [FromServices] NativeGitBackend gitBackend,
            CancellationToken cancellationToken) =>
        {
            await gitBackend.GetObjectsPackIdx(repositoryName, hash, cancellationToken);
        });

        group.MapPost("/git-upload-pack", async (
            [FromRoute] string repositoryName,
            [FromServices] NativeGitBackend gitBackend,
            CancellationToken cancellationToken) =>
        {
            await gitBackend.GitUploadPack(repositoryName, cancellationToken);
        });
        group.MapPost("/git-upload-archive", async (
            [FromRoute] string repositoryName,
            [FromServices] NativeGitBackend gitBackend,
            CancellationToken cancellationToken) =>
        {
            await gitBackend.GitUploadArchive(repositoryName, cancellationToken);
        });
        group.MapPost("/git-receive-pack", async (
            [FromRoute] string repositoryName,
            [FromServices] NativeGitBackend gitBackend,
            CancellationToken cancellationToken) =>
        {
            await gitBackend.GitReceivePack(repositoryName, cancellationToken);
        });

        return app;
    }
}