// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Shared.Entities;

namespace Pyro.Domain.Identity.Models;

public class UserProfile : Entity
{
    public required string Name { get; set; }

    public string? Status { get; set; }
}