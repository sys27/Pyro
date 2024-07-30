// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Issues.Commands;

public record CreateIssueComment(
    string RepositoryName,
    int IssueNumber,
    string Content) : IRequest<IssueComment>;

public class CreateIssueCommentValidator : AbstractValidator<CreateIssueComment>
{
    public CreateIssueCommentValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.IssueNumber)
            .GreaterThan(0);

        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(2000);
    }
}

public class CreateIssueCommentHandler : IRequestHandler<CreateIssueComment, IssueComment>
{
    private readonly IIssueRepository repository;
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly TimeProvider timeProvider;

    public CreateIssueCommentHandler(
        IIssueRepository repository,
        ICurrentUserProvider currentUserProvider,
        TimeProvider timeProvider)
    {
        this.repository = repository;
        this.currentUserProvider = currentUserProvider;
        this.timeProvider = timeProvider;
    }

    public async Task<IssueComment> Handle(
        CreateIssueComment request,
        CancellationToken cancellationToken = default)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        var author = await repository.GetUser(currentUser.Id, cancellationToken) ??
                     throw new NotFoundException($"User (Id: {currentUser.Id}) not found");

        var issue = await repository.GetIssue(request.RepositoryName, request.IssueNumber, cancellationToken) ??
                    throw new NotFoundException($"Issue ('{request.RepositoryName}' #{request.IssueNumber}) not found");

        var comment = issue.AddComment(request.Content, author, timeProvider.GetUtcNow());

        return comment;
    }
}