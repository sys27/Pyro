// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;
using Pyro.Contracts.Requests;
using Pyro.Contracts.Responses;
using Pyro.Domain.UserProfiles;
using Pyro.DtoMappings;
using Pyro.Infrastructure.Shared.DataAccess;

namespace Pyro.Endpoints;

internal static class ProfileEndpoints
{
    public static IEndpointRouteBuilder MapProfileEndpoints(this IEndpointRouteBuilder app)
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
            .WithName("Get Current Profile")
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
            .WithName("Update Current Profile")
            .WithOpenApi();

        return app;
    }
}