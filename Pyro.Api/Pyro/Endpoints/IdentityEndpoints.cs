// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Net.Mime;
using MediatR;
using Pyro.Contracts.Requests;
using Pyro.Contracts.Requests.Identity;
using Pyro.Contracts.Responses.Identity;
using Pyro.Domain.Identity.Commands;
using Pyro.Domain.Identity.Models;
using Pyro.Domain.Identity.Queries;
using Pyro.DtoMappings;
using Pyro.Infrastructure.Shared.DataAccess;

namespace Pyro.Endpoints;

internal static class IdentityEndpoints
{
    public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder app)
        => app
            .MapUserEndpoints()
            .MapRoleEndpoints()
            .MapPermissionEndpoints()
            .MapAuthEndpoints()
            .MapProfileEndpoints();

    private static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var usersBuilder = app.MapGroup("/users")
            .WithTags("Users");

        usersBuilder.MapGet("/", async (
                IMediator mediator,
                [AsParameters] PageRequest<string> request,
                CancellationToken cancellationToken) =>
            {
                var query = request.ToGetUsers();
                var users = await mediator.Send(query, cancellationToken);
                var result = users.ToResponse();

                return Results.Ok(result);
            })
            .RequirePermission(Permission.UserView)
            .Produces<IReadOnlyList<UserResponse>>()
            .Produces(401)
            .Produces(403)
            .ProducesProblem(500, MediaTypeNames.Application.Json)
            .WithName("GetUsers")
            .WithSummary("Get Users")
            .WithDescription("Get all users.")
            .WithOpenApi();

        usersBuilder.MapGet("/{login}", async (
                IMediator mediator,
                string login,
                CancellationToken cancellationToken) =>
            {
                var request = new GetUser(login);
                var user = await mediator.Send(request, cancellationToken);
                var result = user?.ToResponse();

                return result is not null
                    ? Results.Ok(result)
                    : Results.NotFound();
            })
            .RequirePermission(Permission.UserView)
            .Produces<UserResponse>()
            .Produces(404)
            .ProducesProblem(500)
            .WithName("GetUserByLogin")
            .WithSummary("Get User by login")
            .WithDescription("Get user by login.")
            .WithOpenApi();

        usersBuilder.MapPost("/", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                CreateUserRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = request.ToCommand();
                var user = await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                var result = user.ToResponse();

                return Results.Created($"/api/users/{request.Login}", result);
            })
            .RequirePermission(Permission.UserEdit)
            .Produces(201)
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .ProducesProblem(500)
            .WithName("CreateUser")
            .WithSummary("Create User")
            .WithDescription("Create a new user.")
            .WithOpenApi();

        usersBuilder.MapPut("/{login}", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                string login,
                UpdateUserRequest request,
                CancellationToken cancellationToken) =>
            {
                var user = await mediator.Send(new GetUser(login), cancellationToken);
                if (user is null)
                    return Results.NotFound();

                var command = new UpdateUser(user, request.Roles);
                user = await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Results.Ok(user.ToResponse());
            })
            .RequirePermission(Permission.UserEdit)
            .Produces<UserResponse>()
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .ProducesProblem(500)
            .WithName("UpdateUser")
            .WithSummary("Update User")
            .WithDescription("Update user.")
            .WithOpenApi();

        usersBuilder.MapPost("/{login}/lock", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                string login,
                CancellationToken cancellationToken) =>
            {
                var command = new LockUser(login);
                await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Results.Ok();
            })
            .RequirePermission(Permission.UserManage)
            .Produces(200)
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .ProducesProblem(500)
            .WithName("Lock User")
            .WithOpenApi();

        usersBuilder.MapPost("/{login}/unlock", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                string login,
                CancellationToken cancellationToken) =>
            {
                var command = new UnlockUser(login);
                await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Results.Ok();
            })
            .RequirePermission(Permission.UserManage)
            .Produces(200)
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .ProducesProblem(500)
            .WithName("UnlockUser")
            .WithSummary("Unlock User")
            .WithDescription("Unlock user.")
            .WithOpenApi();

        usersBuilder.MapPost("/activate", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                ActivateUserRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = request.ToCommand();
                await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Results.Ok();
            })
            .AllowAnonymous()
            .Produces(200)
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .ProducesProblem(500)
            .WithName("ActivateUser")
            .WithSummary("Activate User")
            .WithDescription("Activate user.")
            .WithOpenApi();

        usersBuilder.MapPost("/change-password", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                ChangePasswordRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = request.ToCommand();
                await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Results.Ok();
            })
            .Produces(200)
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .ProducesProblem(500)
            .WithName("ChangePassword")
            .WithSummary("Change Password")
            .WithDescription("Change user password.")
            .WithOpenApi();

        usersBuilder.MapPost("/forgot-password", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                ForgotPasswordRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = request.ToCommand();
                await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Results.Ok();
            })
            .AllowAnonymous()
            .Produces(200)
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .ProducesProblem(500)
            .WithName("ForgotPassword")
            .WithSummary("Forgot Password")
            .WithDescription("Request password reset.")
            .WithOpenApi();

        usersBuilder.MapPost("/reset-password", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                ResetPasswordRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = request.ToCommand();
                await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Results.Ok();
            })
            .AllowAnonymous()
            .Produces(200)
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .ProducesProblem(500)
            .WithName("ResetPassword")
            .WithSummary("Reset Password")
            .WithDescription("Reset user password.")
            .WithOpenApi();

        var accessTokenBuilder = usersBuilder.MapGroup("/access-tokens")
            .WithTags("Access Tokens");

        accessTokenBuilder.MapGet("/", async (
                IMediator mediator,
                string? accessTokenName,
                CancellationToken cancellationToken) =>
            {
                var request = new GetAccessTokens(accessTokenName);
                var accessTokens = await mediator.Send(request, cancellationToken);
                var result = accessTokens.ToResponse();

                return Results.Ok(result);
            })
            .Produces<IReadOnlyList<AccessTokenResponse>>()
            .Produces(401)
            .Produces(403)
            .ProducesProblem(500)
            .WithName("GetAccessTokens")
            .WithSummary("Get Access Tokens")
            .WithDescription("Get all access tokens.")
            .WithOpenApi();

        accessTokenBuilder.MapPost("/", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                CreateAccessTokenRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = request.ToCommand();
                var result = await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Results.Created((string)null!, result.ToResponse());
            })
            .Produces<CreateAccessTokenResult>()
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .ProducesProblem(500)
            .WithName("CreateAccessToken")
            .WithSummary("Create Access Token")
            .WithDescription("Create a new access token.")
            .WithOpenApi();

        accessTokenBuilder.MapDelete("/{name}", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                string name,
                CancellationToken cancellationToken) =>
            {
                var command = new DeleteAccessToken(name);
                await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Results.Ok();
            })
            .Produces(200)
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .ProducesProblem(500)
            .WithName("DeleteAccessToken")
            .WithSummary("Delete Access Token")
            .WithDescription("Delete access token.")
            .WithOpenApi();

        return app;
    }

    private static IEndpointRouteBuilder MapRoleEndpoints(this IEndpointRouteBuilder app)
    {
        var rolesBuilder = app.MapGroup("/roles")
            .WithTags("Roles");

        rolesBuilder.MapGet("/", async (
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var request = new GetRoles();
                var roles = await mediator.Send(request, cancellationToken);
                var result = roles.ToResponse();

                return Results.Ok(result);
            })
            .AllowAnonymous()
            .Produces<IReadOnlyList<RoleResponse>>()
            .Produces(401)
            .Produces(403)
            .ProducesProblem(500)
            .WithName("GetRoles")
            .WithSummary("Get Roles")
            .WithDescription("Get all roles.")
            .WithOpenApi();

        return app;
    }

    private static IEndpointRouteBuilder MapPermissionEndpoints(this IEndpointRouteBuilder app)
    {
        var permissionsBuilder = app.MapGroup("/permissions")
            .WithTags("Permissions");

        permissionsBuilder.MapGet("/", async (
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var request = new GetPermissions();
                var permissions = await mediator.Send(request, cancellationToken);
                var result = permissions.ToResponse();

                return Results.Ok(result);
            })
            .AllowAnonymous()
            .Produces<IReadOnlyList<PermissionResponse>>()
            .Produces(401)
            .Produces(403)
            .ProducesProblem(500)
            .WithName("GetPermissions")
            .WithSummary("Get Permissions")
            .WithDescription("Get all permissions.")
            .WithOpenApi();

        return app;
    }

    private static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var identity = app.MapGroup("/identity")
            .WithTags("Identity");

        identity.MapPost("/login", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                LoginRequest request,
                CancellationToken cancellationToken = default) =>
            {
                var command = new LoginCommand(request.Login, request.Password);
                var result = await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return result.IsSuccess
                    ? Results.Ok(result.TokenPair.ToResponse())
                    : Results.Unauthorized();
            })
            .AllowAnonymous()
            .Produces<TokenPairResponse>()
            .ProducesValidationProblem()
            .Produces(401)
            .ProducesProblem(500)
            .WithName("Login")
            .WithSummary("Login")
            .WithDescription("Login user.")
            .WithOpenApi();

        identity.MapPost("/logout", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                CancellationToken cancellationToken = default) =>
            {
                var command = new Logout();
                await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Results.NoContent();
            })
            .Produces(204)
            .ProducesProblem(500)
            .WithName("Logout")
            .WithSummary("Logout")
            .WithDescription("Logout user.")
            .WithOpenApi();

        identity.MapPost("/refresh", async (
                IMediator mediator,
                RefreshTokenRequest request,
                CancellationToken cancellationToken = default) =>
            {
                var command = new RefreshToken(request.RefreshToken);
                var result = await mediator.Send(command, cancellationToken);

                return result.IsSuccess
                    ? Results.Ok(result.ToResponse())
                    : Results.Unauthorized();
            })
            .Produces<TokenResponse>()
            .ProducesValidationProblem()
            .Produces(401)
            .ProducesProblem(500)
            .WithName("RefreshToken")
            .WithSummary("Refresh Token")
            .WithDescription("Refresh user token.")
            .AllowAnonymous()
            .WithOpenApi();

        return app;
    }

    private static IEndpointRouteBuilder MapProfileEndpoints(this IEndpointRouteBuilder app)
    {
        var profileBuilder = app.MapGroup("/profile")
            .WithTags("Profile");

        profileBuilder.MapGet("/", async (
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var request = new GetUserProfile();
                var profile = await mediator.Send(request, cancellationToken);
                var result = profile.ToResponse();

                return Results.Ok(result);
            })
            .Produces<UserProfileResponse>()
            .Produces(401)
            .Produces(403)
            .ProducesProblem(500)
            .WithName("GetCurrentProfile")
            .WithSummary("Get Current Profile")
            .WithDescription("Get current user profile.")
            .WithOpenApi();

        profileBuilder.MapPut("/", async (
                IMediator mediator,
                UnitOfWork unitOfWork,
                UpdateUserProfileRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = request.ToCommand();
                await mediator.Send(command, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Results.NoContent();
            })
            .Produces(204)
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403)
            .ProducesProblem(500)
            .WithName("UpdateCurrentProfile")
            .WithSummary("Update Current Profile")
            .WithDescription("Update current user profile.")
            .WithOpenApi();

        return app;
    }
}