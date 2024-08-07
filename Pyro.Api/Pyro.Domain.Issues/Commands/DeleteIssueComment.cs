// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Issues.Commands;

public record DeleteIssueComment(
    string RepositoryName,
    int IssueNumber,
    Guid CommentId) : IRequest;

public class DeleteIssueCommentValidator : AbstractValidator<DeleteIssueComment>
{
    public DeleteIssueCommentValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.IssueNumber)
            .GreaterThan(0);

        RuleFor(x => x.CommentId)
            .NotEmpty();
    }
}

public class DeleteIssueCommentHandler : IRequestHandler<DeleteIssueComment>
{
    private readonly IIssueRepository repository;

    public DeleteIssueCommentHandler(IIssueRepository repository)
        => this.repository = repository;

    public async Task Handle(DeleteIssueComment request, CancellationToken cancellationToken)
    {
        var issue = await repository.GetIssue(request.RepositoryName, request.IssueNumber, cancellationToken) ??
                    throw new NotFoundException($"Issue (Id: {request.CommentId}) not found");

        var comment = issue.GetComment(request.CommentId);
        if (comment is not null)
            issue.DeleteComment(comment);
    }
}