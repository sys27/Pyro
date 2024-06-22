// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Core.Models;

namespace Pyro.Domain.Identity.Models;

public record UserCreated(Guid UserId, string Email) : IDomainEvent;