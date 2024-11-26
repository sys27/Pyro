// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Pyro.Domain.Identity.Commands;

public record ResetPassword(string Token, string Password) : IRequest;

public class ResetPasswordValidator : AbstractValidator<ResetPassword>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);
    }
}

public class ResetPasswordHandler : IRequestHandler<ResetPassword>
{
    private readonly ILogger<ResetPasswordHandler> logger;
    private readonly IUserRepository repository;
    private readonly TimeProvider timeProvider;
    private readonly IPasswordService passwordService;

    public ResetPasswordHandler(
        ILogger<ResetPasswordHandler> logger,
        IUserRepository repository,
        TimeProvider timeProvider,
        IPasswordService passwordService)
    {
        this.logger = logger;
        this.repository = repository;
        this.timeProvider = timeProvider;
        this.passwordService = passwordService;
    }

    public async Task Handle(ResetPassword request, CancellationToken cancellationToken = default)
    {
        var user = await repository.GetUserByToken(request.Token, cancellationToken);
        var otp = user?.GetOneTimePassword(request.Token);
        if (user is null || otp is null)
        {
            logger.LogError("The token '{Token}' is invalid", request.Token);
            return;
        }

        user.ResetPassword(timeProvider, passwordService, otp, request.Password);

        logger.LogInformation("User '{Login}' password reset", user.Login);
    }
}