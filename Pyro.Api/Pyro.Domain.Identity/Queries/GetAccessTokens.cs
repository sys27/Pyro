// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;
using Pyro.Domain.Core;
using Pyro.Domain.Core.Exceptions;
using Pyro.Domain.Identity.Models;

namespace Pyro.Domain.Identity.Queries;

public record GetAccessTokens : IRequest<IReadOnlyList<AccessToken>>;

public class GetAccessTokenHandler : IRequestHandler<GetAccessTokens, IReadOnlyList<AccessToken>>
{
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IUserRepository repository;

    public GetAccessTokenHandler(ICurrentUserProvider currentUserProvider, IUserRepository repository)
    {
        this.currentUserProvider = currentUserProvider;
        this.repository = repository;
    }

    public async Task<IReadOnlyList<AccessToken>> Handle(GetAccessTokens request, CancellationToken cancellationToken)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        var user = await repository.GetUserById(currentUser.Id, cancellationToken);
        if (user is null)
            throw new DomainException("User not found");

        return user.AccessTokens
            .OrderBy(x => x.Name)
            .ToList();
    }
}