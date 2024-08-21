// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Issues.Commands;

public record UpdateIssue(
    string RepositoryName,
    int IssueNumber,
    string Title,
    Guid? AssigneeId,
    Guid StatusId,
    IReadOnlyList<Guid> Labels) : IRequest<Issue>;

public class UpdateIssueValidator : AbstractValidator<UpdateIssue>
{
    public UpdateIssueValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.StatusId)
            .NotEmpty();

        RuleFor(x => x.Labels)
            .ForEach(x => x.NotEmpty());
    }
}

public class UpdateIssueHandler : IRequestHandler<UpdateIssue, Issue>
{
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IIssueRepository issueRepository;
    private readonly IGitRepositoryRepository gitRepositoryRepository;

    public UpdateIssueHandler(
        ICurrentUserProvider currentUserProvider,
        IIssueRepository issueRepository,
        IGitRepositoryRepository gitRepositoryRepository)
    {
        this.currentUserProvider = currentUserProvider;
        this.issueRepository = issueRepository;
        this.gitRepositoryRepository = gitRepositoryRepository;
    }

    public async Task<Issue> Handle(UpdateIssue request, CancellationToken cancellationToken = default)
    {
        var issue = await issueRepository.GetIssue(request.RepositoryName, request.IssueNumber, cancellationToken) ??
                    throw new NotFoundException($"The issue ('{request.RepositoryName}' #{request.IssueNumber}) not found");
        if (issue.Author.Id != currentUserProvider.GetCurrentUser().Id)
            throw new DomainException("You can only update your own issues");

        var repository = await gitRepositoryRepository.GetRepository(request.RepositoryName, cancellationToken) ??
                         throw new NotFoundException($"The repository ('{request.RepositoryName}') not found");
        var assignee = request.AssigneeId is not null
            ? await gitRepositoryRepository.GetUser(request.AssigneeId.Value, cancellationToken)
            : null;

        issue.Title = request.Title;
        issue.AssignTo(assignee);
        issue.TransitionTo(request.StatusId, repository);

        issue.ClearLabels();
        foreach (var labelId in request.Labels)
        {
            var label = repository.GetLabel(labelId);
            if (label is null)
                continue;

            issue.AddLabel(label);
        }

        return issue;
    }
}