// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.UserProfiles;

public record UpdateProfile(string? Name, string? Status) : IRequest;

public class UpdateProfileValidator : AbstractValidator<UpdateProfile>
{
    public UpdateProfileValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(50);

        RuleFor(x => x.Status)
            .MaximumLength(150);
    }
}

public class UpdateProfileHandler : IRequestHandler<UpdateProfile>
{
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IUserProfileRepository repository;

    public UpdateProfileHandler(
        ICurrentUserProvider currentUserProvider,
        IUserProfileRepository repository)
    {
        this.currentUserProvider = currentUserProvider;
        this.repository = repository;
    }

    public async Task Handle(UpdateProfile request, CancellationToken cancellationToken)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        var profile = await repository.GetUserProfile(currentUser.Id, cancellationToken);
        if (profile is null)
            throw new NotFoundException("User profile not found");

        profile.Name = request.Name;
        profile.Status = request.Status;
    }
}