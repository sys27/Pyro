// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;
using Pyro.Domain.Identity.Commands;
using Pyro.Domain.Identity.Queries;
using Pyro.Dtos;
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
        var users = app.MapGroup("/users")
            .WithTags("Users");

        users.MapGet("/{email}", async (
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
        });

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
        });

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