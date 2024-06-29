// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Identity.Models;

namespace Pyro.Domain.Identity.Commands;

public record CreateUser(
    string Login,
    string Password,
    IEnumerable<string> Roles) : IRequest;

public class CreateUserValidator : AbstractValidator<CreateUser>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);

        RuleFor(x => x.Roles)
            .NotEmpty();
    }
}

public class CreateUserHandler : IRequestHandler<CreateUser>
{
    private readonly IUserRepository repository;
    private readonly PasswordService passwordService;

    public CreateUserHandler(IUserRepository repository, PasswordService passwordService)
    {
        this.repository = repository;
        this.passwordService = passwordService;
    }

    public async Task Handle(CreateUser request, CancellationToken cancellationToken)
    {
        var (password, salt) = passwordService.GeneratePasswordHash(request.Password);
        var user = User.Create(request.Login, password, salt);

        var allRoles = await repository.GetRolesAsync(cancellationToken);
        var invalidRoles = request.Roles.Except(allRoles.Select(x => x.Name)).ToList();
        if (invalidRoles.Count > 0)
            throw new InvalidOperationException($"Invalid roles provided: {string.Join(", ", invalidRoles)}.");

        var rolesToAdd = allRoles.Where(x => request.Roles.Contains(x.Name)).ToList();
        foreach (var role in rolesToAdd)
            user.AddRole(role);

        await repository.AddUser(user, cancellationToken);
    }
}