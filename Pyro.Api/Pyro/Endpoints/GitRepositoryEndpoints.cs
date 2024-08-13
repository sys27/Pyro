// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Pyro.Contracts.Requests;
using Pyro.Contracts.Requests.Issues;
using Pyro.Contracts.Responses;
using Pyro.Domain.GitRepositories.Commands;
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
        var repositoryBuilder = app.MapGroup("/repositories")
            .WithTags("Repositories");

        repositoryBuilder
            .MapRepositories()
            .MapTags();

        return app;
    }

    private static IEndpointRouteBuilder MapRepositories(this IEndpointRouteBuilder repositoryBuilder)
    {
        repositoryBuilder.MapGet("/{name}", async (
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

        repositoryBuilder.MapGet("/", async (
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

        repositoryBuilder.MapPost("/", async (
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

        repositoryBuilder.MapGet("/{name}/branches", async (
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

        repositoryBuilder.MapGet("/{name}/tree/{**branchOrPath}", async (
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

        repositoryBuilder.MapGet("/{name}/file/{**branchAndPath}", async (
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

        return repositoryBuilder;
    }

    private static IEndpointRouteBuilder MapTags(this IEndpointRouteBuilder app)
    {
        var tagsBuilder = app.MapGroup("/{name}/tags")
            .WithTags("Repository Tags");

        tagsBuilder.MapGet("/", async (
                IMediator mediator,
                string name,
                CancellationToken cancellationToken) =>
            {
                var query = new GetTags(name);
                var tags = await mediator.Send(query, cancellationToken);
                var result = tags.ToResponse();

                return result;
            })
            .RequirePermission(IssueView)
            .Produces<IReadOnlyList<TagResponse>>()
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .ProducesProblem(500)
            .WithName("Get Tags")
            .WithOpenApi();

        tagsBuilder.MapGet("/{id:guid}", async (
                IMediator mediator,
                [FromRoute] string name,
                [FromRoute] Guid id,
                CancellationToken cancellationToken) =>
            {
                var query = new GetTag(name, id);
                var tag = await mediator.Send(query, cancellationToken);
                var result = tag.ToResponse();

                return result;
            })
            .RequirePermission(IssueView)
            .Produces<TagResponse>()
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .ProducesProblem(500)
            .WithName("Get Tag")
            .WithOpenApi();

        tagsBuilder.MapPost("/", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                [FromRoute] string name,
                [FromBody] CreateTagRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateTag(name, request.Name, request.Color.ToInt());
                var tag = await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                var result = tag.ToResponse();

                return Results.Created($"/repositories/{name}/tags/{tag.Name}", result);
            })
            .RequirePermission(IssueEdit)
            .Produces<TagResponse>(201)
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .ProducesProblem(500)
            .WithName("Create Tag")
            .WithOpenApi();

        tagsBuilder.MapPut("/{id:guid}", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                [FromRoute] string name,
                [FromRoute] Guid id,
                [FromBody] UpdateTagRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateTag(name, id, request.Name, request.Color.ToInt());
                var updatedTag = await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                var result = updatedTag.ToResponse();

                return Results.Ok(result);
            })
            .RequirePermission(RepositoryEdit)
            .Produces<TagResponse>()
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .ProducesProblem(500)
            .WithName("Update Tag")
            .WithOpenApi();

        tagsBuilder.MapDelete("/{id:guid}", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                string name,
                Guid id,
                CancellationToken cancellationToken) =>
            {
                var command = new DeleteTag(name, id);
                await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Results.Ok();
            })
            .RequirePermission(IssueEdit)
            .Produces(200)
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .ProducesProblem(500)
            .WithName("Delete Tag")
            .WithOpenApi();

        return app;
    }
}