// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Options;

namespace Pyro.Domain.Shared.Email;

public class EmailServiceOptions : IValidateOptions<EmailServiceOptions>
{
    public const string Name = "EmailService";

    public EmailProviderKind Provider { get; set; } = EmailProviderKind.Console;

    public Uri? Host { get; set; }

    public string Domain { get; set; } = string.Empty;

    public string Login { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public ValidateOptionsResult Validate(string? name, EmailServiceOptions options)
    {
        if (Provider == EmailProviderKind.Console)
            return ValidateOptionsResult.Success;

        var failures = new List<string>();

        if (options.Host is null)
            failures.Add("Host is required.");

        if (string.IsNullOrWhiteSpace(options.Domain))
            failures.Add("Domain is required.");

        if (string.IsNullOrWhiteSpace(options.Login))
            failures.Add("Login is required.");

        if (string.IsNullOrWhiteSpace(options.Password))
            failures.Add("Password is required.");

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }
}

public enum EmailProviderKind
{
    Console,
    Smtp,
}