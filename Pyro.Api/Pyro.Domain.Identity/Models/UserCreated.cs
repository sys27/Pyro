// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Shared.Entities;
using Pyro.Domain.Shared.Messaging;

namespace Pyro.Domain.Identity.Models;

public record UserCreated(Guid UserId, string Name) : IDomainEvent;

public record UserCreatedIntegrationEvent(Guid UserId) : IntegrationEvent;