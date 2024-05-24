// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;
using Pyro.Domain.GitRepositories;
using Pyro.Dtos;
using Pyro.Dtos.Requests;
using Pyro.Dtos.Responses;
using Pyro.Infrastructure.DataAccess;

namespace Pyro.Endpoints;

/// <summary>
/// Contains mappings for repository endpoints.
/// </summary>
public static class GitRepositoryEndpoints
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
            .Produces<GitRepositoryResponse>()
            .Produces(404)
            .WithName("Get Repository")
            .WithOpenApi();

        repositories.MapPost("/", async (
                IMediator mediator,
                PyroDbContext dbContext,
                CreateGitRepositoryRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = request.ToCommand();
                var gitRepository = await mediator.Send(command, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);

                var result = gitRepository.ToResponse();

                return Results.Created($"/repositories/{command.Name}", result);
            })
            .Produces<GitRepositoryResponse>(201)
            .Produces(404)
            .WithName("Create Repository")
            .WithOpenApi();

        return app;
    }
}