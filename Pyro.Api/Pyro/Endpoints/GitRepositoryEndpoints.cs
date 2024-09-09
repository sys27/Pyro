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
            .MapLabels();

        return app;
    }

    private static IEndpointRouteBuilder MapRepositories(this IEndpointRouteBuilder repositoryBuilder)
    {
        repositoryBuilder.MapGet("/{repositoryName}", async (
                IMediator mediator,
                string repositoryName,
                CancellationToken cancellationToken) =>
            {
                var request = new GetGitRepository(repositoryName);
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

        repositoryBuilder.MapGet("/{repositoryName}/branches", async (
                IMediator mediator,
                string repositoryName,
                CancellationToken cancellationToken) =>
            {
                var request = new GetBranches(repositoryName);
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

        repositoryBuilder.MapGet("/{repositoryName}/tree/{**branchOrPath}", async (
                IMediator mediator,
                string repositoryName,
                string? branchOrPath,
                CancellationToken cancellationToken) =>
            {
                var request = new GetTreeView(repositoryName, branchOrPath);
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

        repositoryBuilder.MapGet("/{repositoryName}/file/{**branchAndPath}", async (
                IMediator mediator,
                [FromServices] IContentTypeProvider contentTypeProvider,
                string repositoryName,
                string branchAndPath,
                CancellationToken cancellationToken) =>
            {
                var request = new GetFile(repositoryName, branchAndPath);
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

    private static IEndpointRouteBuilder MapLabels(this IEndpointRouteBuilder app)
    {
        var labelsBuilder = app.MapGroup("/{repositoryName}/labels")
            .WithTags("Repository Labels");

        labelsBuilder.MapGet("/", async (
                IMediator mediator,
                string repositoryName,
                string? labelName,
                CancellationToken cancellationToken) =>
            {
                var query = new GetLabels(repositoryName, labelName);
                var labels = await mediator.Send(query, cancellationToken);
                var result = labels.ToResponse();

                return result;
            })
            .RequirePermission(IssueView)
            .Produces<IReadOnlyList<LabelResponse>>()
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .ProducesProblem(500)
            .WithName("Get Labels")
            .WithOpenApi();

        labelsBuilder.MapGet("/{id:guid}", async (
                IMediator mediator,
                [FromRoute] string repositoryName,
                [FromRoute] Guid id,
                CancellationToken cancellationToken) =>
            {
                var query = new GetLabel(repositoryName, id);
                var label = await mediator.Send(query, cancellationToken);
                var result = label.ToResponse();

                return result;
            })
            .RequirePermission(IssueView)
            .Produces<LabelResponse>()
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .ProducesProblem(500)
            .WithName("Get Label")
            .WithOpenApi();

        labelsBuilder.MapPost("/", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                [FromRoute] string repositoryName,
                [FromBody] CreateLabelRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateLabel(repositoryName, request.Name, request.Color.ToInt());
                var label = await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                var result = label.ToResponse();

                return Results.Created($"/repositories/{repositoryName}/labels/{label.Name}", result);
            })
            .RequirePermission(IssueEdit)
            .Produces<LabelResponse>(201)
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .ProducesProblem(500)
            .WithName("Create Label")
            .WithOpenApi();

        labelsBuilder.MapPut("/{id:guid}", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                [FromRoute] string repositoryName,
                [FromRoute] Guid id,
                [FromBody] UpdateLabelRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateLabel(repositoryName, id, request.Name, request.Color.ToInt());
                var label = await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                var result = label.ToResponse();

                return Results.Ok(result);
            })
            .RequirePermission(RepositoryEdit)
            .Produces<LabelResponse>()
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .ProducesProblem(500)
            .WithName("Update Label")
            .WithOpenApi();

        labelsBuilder.MapPost("/{id:guid}/enable", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                [FromRoute] string repositoryName,
                [FromRoute] Guid id,
                CancellationToken cancellationToken) =>
            {
                var command = new EnableLabel(repositoryName, id);
                await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Results.Ok();
            })
            .RequirePermission(RepositoryManage)
            .Produces(200)
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .ProducesProblem(500)
            .WithName("Enable Label")
            .WithOpenApi();

        labelsBuilder.MapPost("/{id:guid}/disable", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                [FromRoute] string repositoryName,
                [FromRoute] Guid id,
                CancellationToken cancellationToken) =>
            {
                var command = new DisableLabel(repositoryName, id);
                await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Results.Ok();
            })
            .RequirePermission(RepositoryManage)
            .Produces(200)
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .ProducesProblem(500)
            .WithName("Disable Label")
            .WithOpenApi();

        return app;
    }
}