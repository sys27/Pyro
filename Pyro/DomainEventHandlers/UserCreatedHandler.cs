// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;
using Pyro.Domain.Identity.Models;
using Pyro.Domain.UserProfiles;

namespace Pyro.DomainEventHandlers;

public class UserCreatedHandler : INotificationHandler<UserCreated>
{
    private readonly IMediator mediator;

    public UserCreatedHandler(IMediator mediator)
        => this.mediator = mediator;

    public async Task Handle(UserCreated notification, CancellationToken cancellationToken)
    {
        var command = new CreateUserProfile(notification.UserId);
        await mediator.Send(command, cancellationToken);
    }
}