// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Dtos.Responses;

public record CommitInfoResponse(
    string Hash,
    CommitUserResponse Author,
    string Message,
    DateTimeOffset Date);