// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;
using Pyro.Domain.Identity.Commands;
using Pyro.Domain.Identity.Queries;
using Pyro.Dtos.Mapping;
using Pyro.Dtos.Requests;
using Pyro.Dtos.Responses;
using Pyro.Infrastructure.DataAccess;

namespace Pyro.Endpoints;

public static class IdentityEndpoints
{
    public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder app)
        => app
            .MapUserEndpoints()
            .MapRoleEndpoints()
            .MapPermissionEndpoints()
            .MapAuthEndpoints();

    private static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var usersBuilder = app.MapGroup("/users")
            .WithTags("Users");

        usersBuilder.MapGet("/", async (
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var request = new GetUsers();
                var users = await mediator.Send(request, cancellationToken);
                var result = users.ToResponse();

                return Results.Ok(result);
            })
            .Produces<IReadOnlyCollection<UserResponse>>()
            .Produces(401)
            .Produces(403)
            .WithName("Get Users")
            .WithOpenApi();

        usersBuilder.MapGet("/{email}", async (
                IMediator mediator,
                string email,
                CancellationToken cancellationToken) =>
            {
                var request = new GetUser(email);
                var user = await mediator.Send(request, cancellationToken);
                var result = user?.ToResponse();

                return result is not null
                    ? Results.Ok(result)
                    : Results.NotFound();
            })
            .Produces<UserResponse>()
            .Produces(404)
            .WithName("Get User By Email")
            .WithOpenApi();

        usersBuilder.MapPost("/", async (
                IMediator mediator,
                PyroDbContext dbContext,
                CreateUserRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = request.ToCommand();
                await mediator.Send(command, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);

                return Results.Created($"/users/{request.Email}", new { request.Email });
            })
            .Produces(201)
            .Produces(400)
            .Produces(401)
            .Produces(403)
            .WithName("Create User")
            .WithOpenApi();

        usersBuilder.MapPut("/{email}", async (
                IMediator mediator,
                PyroDbContext dbContext,
                string email,
                UpdateUserRequest request,
                CancellationToken cancellationToken) =>
            {
                var user = await mediator.Send(new GetUser(email), cancellationToken);
                if (user is null)
                    return Results.NotFound();

                var command = new UpdateUser(user, request.Roles);
                user = await mediator.Send(command, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);

                return Results.Ok(user.ToResponse());
            })
            .Produces<UserResponse>()
            .Produces(400)
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .WithName("Update User")
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
            .CacheOutput(b => b.Tag("roles"))
            .Produces<IReadOnlyList<RoleResponse>>()
            .Produces(401)
            .Produces(403)
            .WithName("Get Roles")
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
            .CacheOutput(b => b.Tag("permissions"))
            .Produces<IReadOnlyList<PermissionResponse>>()
            .Produces(401)
            .Produces(403)
            .WithName("Get Permissions")
            .WithOpenApi();

        return app;
    }

    private static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var identity = app.MapGroup("/identity")
            .WithTags("Identity");

        identity.MapPost("/login", async (
                IMediator mediator,
                PyroDbContext dbContext,
                LoginRequest request,
                CancellationToken cancellationToken = default) =>
            {
                var command = new Login(request.Email, request.Password);
                var result = await mediator.Send(command, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);

                return result.IsSuccess
                    ? Results.Ok(result.TokenPair.ToResponse())
                    : Results.Unauthorized();
            })
            .Produces<TokenPairResponse>()
            .Produces(400)
            .Produces(401)
            .WithName("Login")
            .AllowAnonymous()
            .WithOpenApi();

        identity.MapPost("/logout", async (
                IMediator mediator,
                PyroDbContext dbContext,
                CancellationToken cancellationToken = default) =>
            {
                var command = new Logout();
                await mediator.Send(command, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);

                return Results.NoContent();
            })
            .Produces(204)
            .WithName("Logout")
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
            .Produces(400)
            .Produces(401)
            .WithName("Refresh Token")
            .AllowAnonymous()
            .WithOpenApi();

        return app;
    }
}