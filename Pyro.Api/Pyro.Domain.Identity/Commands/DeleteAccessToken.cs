// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Identity.Commands;

public record DeleteAccessToken(string Name) : IRequest;

public class DeleteAccessTokenValidator : AbstractValidator<DeleteAccessToken>
{
    public DeleteAccessTokenValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);
    }
}

public class DeleteAccessTokenHandler : IRequestHandler<DeleteAccessToken>
{
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IUserRepository userRepository;

    public DeleteAccessTokenHandler(ICurrentUserProvider currentUserProvider, IUserRepository userRepository)
    {
        this.currentUserProvider = currentUserProvider;
        this.userRepository = userRepository;
    }

    public async Task Handle(DeleteAccessToken request, CancellationToken cancellationToken = default)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        var user = await userRepository.GetUserById(currentUser.Id, cancellationToken) ??
                   throw new NotFoundException($"User (Id: {currentUser.Id}) not found");

        user.DeleteAccessToken(request.Name);
    }
}