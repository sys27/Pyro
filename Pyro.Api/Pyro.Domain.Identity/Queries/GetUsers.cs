// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;
using Pyro.Domain.Identity.Models;

namespace Pyro.Domain.Identity.Queries;

public record GetUsers : IRequest<IReadOnlyList<User>>;

public class GetUsersHandler : IRequestHandler<GetUsers, IReadOnlyList<User>>
{
    private readonly IUserRepository repository;

    public GetUsersHandler(IUserRepository repository)
        => this.repository = repository;

    public async Task<IReadOnlyList<User>> Handle(GetUsers request, CancellationToken cancellationToken)
    {
        var users = await repository.GetUsers(cancellationToken);

        return users;
    }
}