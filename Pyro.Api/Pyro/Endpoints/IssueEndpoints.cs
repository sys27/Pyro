// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;
using Pyro.Contracts.Requests.Issues;
using Pyro.Contracts.Responses.Issues;
using Pyro.Domain.Issues.Commands;
using Pyro.Domain.Issues.Queries;
using Pyro.DtoMappings;
using Pyro.Infrastructure.Shared.DataAccess;
using static Pyro.Domain.Identity.Models.Permission;

namespace Pyro.Endpoints;

internal static class IssueEndpoints
{
    public static IEndpointRouteBuilder MapIssueEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapIssueUsers();

        var issuesBuilder = app.MapGroup("/repositories/{repositoryName}/issues")
            .WithTags("Issues");

        issuesBuilder
            .MapIssues()
            .MapIssueComments()
            .MapIssueStatuses()
            .MapIssueStatusTransitions();

        return app;
    }

    private static IEndpointRouteBuilder MapIssues(this IEndpointRouteBuilder issuesBuilder)
    {
        issuesBuilder.MapGet("/", async (
                IMediator mediator,
                [AsParameters] GetUsersRequest request,
                CancellationToken cancellationToken) =>
            {
                var query = request.ToGetIssues();
                var issues = await mediator.Send(query, cancellationToken);
                var result = issues.ToResponse();

                return Results.Ok(result);
            })
            .RequirePermission(IssueView)
            .Produces<IReadOnlyList<IssueResponse>>()
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .ProducesProblem(500)
            .WithName("Get Issues")
            .WithOpenApi();

        issuesBuilder.MapGet("/{number:int}", async (
                IMediator mediator,
                string repositoryName,
                int number,
                CancellationToken cancellationToken) =>
            {
                var query = new GetIssue(repositoryName, number);
                var issue = await mediator.Send(query, cancellationToken);
                var result = issue?.ToResponse();

                return result is null
                    ? Results.NotFound()
                    : Results.Ok(result);
            })
            .RequirePermission(IssueView)
            .Produces<IssueResponse>()
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .ProducesProblem(500)
            .WithName("Get Issue")
            .WithOpenApi();

        issuesBuilder.MapPost("/", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                string repositoryName,
                CreateIssueRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateIssue(
                    repositoryName,
                    request.Title,
                    request.AssigneeId,
                    request.StatusId,
                    request.Labels);
                var issue = await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                var result = issue.ToResponse();

                return Results.Created($"/repositories/{repositoryName}/issues/{issue.IssueNumber}", result);
            })
            .RequirePermission(IssueEdit)
            .Produces<IssueResponse>(201)
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .ProducesProblem(500)
            .WithName("Create Issue")
            .WithOpenApi();

        issuesBuilder.MapPut("/{number:int}", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                string repositoryName,
                int number,
                UpdateIssueRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateIssue(
                    repositoryName,
                    number,
                    request.Title,
                    request.AssigneeId,
                    request.StatusId,
                    request.Labels);
                var issue = await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                var result = issue.ToResponse();

                return Results.Ok(result);
            })
            .RequirePermission(IssueEdit)
            .Produces<IssueResponse>()
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .ProducesProblem(500)
            .WithName("Update Issue")
            .WithOpenApi();

        issuesBuilder.MapDelete("/{number:int}", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                string repositoryName,
                int number,
                CancellationToken cancellationToken) =>
            {
                var command = new DeleteIssue(repositoryName, number);
                await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Results.Ok();
            })
            .RequirePermission(IssueManage)
            .Produces(200)
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .ProducesProblem(500)
            .WithName("Delete Issue")
            .WithOpenApi();

        issuesBuilder.MapPost("/{number:int}/lock", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                string repositoryName,
                int number,
                CancellationToken cancellationToken) =>
            {
                var command = new LockIssue(repositoryName, number);
                await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Results.Ok();
            })
            .RequirePermission(IssueManage)
            .Produces(200)
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .ProducesProblem(500)
            .WithName("Lock Issue")
            .WithOpenApi();

        issuesBuilder.MapPost("/{number:int}/unlock", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                string repositoryName,
                int number,
                CancellationToken cancellationToken) =>
            {
                var command = new UnlockIssue(repositoryName, number);
                await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Results.Ok();
            })
            .RequirePermission(IssueManage)
            .Produces(200)
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .ProducesProblem(500)
            .WithName("Unlock Issue")
            .WithOpenApi();

        return issuesBuilder;
    }

    private static IEndpointRouteBuilder MapIssueComments(this IEndpointRouteBuilder issuesBuilder)
    {
        var commentsBuilder = issuesBuilder.MapGroup("/{number:int}/comments")
            .WithTags("Issue Comments");

        commentsBuilder.MapGet("/", async (
                IMediator mediator,
                string repositoryName,
                int number,
                CancellationToken cancellationToken) =>
            {
                var query = new GetIssueComments(repositoryName, number);
                var comments = await mediator.Send(query, cancellationToken);

                var result = comments.ToResponse();

                return Results.Ok(result);
            })
            .RequirePermission(IssueView)
            .Produces<IReadOnlyList<IssueCommentResponse>>()
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .ProducesProblem(500)
            .WithName("Get Issue Comments")
            .WithOpenApi();

        commentsBuilder.MapPost("/", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                string repositoryName,
                int number,
                CreateIssueCommentRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateIssueComment(repositoryName, number, request.Content);
                var comment = await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                var result = comment.ToResponse();

                return Results.Created($"/repositories/{repositoryName}/issues/{number}/comments/{comment.Id}", result);
            })
            .RequirePermission(IssueEdit)
            .Produces<IssueCommentResponse>(201)
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .ProducesProblem(500)
            .WithName("Create Issue Comment")
            .WithOpenApi();

        commentsBuilder.MapPut("/{commentId:guid}", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                string repositoryName,
                int number,
                Guid commentId,
                UpdateIssueCommentRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateIssueComment(repositoryName, number, commentId, request.Content);
                var comment = await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                var result = comment.ToResponse();

                return Results.Ok(result);
            })
            .RequirePermission(IssueEdit)
            .Produces<IssueCommentResponse>()
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .ProducesProblem(500)
            .WithName("Update Issue Comment")
            .WithOpenApi();

        commentsBuilder.MapDelete("/{id:guid}", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                string repositoryName,
                int number,
                Guid id,
                CancellationToken cancellationToken) =>
            {
                var command = new DeleteIssueComment(repositoryName, number, id);
                await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Results.Ok();
            })
            .RequirePermission(IssueManage)
            .Produces(200)
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .ProducesProblem(500)
            .WithName("Delete Issue Comment")
            .WithOpenApi();

        return issuesBuilder;
    }

    private static IEndpointRouteBuilder MapIssueUsers(this IEndpointRouteBuilder app)
    {
        app.MapGet("/repositories/issues/users", async (
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetUsers();
                var users = await mediator.Send(query, cancellationToken);
                var result = users.ToResponse();

                return Results.Ok(result);
            })
            .RequirePermission(IssueView)
            .WithTags("Issues")
            .Produces<IReadOnlyList<UserResponse>>()
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .ProducesProblem(500)
            .WithName("Get Issue Users")
            .WithOpenApi();

        return app;
    }

    private static IEndpointRouteBuilder MapIssueStatuses(this IEndpointRouteBuilder app)
    {
        var statusBuilder = app.MapGroup("/statuses")
            .WithTags("Issue Statuses");

        statusBuilder.MapGet("/", async (
                IMediator mediator,
                string repositoryName,
                string? statusName,
                CancellationToken cancellationToken) =>
            {
                var query = new GetIssueStatuses(repositoryName, statusName);
                var statuses = await mediator.Send(query, cancellationToken);
                var result = statuses.ToResponse();

                return Results.Ok(result);
            })
            .RequirePermission(IssueView)
            .Produces<IReadOnlyList<IssueStatusResponse>>()
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .ProducesProblem(500)
            .WithName("Get Statuses")
            .WithOpenApi();

        statusBuilder.MapGet("/{id:guid}", async (
                IMediator mediator,
                string repositoryName,
                Guid id,
                CancellationToken cancellationToken) =>
            {
                var query = new GetIssueStatus(repositoryName, id);
                var statuses = await mediator.Send(query, cancellationToken);
                var result = statuses?.ToResponse();

                return result is null
                    ? Results.NotFound()
                    : Results.Ok(result);
            })
            .RequirePermission(IssueView)
            .Produces<IssueStatusResponse>()
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .ProducesProblem(500)
            .WithName("Get Status")
            .WithOpenApi();

        statusBuilder.MapPost("/", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                string repositoryName,
                CreateIssueStatusRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateIssueStatus(repositoryName, request.Name, request.Color.ToInt());
                var status = await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                var result = status.ToResponse();

                return Results.Created($"/repositories/{repositoryName}/issues/statuses/{status.Id}", result);
            })
            .RequirePermission(IssueManage)
            .Produces<IssueResponse>(201)
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .ProducesProblem(500)
            .WithName("Create Status")
            .WithOpenApi();

        statusBuilder.MapPut("/{id:guid}", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                string repositoryName,
                Guid id,
                UpdateIssueStatusRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateIssueStatus(repositoryName, id, request.Name, request.Color.ToInt());
                var status = await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                var result = status.ToResponse();

                return Results.Ok(result);
            })
            .RequirePermission(IssueManage)
            .Produces<IssueStatusResponse>()
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .ProducesProblem(500)
            .WithName("Update Status")
            .WithOpenApi();

        statusBuilder.MapDelete("/{id:guid}", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                string repositoryName,
                Guid id,
                CancellationToken cancellationToken) =>
            {
                var command = new DeleteIssueStatus(repositoryName, id);
                await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Results.Ok();
            })
            .RequirePermission(IssueManage)
            .Produces(200)
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .ProducesProblem(500)
            .WithName("Delete Status")
            .WithOpenApi();

        return app;
    }

    private static IEndpointRouteBuilder MapIssueStatusTransitions(this IEndpointRouteBuilder app)
    {
        var transitionsBuilder = app.MapGroup("/statuses/transitions")
            .WithTags("Issue Status Transitions");

        transitionsBuilder.MapGet("/", async (
                IMediator mediator,
                string repositoryName,
                CancellationToken cancellationToken) =>
            {
                var query = new GetAllIssueStatusTransitions(repositoryName);
                var transitions = await mediator.Send(query, cancellationToken);
                var result = transitions.ToResponse();

                return Results.Ok(result);
            })
            .RequirePermission(IssueView)
            .Produces<IReadOnlyList<IssueStatusTransitionResponse>>()
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .ProducesProblem(500)
            .WithTags("Issue Status Transitions")
            .WithName("Get All Status Transitions")
            .WithOpenApi();

        transitionsBuilder.MapPost("/", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                string repositoryName,
                CreateIssueStatusTransitionRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateIssueStatusTransition(repositoryName, request.FromId, request.ToId);
                var transition = await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                var result = transition.ToResponse();

                return Results.Created($"/repositories/{repositoryName}/issues/statuses/transitions/{transition.Id}", result);
            })
            .RequirePermission(IssueManage)
            .Produces<IssueStatusTransitionResponse>(201)
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .ProducesProblem(500)
            .WithName("Create Status Transition")
            .WithOpenApi();

        transitionsBuilder.MapDelete("/{id:guid}", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                string repositoryName,
                Guid id,
                CancellationToken cancellationToken) =>
            {
                var command = new DeleteIssueStatusTransition(repositoryName, id);
                await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Results.Ok();
            })
            .RequirePermission(IssueManage)
            .Produces(200)
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .ProducesProblem(500)
            .WithName("Delete Status Transition")
            .WithOpenApi();

        return app;
    }
}