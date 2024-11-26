// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Text.Encodings.Web;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;
using Pyro.Domain.Shared;
using Pyro.Domain.Shared.Email;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Identity.Commands;

public record ForgotPassword(string Login) : IRequest;

public class ForgotPasswordValidator : AbstractValidator<ForgotPassword>
{
    public ForgotPasswordValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty()
            .MaximumLength(32)
            .Matches(@"^[a-zA-Z0-9_\-]*$");
    }
}

public class ForgotPasswordHandler : IRequestHandler<ForgotPassword>
{
    private readonly IUserRepository repository;
    private readonly IPasswordService passwordService;
    private readonly IEmailService emailService;
    private readonly EmailServiceOptions emailServiceOptions;
    private readonly ServiceOptions serviceOptions;
    private readonly UrlEncoder urlEncoder;

    public ForgotPasswordHandler(
        IUserRepository repository,
        IPasswordService passwordService,
        IEmailService emailService,
        IOptions<EmailServiceOptions> emailServiceOptions,
        IOptions<ServiceOptions> serviceOptions,
        UrlEncoder urlEncoder)
    {
        this.repository = repository;
        this.passwordService = passwordService;
        this.emailService = emailService;
        this.emailServiceOptions = emailServiceOptions.Value;
        this.serviceOptions = serviceOptions.Value;
        this.urlEncoder = urlEncoder;
    }

    public async Task Handle(ForgotPassword request, CancellationToken cancellationToken = default)
    {
        var user = await repository.GetUserByLogin(request.Login, cancellationToken) ??
                   throw new NotFoundException($"User (Login: {request.Login}) not found");

        var oneTimePassword = passwordService.GeneratePasswordResetTokenFor(user);

        var inviteLink = new UriBuilder(serviceOptions.PublicUrl!)
            {
                Path = "/reset-password",
                Query = $"token={urlEncoder.Encode(oneTimePassword.Token)}",
            }
            .Uri
            .ToString();
        var body = $"""
                    Hello {user.DisplayName}!

                    You have requested to reset your password. Please use the following link to reset your password: <a href="{inviteLink}">Reset Password</a>. If you did not request to reset your password, please ignore this email.

                    Thank you, Pyro.
                    """;
        var message = new EmailMessage(
            new EmailAddress("No Reply", $"no-reply@{emailServiceOptions.Domain}"),
            new EmailAddress(user.DisplayName, user.Email),
            "Reset your password",
            body);
        await emailService.SendEmail(message, cancellationToken);
    }
}