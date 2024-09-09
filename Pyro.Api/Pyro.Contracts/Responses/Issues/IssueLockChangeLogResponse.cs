// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Contracts.Responses.Issues;

public record IssueLockChangeLogResponse(
    Guid Id,
    UserResponse Author,
    DateTimeOffset CreatedAt,
    bool OldValue,
    bool NewValue) : IssueChangeLogResponse(Id, Author, CreatedAt);