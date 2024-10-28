// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Buffers;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using SmtpServer;
using SmtpServer.Authentication;
using SmtpServer.ComponentModel;
using SmtpServer.Protocol;
using SmtpServer.Storage;

namespace Pyro.ApiTests.Clients;

internal sealed partial class Smtp : IMessageStore
{
    private readonly SmtpServer.SmtpServer smtpServer;
    private readonly BlockingCollection<Message> messages;

    public Smtp()
    {
        var options = new SmtpServerOptionsBuilder()
            .ServerName("localhost")
            .Endpoint(builder => builder
                .Port(25, false)
                .AllowUnsecureAuthentication())
            .Build();

        var serviceProvider = new ServiceProvider();
        serviceProvider.Add(this);
        serviceProvider.Add(MailboxFilter.Default);
        serviceProvider.Add(UserAuthenticator.Default);

        smtpServer = new SmtpServer.SmtpServer(options, serviceProvider);
        messages = [];
    }

    public void Start()
        => Task.Run(() => smtpServer.StartAsync(CancellationToken.None))
            .ContinueWith(
                task => Console.WriteLine(task.Exception),
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnFaulted,
                TaskScheduler.Default);

    public void Stop()
        => smtpServer.Shutdown();

    public async Task<SmtpResponse> SaveAsync(
        ISessionContext context,
        IMessageTransaction transaction,
        ReadOnlySequence<byte> buffer,
        CancellationToken cancellationToken)
    {
        await using var stream = new MemoryStream();

        var position = buffer.GetPosition(0);
        while (buffer.TryGet(ref position, out var memory))
            await stream.WriteAsync(memory, cancellationToken);

        stream.Position = 0;

        var message = await MimeKit.MimeMessage.LoadAsync(stream, cancellationToken);
        messages.Add(
            new Message
            {
                From = message.From.Mailboxes.First().Address,
                To = message.To.Mailboxes.First().Address,
                Body = message.HtmlBody,
            },
            cancellationToken);

        return SmtpResponse.Ok;
    }

    public Message? WaitForMessage(Func<Message, bool> condition, CancellationToken cancellationToken = default)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(1));
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeout.Token);

        foreach (var message in messages.GetConsumingEnumerable(linked.Token))
            if (condition(message))
                return message;

        return null;
    }

    public partial class Message
    {
        public required string From { get; set; }

        public required string To { get; set; }

        public required string Body { get; set; }

        [GeneratedRegex("token=(.*)\"")]
        private static partial Regex TokenRegex();

        public string GetToken()
            => Uri.UnescapeDataString(TokenRegex().Match(Body).Groups[1].Value);
    }
}