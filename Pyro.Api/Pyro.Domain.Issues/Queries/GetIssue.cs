// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;

namespace Pyro.Domain.Issues.Queries;

public record GetIssue(string RepositoryName, int IssueNumber) : IRequest<Issue?>;

public class GetIssueValidator : AbstractValidator<GetIssue>
{
    public GetIssueValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.IssueNumber)
            .GreaterThan(0);
    }
}

public class GetIssueHandler : IRequestHandler<GetIssue, Issue?>
{
    private readonly IIssueRepository repository;

    public GetIssueHandler(IIssueRepository repository)
        => this.repository = repository;

    public async Task<Issue?> Handle(GetIssue request, CancellationToken cancellationToken)
    {
        var issue = await repository.GetIssue(request.RepositoryName, request.IssueNumber, cancellationToken);

        return issue;
    }
}