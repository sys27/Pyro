// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Pyro.Contracts.Requests;
using Pyro.Contracts.Responses;
using Pyro.Domain.GitRepositories.Queries;
using Pyro.DtoMappings;
using Pyro.Infrastructure.Shared.DataAccess;
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
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .ProducesProblem(500)
            .WithName("Get Repository")
            .WithOpenApi();

        repositories.MapGet("/", async (
                IMediator mediator,
                [AsParameters] PageRequest<string> request,
                CancellationToken cancellationToken) =>
            {
                var command = request.ToGetGitRepositories();
                var gitRepository = await mediator.Send(command, cancellationToken);
                var result = gitRepository.ToResponse();

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
                UnitOfWork unitOfWork,
                CreateGitRepositoryRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = request.ToCommand();
                var gitRepository = await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                var result = gitRepository.ToResponse();

                return Results.Created($"/repositories/{command.Name}", result);
            })
            .RequirePermission(RepositoryEdit)
            .Produces<GitRepositoryResponse>(201)
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .ProducesProblem(500)
            .WithName("Create Repository")
            .WithOpenApi();

        repositories.MapGet("/{name}/branches", async (
                IMediator mediator,
                string name,
                CancellationToken cancellationToken) =>
            {
                var request = new GetBranches(name);
                var branches = await mediator.Send(request, cancellationToken);

                return Results.Ok(branches.ToResponse());
            })
            .RequirePermission(RepositoryView)
            .Produces<IReadOnlyList<BranchItemResponse>>()
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .ProducesProblem(500)
            .WithName("Get Branches")
            .WithOpenApi();

        repositories.MapGet("/{name}/tree/{**branchOrPath}", async (
                IMediator mediator,
                string name,
                string? branchOrPath,
                CancellationToken cancellationToken) =>
            {
                var request = new GetTreeView(name, branchOrPath);
                var treeView = await mediator.Send(request, cancellationToken);

                return treeView is not null
                    ? Results.Ok(treeView.ToResponse())
                    : Results.NotFound();
            })
            .RequirePermission(RepositoryView)
            .Produces<TreeViewResponse>()
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .ProducesProblem(500)
            .WithName("Get Tree")
            .WithOpenApi();

        repositories.MapGet("/{name}/file/{**branchAndPath}", async (
                IMediator mediator,
                [FromServices] IContentTypeProvider contentTypeProvider,
                string name,
                string branchAndPath,
                CancellationToken cancellationToken) =>
            {
                var request = new GetFile(name, branchAndPath);
                var gitFile = await mediator.Send(request, cancellationToken);
                if (gitFile is null)
                    return Results.NotFound();

                contentTypeProvider.TryGetContentType(gitFile.Name, out var contentType);
                contentType ??= "application/octet-stream";

                return Results.Stream(gitFile.Content, contentType, gitFile.Name);
            })
            .RequirePermission(RepositoryView)
            .Produces(200)
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .ProducesProblem(500)
            .WithName("Get File")
            .WithOpenApi();

        return app;
    }
}