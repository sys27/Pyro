// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Identity.Models;
using Pyro.Domain.Shared;

namespace Pyro.Domain.Identity.Queries;

public record GetUsers(int Size, string? Before, string? After) : IRequest<IReadOnlyList<User>>, IPageQuery<string>;

public class GetUsersValidator : AbstractValidator<GetUsers>
{
    public GetUsersValidator()
    {
        Include(new PageQueryValidator<string>());
    }
}

public class GetUsersHandler : IRequestHandler<GetUsers, IReadOnlyList<User>>
{
    private readonly IUserRepository repository;

    public GetUsersHandler(IUserRepository repository)
        => this.repository = repository;

    public async Task<IReadOnlyList<User>> Handle(GetUsers request, CancellationToken cancellationToken = default)
    {
        var users = await repository.GetUsers(request, cancellationToken);

        return users;
    }
}