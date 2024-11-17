// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pyro.Domain.Shared.Email;

namespace Pyro.Domain.Identity.Commands;

public record NotifyExpiringPasswords : IRequest;

public class NotifyExpiringPasswordsHandler : IRequestHandler<NotifyExpiringPasswords>
{
    private readonly ILogger<NotifyExpiringPasswordsHandler> logger;
    private readonly IUserRepository repository;
    private readonly IEmailService emailService;
    private readonly EmailServiceOptions emailServiceOptions;

    public NotifyExpiringPasswordsHandler(
        ILogger<NotifyExpiringPasswordsHandler> logger,
        IUserRepository repository,
        IEmailService emailService,
        IOptions<EmailServiceOptions> emailServiceOptions)
    {
        this.logger = logger;
        this.repository = repository;
        this.emailService = emailService;
        this.emailServiceOptions = emailServiceOptions.Value;
    }

    public async Task Handle(NotifyExpiringPasswords request, CancellationToken cancellationToken = default)
    {
        var count = 0;
        var expiringUsers = repository.GetUsersWithExpiringPasswords().WithCancellation(cancellationToken);
        await foreach (var user in expiringUsers)
        {
            var body = $"""
                        Hello {user.Profile.Name},

                        Your password is about to expire. The expiration date is {user.PasswordExpiresAt:yyyy-MM-dd}.
                        Please change your password to avoid any issues.

                        Thank you, Pyro.
                        """;
            var email = new EmailMessage(
                new EmailAddress(user.Profile.Name, user.Login),
                new EmailAddress("No Reply", $"no-reply@{emailServiceOptions.Domain}"),
                "Password expiration",
                body);

            await emailService.SendEmail(email, cancellationToken);

            logger.LogDebug("Notified '{User}' about expiring password", user.Login);
            count++;
        }

        if (count > 0)
            logger.LogInformation("Notified {Count} users about expiring passwords", count);
    }
}