// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;

namespace Pyro.Contracts.Responses.Issues;

[JsonDerivedType(typeof(IssueAssigneeChangeLogResponse), 1)]
[JsonDerivedType(typeof(IssueLabelChangeLogResponse), 2)]
[JsonDerivedType(typeof(IssueLockChangeLogResponse), 3)]
[JsonDerivedType(typeof(IssueStatusChangeLogResponse), 4)]
[JsonDerivedType(typeof(IssueTitleChangeLogResponse), 5)]
public abstract record IssueChangeLogResponse(
    Guid Id,
    UserResponse Author,
    DateTimeOffset CreatedAt);