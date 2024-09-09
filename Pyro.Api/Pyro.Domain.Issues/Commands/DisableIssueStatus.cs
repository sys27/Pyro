// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Issues.Commands;

public record DisableIssueStatus(string RepositoryName, Guid Id) : IRequest;

public class DisableIssueStatusValidator : AbstractValidator<DisableIssueStatus>
{
    public DisableIssueStatusValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Id)
            .NotEmpty();
    }
}

public class DisableIssueStatusHandler : IRequestHandler<DisableIssueStatus>
{
    private readonly IGitRepositoryRepository gitRepositoryRepository;

    public DisableIssueStatusHandler(IGitRepositoryRepository gitRepositoryRepository)
        => this.gitRepositoryRepository = gitRepositoryRepository;

    public async Task Handle(DisableIssueStatus request, CancellationToken cancellationToken)
    {
        var repository = await gitRepositoryRepository.GetRepository(request.RepositoryName, cancellationToken) ??
                         throw new NotFoundException($"The repository (Name: {request.RepositoryName}) not found");

        var status = repository.GetIssueStatus(request.Id);
        if (status is null)
            return;

        status.IsDisabled = true;
    }
}