// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Pyro.Domain.Identity.Commands;

public record ActivateUser(string Token, string Password) : IRequest;

public class ActivateUserValidator : AbstractValidator<ActivateUser>
{
    public ActivateUserValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);
    }
}

public class ActivateUserHandler : IRequestHandler<ActivateUser>
{
    private readonly ILogger<ActivateUserHandler> logger;
    private readonly TimeProvider timeProvider;
    private readonly IUserRepository userRepository;
    private readonly IPasswordService passwordService;

    public ActivateUserHandler(
        ILogger<ActivateUserHandler> logger,
        TimeProvider timeProvider,
        IUserRepository userRepository,
        IPasswordService passwordService)
    {
        this.logger = logger;
        this.timeProvider = timeProvider;
        this.userRepository = userRepository;
        this.passwordService = passwordService;
    }

    public async Task Handle(ActivateUser request, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetUserByToken(request.Token, cancellationToken);
        var token = user?.GetOneTimePassword(request.Token);
        if (user is null || token is null)
        {
            logger.LogError("The token '{Token}' is invalid", request.Token);
            return;
        }

        var (passwordHash, salt) = passwordService.GeneratePasswordHash(request.Password);
        user.Activate(timeProvider, token, passwordHash, salt);

        logger.LogInformation("User '{Login}' activated", user.Login);
    }
}