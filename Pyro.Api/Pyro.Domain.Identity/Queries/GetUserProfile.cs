// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;
using Pyro.Domain.Shared.CurrentUserProvider;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Identity.Queries;

public record GetUserProfile : IRequest<UserProfile>;

public record UserProfile(string DisplayName, string Email);

public class GetUserProfileHandler : IRequestHandler<GetUserProfile, UserProfile>
{
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IUserRepository repository;

    public GetUserProfileHandler(
        ICurrentUserProvider currentUserProvider,
        IUserRepository repository)
    {
        this.currentUserProvider = currentUserProvider;
        this.repository = repository;
    }

    public async Task<UserProfile> Handle(GetUserProfile request, CancellationToken cancellationToken)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        var user = await repository.GetUserById(currentUser.Id, cancellationToken) ??
                   throw new NotFoundException($"User with id {currentUser.Id} not found");

        return new UserProfile(user.DisplayName, user.Email);
    }
}