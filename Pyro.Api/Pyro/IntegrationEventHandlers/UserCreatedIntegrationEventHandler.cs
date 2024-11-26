// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Text.Encodings.Web;
using MediatR;
using Microsoft.Extensions.Options;
using Pyro.Domain.Identity;
using Pyro.Domain.Identity.Models;
using Pyro.Domain.Shared;
using Pyro.Domain.Shared.Email;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.IntegrationEventHandlers;

public class UserCreatedIntegrationEventHandler : INotificationHandler<UserCreatedIntegrationEvent>
{
    private readonly ILogger<UserCreatedIntegrationEventHandler> logger;
    private readonly UrlEncoder urlEncoder;
    private readonly ServiceOptions serviceOptions;
    private readonly IEmailService emailService;
    private readonly EmailServiceOptions emailServiceOptions;
    private readonly IUserRepository userRepository;

    public UserCreatedIntegrationEventHandler(
        ILogger<UserCreatedIntegrationEventHandler> logger,
        IOptions<ServiceOptions> serviceOptions,
        UrlEncoder urlEncoder,
        IEmailService emailService,
        IOptions<EmailServiceOptions> emailServiceOptions,
        IUserRepository userRepository)
    {
        this.logger = logger;
        this.urlEncoder = urlEncoder;
        this.serviceOptions = serviceOptions.Value;
        this.emailService = emailService;
        this.emailServiceOptions = emailServiceOptions.Value;
        this.userRepository = userRepository;
    }

    public async Task Handle(UserCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserById(notification.UserId, cancellationToken) ??
                   throw new NotFoundException($"The user (Id: {notification.UserId}) was not found.");
        var otp = user.GetRegistrationOneTimePassword() ??
                  throw new DomainException($"The user (Id: {notification.UserId}) has no registration one-time password.");

        var inviteLink = new UriBuilder(serviceOptions.PublicUrl!)
            {
                Path = "/activate",
                Query = $"token={urlEncoder.Encode(otp.Token)}",
            }
            .Uri
            .ToString();
        var body = $"""
                    Welcome to Pyro!

                    Your account has been created successfully. To finish registration, please visit the following link: <a href="{inviteLink}">Finish registration</a>.
                    """;
        var message = new EmailMessage(
            new EmailAddress("No Reply", $"no-reply@{emailServiceOptions.Domain}"),
            new EmailAddress(user.DisplayName, user.Email),
            "Welcome to Pyro",
            body);
        await emailService.SendEmail(message, cancellationToken);

        logger.LogInformation("Sent registration email to '{Login}'.", user.Login);
    }
}