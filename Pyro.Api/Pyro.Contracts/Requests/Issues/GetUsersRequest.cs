// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Contracts.Requests.Issues;

public record GetUsersRequest(string Name, int Size, Guid? Before, Guid? After)
    : PageRequest<Guid?>(Size, Before, After);