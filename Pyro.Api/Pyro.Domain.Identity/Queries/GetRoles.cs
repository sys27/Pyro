// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;
using Pyro.Domain.Identity.Models;

namespace Pyro.Domain.Identity.Queries;

public record GetRoles : IRequest<IEnumerable<Role>>;

public class GetRolesHandler : IRequestHandler<GetRoles, IEnumerable<Role>>
{
    private readonly IUserRepository repository;

    public GetRolesHandler(IUserRepository repository)
        => this.repository = repository;

    public async Task<IEnumerable<Role>> Handle(GetRoles request, CancellationToken cancellationToken)
    {
        var roles = await repository.GetRolesAsync(cancellationToken);

        return roles;
    }
}