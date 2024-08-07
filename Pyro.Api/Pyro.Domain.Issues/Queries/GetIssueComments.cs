// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;

namespace Pyro.Domain.Issues.Queries;

public record GetIssueComments(string RepositoryName, int IssueNumber) : IRequest<IReadOnlyList<IssueComment>>;

public class GetIssueCommentsValidator : AbstractValidator<GetIssueComments>
{
    public GetIssueCommentsValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.IssueNumber)
            .GreaterThan(0);
    }
}

public class GetIssueCommentsHandler : IRequestHandler<GetIssueComments, IReadOnlyList<IssueComment>>
{
    private readonly IIssueRepository repository;

    public GetIssueCommentsHandler(IIssueRepository repository)
        => this.repository = repository;

    public async Task<IReadOnlyList<IssueComment>> Handle(
        GetIssueComments request,
        CancellationToken cancellationToken = default)
    {
        var issue = await repository.GetIssue(request.RepositoryName, request.IssueNumber, cancellationToken);
        if (issue is null)
            return [];

        return issue.Comments;
    }
}