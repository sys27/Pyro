// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Domain.Core.Models;

public record CurrentUser(
    Guid Id,
    string Email,
    IEnumerable<string> Roles,
    IEnumerable<string> Permissions);