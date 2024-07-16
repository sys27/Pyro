// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.SignalR;

namespace Pyro.Services;

public class PyroHub : Hub<IPyroHubClient>
{
}