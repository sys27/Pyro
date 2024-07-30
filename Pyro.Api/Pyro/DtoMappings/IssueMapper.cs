// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Contracts.Responses.Issues;
using Pyro.Domain.Issues;
using Riok.Mapperly.Abstractions;

namespace Pyro.DtoMappings;

[Mapper]
public static partial class IssueMapper
{
    [MapperIgnoreSource(nameof(Issue.Id))]
    [MapperIgnoreSource(nameof(Issue.Repository))]
    [MapperIgnoreSource(nameof(Issue.Comments))]
    public static partial IssueResponse ToResponse(this Issue gitRepository);

    public static partial IReadOnlyList<IssueResponse> ToResponse(this IReadOnlyList<Issue> gitRepository);

    [MapperIgnoreSource(nameof(IssueComment.Issue))]
    public static partial IssueCommentResponse ToResponse(this IssueComment gitRepository);

    public static partial IReadOnlyList<IssueCommentResponse> ToResponse(this IReadOnlyList<IssueComment> gitRepository);

    public static partial UserResponse ToResponse(this User gitRepository);

    public static partial IReadOnlyList<UserResponse> ToResponse(this IReadOnlyList<User> gitRepository);
}