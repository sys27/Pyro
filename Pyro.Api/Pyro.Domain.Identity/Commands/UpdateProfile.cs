// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared.CurrentUserProvider;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Identity.Commands;

public record UpdateProfile(string Name, string? Status) : IRequest;

public class UpdateProfileValidator : AbstractValidator<UpdateProfile>
{
    public UpdateProfileValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Status)
            .MaximumLength(200);
    }
}

public class UpdateProfileHandler : IRequestHandler<UpdateProfile>
{
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IUserRepository repository;

    public UpdateProfileHandler(
        ICurrentUserProvider currentUserProvider,
        IUserRepository repository)
    {
        this.currentUserProvider = currentUserProvider;
        this.repository = repository;
    }

    public async Task Handle(UpdateProfile request, CancellationToken cancellationToken)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        var user = await repository.GetUserById(currentUser.Id, cancellationToken) ??
                   throw new NotFoundException($"User with id {currentUser.Id} is not found");

        user.Profile.Name = request.Name;
        user.Profile.Status = request.Status;
    }
}