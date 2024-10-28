// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Domain.Shared.Email;

public record EmailMessage(
    string From,
    string To,
    string Subject,
    string Body);