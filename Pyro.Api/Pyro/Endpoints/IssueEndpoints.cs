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
            .MapIssueStatusTransitions()
            .MapIssueChangeLogs();

        return app;
    }

    private static IEndpointRouteBuilder MapIssues(this IEndpointRouteBuilder issuesBuilder)
    {
        issuesBuilder.MapGet("/", async (
                IMediator mediator,
                [AsParameters] GetIssuesRequest request,
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
            .WithName("GetIssues")
            .WithSummary("Get all issues")
            .WithDescription("Get all issues in the repository")
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
            .WithName("GetIssue")
            .WithSummary("Get issue")
            .WithDescription("Get issue by number")
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
                    request.Labels,
                    request.InitialComment);
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
            .WithName("CreateIssue")
            .WithSummary("Create issue")
            .WithDescription("Create a new issue")
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
            .WithName("UpdateIssue")
            .WithSummary("Update issue")
            .WithDescription("Update issue by number")
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
            .WithName("DeleteIssue")
            .WithSummary("Delete issue")
            .WithDescription("Delete issue by number")
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
            .WithName("LockIssue")
            .WithSummary("Lock issue")
            .WithDescription("Lock issue by number")
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
            .WithName("UnlockIssue")
            .WithSummary("Unlock issue")
            .WithDescription("Unlock issue by number")
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
            .WithName("GetIssueComments")
            .WithSummary("Get issue comments")
            .WithDescription("Get all comments for the issue")
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
            .WithName("CreateIssueComment")
            .WithSummary("Create issue comment")
            .WithDescription("Create a new comment for the issue")
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
            .WithName("UpdateIssueComment")
            .WithSummary("Update issue comment")
            .WithDescription("Update a comment for the issue")
            .WithOpenApi();

        // TODO: hide instead of delete?
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
            .WithName("DeleteIssueComment")
            .WithSummary("Delete issue comment")
            .WithDescription("Delete a comment for the issue")
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
            .WithName("GetIssueUsers")
            .WithSummary("Get issue users")
            .WithDescription("Get all users that can be assigned to issues")
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
            .WithName("GetStatuses")
            .WithSummary("Get all statuses")
            .WithDescription("Get all statuses in the repository")
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
            .WithName("GetStatus")
            .WithSummary("Get status")
            .WithDescription("Get status by id")
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
            .WithName("CreateStatus")
            .WithSummary("Create status")
            .WithDescription("Create a new status in the repository")
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
            .WithName("UpdateStatus")
            .WithSummary("Update status")
            .WithDescription("Update status by id")
            .WithOpenApi();

        statusBuilder.MapPost("/{id:guid}/enable", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                string repositoryName,
                Guid id,
                CancellationToken cancellationToken) =>
            {
                var command = new EnableIssueStatus(repositoryName, id);
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
            .WithName("EnableStatus")
            .WithSummary("Enable status")
            .WithDescription("Enable status by id")
            .WithOpenApi();

        statusBuilder.MapPost("/{id:guid}/disable", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                string repositoryName,
                Guid id,
                CancellationToken cancellationToken) =>
            {
                var command = new DisableIssueStatus(repositoryName, id);
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
            .WithName("DisableStatus")
            .WithSummary("Disable status")
            .WithDescription("Disable status by id")
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
            .WithName("GetAllStatusTransitions")
            .WithSummary("Get all status transitions")
            .WithDescription("Get all status transitions in the repository")
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
            .WithName("CreateStatusTransition")
            .WithSummary("Create status transition")
            .WithDescription("Create a new status transition in the repository")
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
            .WithName("DeleteStatusTransition")
            .WithSummary("Delete status transition")
            .WithDescription("Delete status transition by id")
            .WithOpenApi();

        return app;
    }

    private static IEndpointRouteBuilder MapIssueChangeLogs(this IEndpointRouteBuilder app)
    {
        var changeLogBuilder = app.MapGroup("/{number:int}/change-logs")
            .WithTags("Issue Change Logs");

        changeLogBuilder.MapGet("/", async (
                IMediator mediator,
                string repositoryName,
                int number,
                CancellationToken cancellationToken) =>
            {
                var query = new GetIssueChangeLogs(repositoryName, number);
                var changeLogs = await mediator.Send(query, cancellationToken);
                var result = changeLogs.ToResponse();

                return Results.Ok(result);
            })
            .RequirePermission(IssueView)
            .Produces<IReadOnlyList<IssueChangeLogResponse>>()
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .ProducesProblem(500)
            .WithName("GetIssueChangeLogs")
            .WithSummary("Get issue change logs")
            .WithDescription("Get all change logs for the issue")
            .WithOpenApi();

        return app;
    }
}