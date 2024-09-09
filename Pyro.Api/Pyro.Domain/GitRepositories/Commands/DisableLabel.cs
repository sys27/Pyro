// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.GitRepositories.Commands;

public record DisableLabel(string RepositoryName, Guid Id) : IRequest;

public class DisableLabelValidator : AbstractValidator<DisableLabel>
{
    public DisableLabelValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Id)
            .NotEmpty();
    }
}

public class DisableLabelHandler : IRequestHandler<DisableLabel>
{
    private readonly IGitRepositoryRepository repository;

    public DisableLabelHandler(IGitRepositoryRepository repository)
        => this.repository = repository;

    public async Task Handle(DisableLabel request, CancellationToken cancellationToken = default)
    {
        var gitRepository = await repository.GetGitRepository(request.RepositoryName, cancellationToken) ??
                            throw new NotFoundException($"The '{request.RepositoryName}' repository not found");

        var label = gitRepository.GetLabel(request.Id);
        if (label is null)
            return;

        label.IsDisabled = true;
    }
}