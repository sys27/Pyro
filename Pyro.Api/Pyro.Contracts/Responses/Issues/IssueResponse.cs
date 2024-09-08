// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Contracts.Responses.Issues;

public record IssueResponse(
    Guid Id,
    int IssueNumber,
    string Title,
    IssueStatusResponse Status,
    UserResponse Author,
    DateTimeOffset CreatedAt,
    UserResponse? Assignee,
    bool IsLocked,
    IReadOnlyList<LabelResponse> Labels);