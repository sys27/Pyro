// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Issues.Commands;

public record UpdateIssue(string RepositoryName, int IssueNumber, string Title, Guid? AssigneeId) : IRequest<Issue>;

public class UpdateIssueValidator : AbstractValidator<UpdateIssue>
{
    public UpdateIssueValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);
    }
}

public class UpdateIssueHandler : IRequestHandler<UpdateIssue, Issue>
{
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IIssueRepository issueRepository;

    public UpdateIssueHandler(ICurrentUserProvider currentUserProvider, IIssueRepository issueRepository)
    {
        this.currentUserProvider = currentUserProvider;
        this.issueRepository = issueRepository;
    }

    public async Task<Issue> Handle(UpdateIssue request, CancellationToken cancellationToken = default)
    {
        var issue = await issueRepository.GetIssue(request.RepositoryName, request.IssueNumber, cancellationToken) ??
                    throw new NotFoundException($"Issue ('{request.RepositoryName}' #{request.IssueNumber}) not found");

        var assignee = request.AssigneeId is not null
            ? await issueRepository.GetUser(request.AssigneeId.Value, cancellationToken)
            : null;

        if (issue.Author.Id != currentUserProvider.GetCurrentUser().Id)
            throw new DomainException("You can only update your own issues");

        issue.Title = request.Title;
        issue.AssignTo(assignee);

        return issue;
    }
}