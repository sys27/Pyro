// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;
using Pyro.Domain.Core;
using Pyro.Domain.Core.Exceptions;

namespace Pyro.Domain.UserProfiles;

public record GetUserProfile : IRequest<UserProfile>;

public class GetUserProfileHandler : IRequestHandler<GetUserProfile, UserProfile>
{
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IUserProfileRepository repository;

    public GetUserProfileHandler(
        ICurrentUserProvider currentUserProvider,
        IUserProfileRepository repository)
    {
        this.currentUserProvider = currentUserProvider;
        this.repository = repository;
    }

    public async Task<UserProfile> Handle(GetUserProfile request, CancellationToken cancellationToken)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        var profile = await repository.GetUserProfile(currentUser.Id, cancellationToken);
        if (profile is null)
            throw new NotFoundException("User profile not found");

        return profile;
    }
}