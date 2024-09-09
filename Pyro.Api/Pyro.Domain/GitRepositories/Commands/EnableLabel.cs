// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.GitRepositories.Commands;

public record EnableLabel(string RepositoryName, Guid Id) : IRequest;

public class EnableLabelValidator : AbstractValidator<EnableLabel>
{
    public EnableLabelValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Id)
            .NotEmpty();
    }
}

public class EnableLabelHandler : IRequestHandler<EnableLabel>
{
    private readonly IGitRepositoryRepository repository;

    public EnableLabelHandler(IGitRepositoryRepository repository)
        => this.repository = repository;

    public async Task Handle(EnableLabel request, CancellationToken cancellationToken = default)
    {
        var gitRepository = await repository.GetGitRepository(request.RepositoryName, cancellationToken) ??
                            throw new NotFoundException($"The '{request.RepositoryName}' repository not found");

        var label = gitRepository.GetLabel(request.Id);
        if (label is null)
            return;

        label.IsDisabled = false;
    }
}