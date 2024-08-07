// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Issues.Commands;

public record UpdateIssueComment(
    string RepositoryName,
    int IssueNumber,
    Guid CommentId,
    string Content) : IRequest<IssueComment>;

public class UpdateIssueCommentValidator : AbstractValidator<UpdateIssueComment>
{
    public UpdateIssueCommentValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.IssueNumber)
            .GreaterThan(0);

        RuleFor(x => x.CommentId)
            .NotEmpty();

        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(2000);
    }
}

public class UpdateIssueCommentHandler : IRequestHandler<UpdateIssueComment, IssueComment>
{
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IIssueRepository repository;

    public UpdateIssueCommentHandler(ICurrentUserProvider currentUserProvider, IIssueRepository repository)
    {
        this.currentUserProvider = currentUserProvider;
        this.repository = repository;
    }

    public async Task<IssueComment> Handle(UpdateIssueComment request, CancellationToken cancellationToken = default)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        var issue = await repository.GetIssue(request.RepositoryName, request.IssueNumber, cancellationToken) ??
                    throw new NotFoundException($"Issue ('{request.RepositoryName}' #{request.IssueNumber}) not found");

        var comment = issue.GetComment(request.CommentId) ??
                      throw new NotFoundException($"Comment ('{request.CommentId}') not found");

        if (comment.Author.Id != currentUser.Id)
            throw new DomainException("You can only update your own comments");

        comment.Content = request.Content;

        return comment;
    }
}