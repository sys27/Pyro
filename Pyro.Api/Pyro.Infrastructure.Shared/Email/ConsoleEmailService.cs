// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using Pyro.Domain.Shared.Email;

namespace Pyro.Infrastructure.Shared.Email;

internal class ConsoleEmailService : IEmailService
{
    private readonly ILogger<ConsoleEmailService> logger;

    public ConsoleEmailService(ILogger<ConsoleEmailService> logger)
        => this.logger = logger;

    public Task SendEmail(EmailMessage emailMessage, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Sending email message. From: {From}, To: {To}, Subject: {Subject}, Body: {Body}",
            emailMessage.From,
            emailMessage.To,
            emailMessage.Subject,
            emailMessage.Body);

        return Task.CompletedTask;
    }
}