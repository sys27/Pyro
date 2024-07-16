// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.SignalR;
using Pyro.Domain.Shared;

namespace Pyro.Services;

internal class NotificationService : INotificationService
{
    private readonly IHubContext<PyroHub, IPyroHubClient> hubContext;

    public NotificationService(IHubContext<PyroHub, IPyroHubClient> hubContext)
        => this.hubContext = hubContext;

    public async Task RepositoryInitialized(string name)
        => await hubContext.Clients.All.RepositoryInitialized(name);
}