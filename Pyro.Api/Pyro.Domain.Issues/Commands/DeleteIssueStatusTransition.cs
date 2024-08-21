// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;

namespace Pyro.Domain.Issues.Commands;

public record DeleteIssueStatusTransition(string RepositoryName, Guid Id) : IRequest;

public class DeleteIssueStatusTransitionValidator : AbstractValidator<DeleteIssueStatusTransition>
{
    public DeleteIssueStatusTransitionValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Id)
            .NotEmpty();
    }
}

public class DeleteIssueStatusTransitionHandler : IRequestHandler<DeleteIssueStatusTransition>
{
    private readonly IGitRepositoryRepository gitRepositoryRepository;

    public DeleteIssueStatusTransitionHandler(IGitRepositoryRepository gitRepositoryRepository)
        => this.gitRepositoryRepository = gitRepositoryRepository;

    public async Task Handle(DeleteIssueStatusTransition request, CancellationToken cancellationToken)
    {
        var repository = await gitRepositoryRepository.GetRepository(request.RepositoryName, cancellationToken);
        if (repository is null)
            return;

        var transition = repository.GetTransition(request.Id);
        if (transition is not null)
            repository.DeleteTransition(transition);
    }
}