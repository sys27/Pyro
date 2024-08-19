// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Issues.Commands;

public record CreateIssue(
    string RepositoryName,
    string Title,
    Guid? AssigneeId,
    IReadOnlyList<Guid> Labels) : IRequest<Issue>;

public class CreateIssueValidator : AbstractValidator<CreateIssue>
{
    public CreateIssueValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Labels)
            .ForEach(x => x.NotEmpty());
    }
}

public class CreateIssueHandler : IRequestHandler<CreateIssue, Issue>
{
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IIssueRepository issueRepository;
    private readonly IGitRepositoryRepository gitRepositoryRepository;
    private readonly TimeProvider timeProvider;

    public CreateIssueHandler(
        ICurrentUserProvider currentUserProvider,
        IIssueRepository issueRepository,
        IGitRepositoryRepository gitRepositoryRepository,
        TimeProvider timeProvider)
    {
        this.currentUserProvider = currentUserProvider;
        this.issueRepository = issueRepository;
        this.gitRepositoryRepository = gitRepositoryRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Issue> Handle(CreateIssue request, CancellationToken cancellationToken = default)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        var repository = await gitRepositoryRepository.GetRepository(request.RepositoryName, cancellationToken) ??
                         throw new NotFoundException($"The repository (Name: {request.RepositoryName}) not found");
        var author = await issueRepository.GetUser(currentUser.Id, cancellationToken) ??
                     throw new NotFoundException($"The user (Id: {currentUser.Id}) not found");
        var assignee = request.AssigneeId.HasValue
            ? await issueRepository.GetUser(request.AssigneeId.Value, cancellationToken)
            : null;

        var issue = new Issue
        {
            Title = request.Title,
            Repository = repository,
            Author = author,
            CreatedAt = timeProvider.GetUtcNow(),
        };
        issue.AssignTo(assignee);

        foreach (var labelId in request.Labels)
        {
            var label = repository.GetLabel(labelId);
            if (label is null)
                continue;

            issue.AddLabel(label);
        }

        await issueRepository.AddIssue(issue, cancellationToken);

        return issue;
    }
}