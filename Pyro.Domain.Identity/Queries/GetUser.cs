// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Identity.Models;

namespace Pyro.Domain.Identity.Queries;

public record GetUser(string Login) : IRequest<User?>;

public class GetUserValidator : AbstractValidator<GetUser>
{
    public GetUserValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty()
            .MaximumLength(50)
            .EmailAddress();
    }
}

public class GetUserHandler : IRequestHandler<GetUser, User?>
{
    private readonly IUserRepository repository;

    public GetUserHandler(IUserRepository repository)
    {
        this.repository = repository;
    }

    public async Task<User?> Handle(GetUser request, CancellationToken cancellationToken)
    {
        var user = await repository.GetUserByLogin(request.Login, cancellationToken);

        return user;
    }
}