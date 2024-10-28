// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Options;

namespace Pyro;

public class ServiceOptions : IValidateOptions<ServiceOptions>
{
    public const string Name = "Service";

    public Uri? PublicUrl { get; set; }

    public ValidateOptionsResult Validate(string? name, ServiceOptions options)
    {
        if (options.PublicUrl is null)
            return ValidateOptionsResult.Fail("PublicUrl must be provided.");

        return ValidateOptionsResult.Success;
    }
}