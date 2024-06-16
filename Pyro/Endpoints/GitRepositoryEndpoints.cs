// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;
using Pyro.Domain.GitRepositories.Queries;
using Pyro.Dtos.Mapping;
using Pyro.Dtos.Requests;
using Pyro.Dtos.Responses;
using Pyro.Infrastructure.DataAccess;
using static Pyro.Domain.Identity.Models.Permission;

namespace Pyro.Endpoints;

/// <summary>
/// Contains mappings for repository endpoints.
/// </summary>
internal static class GitRepositoryEndpoints
{
    public static IEndpointRouteBuilder MapGitRepositoryEndpoints(this IEndpointRouteBuilder app)
    {
        var repositories = app.MapGroup("/repositories")
            .WithTags("Repositories");

        repositories.MapGet("/{name}", async (
                IMediator mediator,
                string name,
                CancellationToken cancellationToken) =>
            {
                var request = new GetGitRepository(name);
                var gitRepository = await mediator.Send(request, cancellationToken);
                var result = gitRepository?.ToResponse();

                return result is not null
                    ? Results.Ok(result)
                    : Results.NotFound();
            })
            .RequirePermission(RepositoryView)
            .Produces<GitRepositoryResponse>()
            .ProducesProblem(400)
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .ProducesProblem(500)
            .WithName("Get Repository")
            .WithOpenApi();

        repositories.MapGet("/", async (
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new GetGitRepositories();
                var gitRepository = await mediator.Send(command, cancellationToken);
                var result = DtoMapper.ToResponse(gitRepository);

                return Results.Ok(result);
            })
            .RequirePermission(RepositoryView)
            .Produces<IReadOnlyList<GitRepositoryResponse>>()
            .Produces(401)
            .Produces(403)
            .ProducesProblem(500)
            .WithName("Get Repositories")
            .WithOpenApi();

        repositories.MapPost("/", async (
                IMediator mediator,
                PyroDbContext dbContext,
                CreateGitRepositoryRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = DtoMapper.ToCommand(request);
                var gitRepository = await mediator.Send(command, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);

                var result = DtoMapper.ToResponse(gitRepository);

                return Results.Created($"/repositories/{command.Name}", result);
            })
            .RequirePermission(RepositoryEdit)
            .Produces<GitRepositoryResponse>(201)
            .ProducesProblem(400)
            .Produces(401)
            .Produces(403)
            .ProducesProblem(500)
            .WithName("Create Repository")
            .WithOpenApi();

        return app;
    }
}