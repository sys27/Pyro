// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Contracts.Requests.Issues;
using Pyro.Contracts.Responses;
using Pyro.Contracts.Responses.Issues;
using Pyro.Domain.Issues;
using Pyro.Domain.Issues.Queries;
using Riok.Mapperly.Abstractions;

namespace Pyro.DtoMappings;

[Mapper]
public static partial class IssueMapper
{
    [MapperIgnoreSource(nameof(Issue.Repository))]
    [MapperIgnoreSource(nameof(Issue.Comments))]
    public static partial IssueResponse ToResponse(this Issue gitRepository);

    public static partial IReadOnlyList<IssueResponse> ToResponse(this IReadOnlyList<Issue> gitRepository);

    [MapperIgnoreSource(nameof(IssueComment.Issue))]
    public static partial IssueCommentResponse ToResponse(this IssueComment gitRepository);

    public static partial IReadOnlyList<IssueCommentResponse> ToResponse(this IReadOnlyList<IssueComment> gitRepository);

    public static partial UserResponse ToResponse(this User gitRepository);

    public static partial IReadOnlyList<UserResponse> ToResponse(this IReadOnlyList<User> gitRepository);

    [MapProperty(nameof(GetUsersRequest.Name), nameof(GetIssues.RepositoryName))]
    public static partial GetIssues ToGetIssues(this GetUsersRequest request);

    [UserMapping(Default = true)]
    public static ColorResponse ToResponse(this int color)
    {
        var r = (byte)((color & 0xFF0000) >> 16);
        var g = (byte)((color & 0x00FF00) >> 8);
        var b = (byte)(color & 0x0000FF);

        return new ColorResponse(r, g, b);
    }

    public static partial LabelResponse ToResponse(this Label request);
}