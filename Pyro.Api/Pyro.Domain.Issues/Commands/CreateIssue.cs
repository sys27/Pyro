// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Issues.Commands;

public record CreateIssue(string RepositoryName, string Title, Guid? AssigneeId) : IRequest<Issue>;

public class CreateIssueValidator : AbstractValidator<CreateIssue>
{
    public CreateIssueValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);
    }
}

public class CreateIssueHandler : IRequestHandler<CreateIssue, Issue>
{
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IIssueRepository repository;
    private readonly TimeProvider timeProvider;

    public CreateIssueHandler(
        ICurrentUserProvider currentUserProvider,
        IIssueRepository repository,
        TimeProvider timeProvider)
    {
        this.currentUserProvider = currentUserProvider;
        this.repository = repository;
        this.timeProvider = timeProvider;
    }

    public async Task<Issue> Handle(CreateIssue request, CancellationToken cancellationToken = default)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        var author = await repository.GetUser(currentUser.Id, cancellationToken) ??
                     throw new NotFoundException($"User (Id: {currentUser.Id}) not found");
        var assignee = request.AssigneeId.HasValue
            ? await repository.GetUser(request.AssigneeId.Value, cancellationToken)
            : null;

        var issue = new Issue
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Repository = new GitRepository(request.RepositoryName),
            Author = author,
            CreatedAt = timeProvider.GetUtcNow(),
        };
        issue.AssignTo(assignee);

        await repository.AddIssue(issue, cancellationToken);

        return issue;
    }
}