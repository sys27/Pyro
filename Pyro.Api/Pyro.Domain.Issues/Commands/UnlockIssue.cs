// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Issues.Commands;

public record UnlockIssue(string RepositoryName, int IssueNumber) : IRequest;

public class UnlockIssueValidator : AbstractValidator<UnlockIssue>
{
    public UnlockIssueValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.IssueNumber)
            .GreaterThan(0);
    }
}

public class UnlockIssueHandler : IRequestHandler<UnlockIssue>
{
    private readonly IIssueRepository issueRepository;

    public UnlockIssueHandler(IIssueRepository issueRepository)
        => this.issueRepository = issueRepository;

    public async Task Handle(UnlockIssue request, CancellationToken cancellationToken)
    {
        var issue = await issueRepository.GetIssue(request.RepositoryName, request.IssueNumber, cancellationToken) ??
                    throw new NotFoundException($"Issue ('{request.RepositoryName}' #{request.IssueNumber}) not found");

        issue.Unlock();
    }
}