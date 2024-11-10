// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared.CurrentUserProvider;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Identity.Commands;

public record ChangePassword(string OldPassword, string NewPassword) : IRequest;

public class ChangePasswordValidator : AbstractValidator<ChangePassword>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.OldPassword)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8);
    }
}

public class ChangePasswordHandler : IRequestHandler<ChangePassword>
{
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IUserRepository userRepository;
    private readonly IPasswordService passwordService;

    public ChangePasswordHandler(
        ICurrentUserProvider currentUserProvider,
        IUserRepository userRepository,
        IPasswordService passwordService)
    {
        this.currentUserProvider = currentUserProvider;
        this.userRepository = userRepository;
        this.passwordService = passwordService;
    }

    public async Task Handle(ChangePassword request, CancellationToken cancellationToken)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        var user = await userRepository.GetUserById(currentUser.Id, cancellationToken) ??
                   throw new NotFoundException($"The user (Id: {currentUser.Id}) not found");

        user.ChangePassword(passwordService, request.OldPassword, request.NewPassword);
    }
}