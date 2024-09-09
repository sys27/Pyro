// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Issues.Queries;

public record GetIssueChangeLogs(string RepositoryName, int IssueNumber) : IRequest<IReadOnlyList<IssueChangeLog>>;

public class GetIssueChangeLogsValidator : AbstractValidator<GetIssueChangeLogs>
{
    public GetIssueChangeLogsValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.IssueNumber)
            .GreaterThan(0);
    }
}

public class GetIssueChangeLogsHandler : IRequestHandler<GetIssueChangeLogs, IReadOnlyList<IssueChangeLog>>
{
    private readonly IIssueRepository issueRepository;

    public GetIssueChangeLogsHandler(IIssueRepository issueRepository)
        => this.issueRepository = issueRepository;

    public async Task<IReadOnlyList<IssueChangeLog>> Handle(
        GetIssueChangeLogs request,
        CancellationToken cancellationToken = default)
    {
        var issue = await issueRepository.GetIssue(request.RepositoryName, request.IssueNumber, cancellationToken) ??
                    throw new NotFoundException($"Issue ('{request.RepositoryName}' #{request.IssueNumber}) not found");

        return issue.ChangeLogs;
    }
}