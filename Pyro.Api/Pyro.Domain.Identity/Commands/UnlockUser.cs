// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Identity.Commands;

public record UnlockUser(string Login) : IRequest;

public class UnlockUserValidator : AbstractValidator<UnlockUser>
{
    public UnlockUserValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty()
            .MaximumLength(32);
    }
}

public class UnlockUserHandler : IRequestHandler<UnlockUser>
{
    private readonly IUserRepository repository;

    public UnlockUserHandler(IUserRepository repository)
        => this.repository = repository;

    public async Task Handle(UnlockUser request, CancellationToken cancellationToken = default)
    {
        var user = await repository.GetUserByLogin(request.Login, cancellationToken) ??
                   throw new NotFoundException($"User (Login: {request.Login}) not found");

        user.Unlock();
    }
}