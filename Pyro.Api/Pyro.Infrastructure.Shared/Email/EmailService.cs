// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Pyro.Domain.Shared.Email;

namespace Pyro.Infrastructure.Shared.Email;

internal sealed class EmailService : IEmailService, IAsyncDisposable, IDisposable
{
    private readonly SmtpClient smtpClient;
    private readonly EmailServiceOptions options;

    public EmailService(IOptions<EmailServiceOptions> options)
    {
        this.smtpClient = new SmtpClient();
        this.options = options.Value;
    }

    public async ValueTask DisposeAsync()
    {
        if (smtpClient.IsConnected)
            await smtpClient.DisconnectAsync(true);

        smtpClient.Dispose();
    }

    public void Dispose()
    {
        if (smtpClient.IsConnected)
            smtpClient.Disconnect(true);

        smtpClient.Dispose();
    }

    private async Task Connect(CancellationToken cancellationToken)
    {
        if (smtpClient.IsConnected)
            return;

        await smtpClient.ConnectAsync(options.Host!, cancellationToken);
        await smtpClient.AuthenticateAsync(options.Login, options.Password, cancellationToken);
    }

    public async Task SendEmail(EmailMessage emailMessage, CancellationToken cancellationToken = default)
    {
        await Connect(cancellationToken);

        using var message = new MimeMessage();

        message.From.Add(new MailboxAddress(emailMessage.From.Name, emailMessage.From.Address));
        message.To.Add(new MailboxAddress(emailMessage.To.Name, emailMessage.To.Address));
        message.Subject = emailMessage.Subject;
        message.Body = new TextPart(TextFormat.Html)
        {
            Text = emailMessage.Body,
        };

        await smtpClient.SendAsync(message, cancellationToken);
    }
}