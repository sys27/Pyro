// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;

namespace Pyro.Domain.Issues.Queries;

public record GetUsers : IRequest<IReadOnlyList<User>>;

public class GetUsersHandler : IRequestHandler<GetUsers, IReadOnlyList<User>>
{
    private readonly IGitRepositoryRepository repository;

    public GetUsersHandler(IGitRepositoryRepository repository)
        => this.repository = repository;

    public async Task<IReadOnlyList<User>> Handle(GetUsers request, CancellationToken cancellationToken)
    {
        var result = await repository.GetUsers(cancellationToken);

        return result;
    }
}