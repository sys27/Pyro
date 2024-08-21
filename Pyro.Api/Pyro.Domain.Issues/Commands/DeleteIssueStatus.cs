// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;

namespace Pyro.Domain.Issues.Commands;

public record DeleteIssueStatus(string RepositoryName, Guid Id) : IRequest;

public class DeleteIssueStatusValidator : AbstractValidator<DeleteIssueStatus>
{
    public DeleteIssueStatusValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Id)
            .NotEmpty();
    }
}

public class DeleteIssueStatusHandler : IRequestHandler<DeleteIssueStatus>
{
    private readonly IGitRepositoryRepository gitRepositoryRepository;

    public DeleteIssueStatusHandler(IGitRepositoryRepository gitRepositoryRepository)
        => this.gitRepositoryRepository = gitRepositoryRepository;

    public async Task Handle(DeleteIssueStatus request, CancellationToken cancellationToken)
    {
        var repository = await gitRepositoryRepository.GetRepository(request.RepositoryName, cancellationToken);
        if (repository is null)
            return;

        var status = repository.GetIssueStatus(request.Id);
        if (status is null)
            return;

        repository.DeleteIssueStatus(status);
    }
}