// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;

namespace Pyro.Domain.UserProfiles;

public record CreateUserProfile(Guid UserId) : IRequest;

public class CreateUserProfileValidator : AbstractValidator<CreateUserProfile>
{
    public CreateUserProfileValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}

public class CreateUserProfileHandler : IRequestHandler<CreateUserProfile>
{
    private readonly IUserProfileRepository repository;

    public CreateUserProfileHandler(IUserProfileRepository repository)
        => this.repository = repository;

    public async Task Handle(CreateUserProfile request, CancellationToken cancellationToken = default)
    {
        var userProfile = new UserProfile
        {
            Id = request.UserId,
        };
        await repository.AddUserProfile(userProfile, cancellationToken);
    }
}