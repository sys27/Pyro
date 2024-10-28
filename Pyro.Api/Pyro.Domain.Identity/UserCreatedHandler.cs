// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;
using Pyro.Domain.Identity.Models;
using Pyro.Domain.Shared.Messaging;

namespace Pyro.Domain.Identity;

public class UserCreatedHandler : INotificationHandler<UserCreated>
{
    private readonly IBus bus;

    public UserCreatedHandler(IBus bus)
        => this.bus = bus;

    public async Task Handle(UserCreated notification, CancellationToken cancellationToken = default)
    {
        var integrationEvent = new UserCreatedIntegrationEvent(notification.UserId);
        await bus.Publish(integrationEvent, cancellationToken);
    }
}