// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Identity.Models;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Identity.Commands;

public record UpdateUser(User User, IEnumerable<string> Roles) : IRequest<User>;

public class UpdateUserValidator : AbstractValidator<UpdateUser>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.User)
            .NotNull();

        RuleFor(x => x.Roles)
            .NotEmpty();
    }
}

public class UpdateUserHandler : IRequestHandler<UpdateUser, User>
{
    private readonly IUserRepository repository;

    public UpdateUserHandler(IUserRepository repository)
        => this.repository = repository;

    public async Task<User> Handle(UpdateUser request, CancellationToken cancellationToken = default)
    {
        var allRoles = await repository.GetRolesAsync(cancellationToken);
        var invalidRoles = request.Roles.Except(allRoles.Select(x => x.Name)).ToList();
        if (invalidRoles.Count > 0)
            throw new DomainException($"Invalid roles provided: {string.Join(", ", invalidRoles)}.");

        var user = request.User;
        user.ClearRoles();

        var rolesToAdd = allRoles.Where(x => request.Roles.Contains(x.Name)).ToList();
        foreach (var role in rolesToAdd)
            user.AddRole(role);

        return user;
    }
}