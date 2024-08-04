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
        var issuesBuilder = app.MapGroup("/repositories/{name}/issues")
            .WithTags("Issues");

        issuesBuilder.MapGet("/", async (
                IMediator mediator,
                string name,
                CancellationToken cancellationToken) =>
            {
                var query = new GetIssues(name);
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
                string name,
                int number,
                CancellationToken cancellationToken) =>
            {
                var query = new GetIssue(name, number);
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
                string name,
                CreateIssueRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateIssue(name, request.Title, request.AssigneeId);
                var issue = await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                var result = issue.ToResponse();

                return Results.Created($"/repositories/{name}/issues/{issue.IssueNumber}", result);
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
                string name,
                int number,
                UpdateIssueRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateIssue(name, number, request.Title, request.AssigneeId);
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
                string name,
                int number,
                CancellationToken cancellationToken) =>
            {
                var command = new DeleteIssue(name, number);
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

        var commentsBuilder = issuesBuilder.MapGroup("/{number:int}/comments")
            .WithTags("Issue Comments");

        commentsBuilder.MapGet("/", async (
                IMediator mediator,
                string name,
                int number,
                CancellationToken cancellationToken) =>
            {
                var query = new GetIssueComments(name, number);
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
                string name,
                int number,
                CreateIssueCommentRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateIssueComment(name, number, request.Content);
                var comment = await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                var result = comment.ToResponse();

                return Results.Created($"/repositories/{name}/issues/{number}/comments/{comment.Id}", result);
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
                string name,
                int number,
                Guid commentId,
                UpdateIssueCommentRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateIssueComment(name, number, commentId, request.Content);
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
                string name,
                int number,
                Guid id,
                CancellationToken cancellationToken) =>
            {
                var command = new DeleteIssueComment(name, number, id);
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

        return app;
    }
}