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
    [MapperIgnoreSource(nameof(Issue.DomainEvents))]
    [MapperIgnoreSource(nameof(Issue.RepositoryId))]
    [MapperIgnoreSource(nameof(Issue.Comments))]
    [MapperIgnoreSource(nameof(Issue.ChangeLogs))]
    public static partial IssueResponse ToResponse(this Issue gitRepository);

    public static partial IReadOnlyList<IssueResponse> ToResponse(this IReadOnlyList<Issue> gitRepository);

    [MapperIgnoreSource(nameof(IssueComment.Issue))]
    public static partial IssueCommentResponse ToResponse(this IssueComment gitRepository);

    public static partial IReadOnlyList<IssueCommentResponse> ToResponse(this IReadOnlyList<IssueComment> gitRepository);

    public static partial UserResponse ToResponse(this User gitRepository);

    public static partial IReadOnlyList<UserResponse> ToResponse(this IReadOnlyList<User> gitRepository);

    public static partial GetIssues ToGetIssues(this GetIssuesRequest request);

    [UserMapping(Default = true)]
    public static ColorResponse ToResponse(this int color)
    {
        var r = (byte)((color & 0xFF0000) >> 16);
        var g = (byte)((color & 0x00FF00) >> 8);
        var b = (byte)(color & 0x0000FF);

        return new ColorResponse(r, g, b);
    }

    public static partial LabelResponse ToResponse(this Label request);

    [MapperIgnoreSource(nameof(IssueStatus.Repository))]
    [MapperIgnoreSource(nameof(IssueStatus.FromTransitions))]
    [MapperIgnoreSource(nameof(IssueStatus.ToTransitions))]
    public static partial IssueStatusResponse ToResponse(this IssueStatus request);

    public static partial IReadOnlyList<IssueStatusResponse> ToResponse(this IReadOnlyList<IssueStatus> request);

    public static partial IssueStatusTransitionResponse ToResponse(this IssueStatusTransition request);

    public static partial IReadOnlyList<IssueStatusTransitionResponse> ToResponse(
        this IReadOnlyList<IssueStatusTransition> request);

    [MapperIgnoreSource(nameof(IssueChangeLog.Issue))]
    [MapDerivedType<IssueAssigneeChangeLog, IssueAssigneeChangeLogResponse>]
    [MapDerivedType<IssueLabelChangeLog, IssueLabelChangeLogResponse>]
    [MapDerivedType<IssueLockChangeLog, IssueLockChangeLogResponse>]
    [MapDerivedType<IssueStatusChangeLog, IssueStatusChangeLogResponse>]
    [MapDerivedType<IssueTitleChangeLog, IssueTitleChangeLogResponse>]
    public static partial IssueChangeLogResponse ToResponse(this IssueChangeLog request);

    public static partial IReadOnlyList<IssueChangeLogResponse> ToResponse(
        this IReadOnlyList<IssueChangeLog> request);

    [MapperIgnoreSource(nameof(IssueChangeLog.Issue))]
    public static partial IssueAssigneeChangeLogResponse ToResponse(this IssueAssigneeChangeLog request);

    [MapperIgnoreSource(nameof(IssueChangeLog.Issue))]
    public static partial IssueLabelChangeLogResponse ToResponse(this IssueLabelChangeLog request);

    [MapperIgnoreSource(nameof(IssueChangeLog.Issue))]
    public static partial IssueLockChangeLogResponse ToResponse(this IssueLockChangeLog request);

    [MapperIgnoreSource(nameof(IssueChangeLog.Issue))]
    public static partial IssueStatusChangeLogResponse ToResponse(this IssueStatusChangeLog request);

    [MapperIgnoreSource(nameof(IssueChangeLog.Issue))]
    public static partial IssueTitleChangeLogResponse ToResponse(this IssueTitleChangeLog request);
}