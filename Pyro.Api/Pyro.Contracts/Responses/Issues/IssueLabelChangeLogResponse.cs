// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Contracts.Responses.Issues;

public record IssueLabelChangeLogResponse(
    Guid Id,
    UserResponse Author,
    DateTimeOffset CreatedAt,
    LabelResponse? OldLabel,
    LabelResponse? NewLabel) : IssueChangeLogResponse(Id, Author, CreatedAt);