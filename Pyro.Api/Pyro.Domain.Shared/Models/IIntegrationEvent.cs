// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;

namespace Pyro.Domain.Shared.Models;

public interface IIntegrationEvent : INotification
{
    Guid MessageId { get; }
}