// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Pyro.Domain.Issues.Commands;

public record DeleteIssue(string RepositoryName, int IssueNumber) : IRequest;

public class DeleteIssueValidator : AbstractValidator<DeleteIssue>
{
    public DeleteIssueValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.IssueNumber)
            .GreaterThan(0);
    }
}

public class DeleteIssueHandler : IRequestHandler<DeleteIssue>
{
    private readonly IIssueRepository repository;
    private readonly ILogger<DeleteIssueHandler> logger;

    public DeleteIssueHandler(IIssueRepository repository, ILogger<DeleteIssueHandler> logger)
    {
        this.repository = repository;
        this.logger = logger;
    }

    public async Task Handle(DeleteIssue request, CancellationToken cancellationToken)
    {
        var issue = await repository.GetIssue(request.RepositoryName, request.IssueNumber, cancellationToken);
        if (issue is null)
        {
            logger.LogWarning("Issue #{Number} not found in repository '{RepositoryName}'", request.IssueNumber, request.RepositoryName);
            return;
        }

        repository.DeleteIssue(issue);
    }
}