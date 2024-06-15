// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;
using Pyro.Domain.Identity.Models;

namespace Pyro.Domain.Identity.Queries;

public record GetPermissions : IRequest<IReadOnlyList<Permission>>;

public class GetPermissionsHandler : IRequestHandler<GetPermissions, IReadOnlyList<Permission>>
{
    private readonly IUserRepository repository;

    public GetPermissionsHandler(IUserRepository repository)
    {
        this.repository = repository;
    }

    public async Task<IReadOnlyList<Permission>> Handle(GetPermissions request, CancellationToken cancellationToken)
    {
        var permissions = await repository.GetPermissionsAsync(cancellationToken);

        return permissions;
    }
}