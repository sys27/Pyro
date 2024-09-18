// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Identity.Commands;

public record LockUser(string Login) : IRequest;

public class LockUserValidator : AbstractValidator<LockUser>
{
    public LockUserValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty()
            .MaximumLength(32);
    }
}

public class LockUserHandler : IRequestHandler<LockUser>
{
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IUserRepository repository;

    public LockUserHandler(ICurrentUserProvider currentUserProvider, IUserRepository repository)
    {
        this.currentUserProvider = currentUserProvider;
        this.repository = repository;
    }

    public async Task Handle(LockUser request, CancellationToken cancellationToken = default)
    {
        var user = await repository.GetUserByLogin(request.Login, cancellationToken) ??
                   throw new NotFoundException($"User (Login: {request.Login}) not found");
        var currentUser = currentUserProvider.GetCurrentUser();
        if (currentUser.Id == user.Id)
            throw new DomainException("You cannot lock yourself.");

        user.Lock();
    }
}