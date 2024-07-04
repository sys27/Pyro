// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Core.Exceptions;
using Pyro.Domain.Core.Models;

namespace Pyro.Domain.UserProfiles;

public class UserProfile
{
    private readonly string? email;
    private string? name;
    private string? status;

    public Guid Id { get; init; }

    public string? Email
    {
        get => email;
        init
        {
            if (value is not null)
            {
                if (value.Length > 150)
                    throw new DomainValidationException("Email cannot be longer than 150 characters.");

                if (!Regexes.GetEmailRegex().IsMatch(value))
                    throw new DomainValidationException("Email is not valid.");
            }

            email = value;
        }
    }

    public string? Name
    {
        get => name;
        set
        {
            if (value?.Length > 50)
                throw new DomainValidationException("Name cannot be longer than 50 characters.");

            name = value;
        }
    }

    public string? Status
    {
        get => status;
        set
        {
            if (value?.Length > 150)
                throw new DomainValidationException("Status cannot be longer than 150 characters.");

            status = value;
        }
    }

    public UserAvatar? Avatar { get; init; }
}