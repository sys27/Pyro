// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Issues.Commands;

public record EnableIssueStatus(string RepositoryName, Guid Id) : IRequest;

public class EnableIssueStatusValidator : AbstractValidator<EnableIssueStatus>
{
    public EnableIssueStatusValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Id)
            .NotEmpty();
    }
}

public class EnableIssueStatusHandler : IRequestHandler<EnableIssueStatus>
{
    private readonly IGitRepositoryRepository gitRepositoryRepository;

    public EnableIssueStatusHandler(IGitRepositoryRepository gitRepositoryRepository)
        => this.gitRepositoryRepository = gitRepositoryRepository;

    public async Task Handle(EnableIssueStatus request, CancellationToken cancellationToken)
    {
        var repository = await gitRepositoryRepository.GetRepository(request.RepositoryName, cancellationToken) ??
                         throw new NotFoundException($"The repository (Name: {request.RepositoryName}) not found");

        var status = repository.GetIssueStatus(request.Id);
        if (status is null)
            return;

        status.IsDisabled = false;
    }
}